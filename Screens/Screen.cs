using System.Collections.Concurrent;
using System.Linq.Expressions;

using Box.Graphics.Batch;
using Box.Services;
using Box.Services.Types;

namespace Box.Screens;

/// <summary>
/// Represents a base class for screens within the application.
/// </summary>
public class Screen
{
	internal TimeManager Timers = new();
	internal bool IsDirty = true;

	private readonly List<Entity> _entities = new();
	private readonly Dictionary<string, List<Action<SignalHandle>>> _signals = new();
	private readonly List<CoroutineHandle> _coroutines = new();
	private IEnumerable<Entity> _orderedEntities;
	private int _entityCount, _entityRoutineCount;
	private bool _internalExitScreen;



	// Cache: (Type, argTypesSignature) -> compiled constructor delegate
	private readonly ConcurrentDictionary<Type, ConcurrentBag<Entity>> _entityPools
		= new();

	private readonly ConcurrentDictionary<string, Func<object[], Entity>> _ctorCache
		= new();

	private void ResetEntity(Entity entity)
	{
		entity.ClearChildren();
		entity.ClearSignals();
		entity.ClearRoutines();
		entity.ClearTimers();
		entity.Layer = 0;
		entity.Position = Vect2.Zero;
		entity.Size = Vect2.Zero;
	}

	public T SpawnEntity<T>(params object[] args) where T : Entity
	{
		var type = typeof(T);

		// try reuse
		var pool = _entityPools.GetOrAdd(type, _ => new ConcurrentBag<Entity>());
		if (pool.TryTake(out var recycled))
		{
			ResetEntity(recycled);
			recycled.Screen = this;
			recycled.EngineOnEnter();
			_entities.Add(recycled);
			IsDirty = true;
			return (T)recycled;
		}

		// no recycled → create via reflection
		var instance = (T)CreateWithReflection(type, args);

		instance.Screen = this;
		instance.EngineOnEnter();
		_entities.Add(instance);
		IsDirty = true;
		return instance;
	}

	private Entity CreateWithReflection(Type type, object[] args)
	{
		foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
		{
			var ps = ctor.GetParameters();
			bool isParams = ps.LastOrDefault()?.GetCustomAttribute<ParamArrayAttribute>() != null;
			int fixedCount = isParams ? ps.Length - 1 : ps.Length;

			// Reject only if you passed *too many* args
			if (!isParams && args.Length > fixedCount)
				continue;
			// For params ctors, you need at least the fixed arguments
			if (isParams && args.Length < fixedCount)
				continue;

			var finalArgs = new object[ps.Length];
			bool match = true;

			// 1) Fill fixed parameters from your args[] or defaults
			for (int i = 0; i < fixedCount; i++)
			{
				if (i < args.Length)
				{
					// type check
					if (!ps[i].ParameterType.IsInstanceOfType(args[i]))
					{
						match = false;
						break;
					}
					finalArgs[i] = args[i];
				}
				else
				{
					// no arg supplied → must be optional
					if (!ps[i].IsOptional)
					{
						match = false;
						break;
					}
					finalArgs[i] = ps[i].DefaultValue;
				}
			}
			if (!match)
				continue;

			// 2) Pack any extra args into the params-array
			if (isParams)
			{
				var elemType = ps.Last().ParameterType.GetElementType()!;
				int varCount = args.Length - fixedCount;
				var arr = Array.CreateInstance(elemType, varCount);
				for (int j = 0; j < varCount; j++)
					arr.SetValue(args[fixedCount + j], j);
				finalArgs[fixedCount] = arr;
			}

			// 3) We have a match—invoke it
			return (Entity)ctor.Invoke(finalArgs)!;
		}

		// fallback parameterless
		if (args.Length == 0 && type.GetConstructor(Type.EmptyTypes) is ConstructorInfo defCtor)
			return (Entity)defCtor.Invoke(null)!;

		throw new MissingMethodException(
			$"No constructor on {type.Name} matched {args.Length} args."
		);
	}






	/// <summary>
	/// The layer of the screen.
	/// <para>Note: Entity layers cannot exceed screen layers.</para>
	/// </summary>
	public int Layer { get; set; }

	/// <summary>
	/// Gets the size of the safe region from the engine settings.
	/// </summary>
	/// <value>
	/// A <see cref="float"/> representing the safe region size.
	/// </value>
	public float SafeRegion => Engine.GetService<EngineSettings>().SafeRegion;

	/// <summary>
	/// Indicates whether the screen is currently exiting or closing.
	/// </summary>
	public bool IsExiting { get; private set; }

	/// <summary>
	/// The camera associated with this screen.
	/// </summary>
	public Camera Camera { get; internal set; }

	/// <summary>
	/// Determines if the screen is visible.
	/// </summary>
	public bool Visible { get; set; } = true;

	/// <summary>
	/// Indicates if this screen is a UI screen.
	/// <para>Note: This is required for UI/overlay screens that display UI elements.</para>
	/// </summary>
	public bool IsUiScreen { get; set; }

	/// <summary>
	/// Indicates if this screen is currently the topmost screen.
	/// </summary>
	public bool IsTopmostScreen { get; internal set; }

	/// <summary>
	/// Indicates if this screen has focus.
	/// </summary>
	public bool IsActive { get; internal set; }

	/// <summary>
	/// Indicates if this screen has focus and is the topmost screen.
	/// </summary>
	public bool IsActiveScreen => IsActive && IsTopmostScreen;

	/// <summary>
	/// Total number of entities on this screen.
	/// </summary>
	public int EntityCount => Entities.Count();

	/// <summary>
	/// Retrieves all entities on this screen that are not null or exiting.
	/// </summary>
	public IEnumerable<Entity> Entities
	{
		get
		{
			if (_entities.Count == 0)
				yield break;

			foreach (Entity entity in _entities.ToList())
			{
				if (entity is null || entity.IsExiting)
					continue;

				yield return entity;
			}
		}
	}



	#region Helpers
	/// <summary>
	/// Provides access to the ScreenManager singleton instance.
	/// </summary>
	protected ScreenManager ScreenManager => Service.GetService<ScreenManager>();

	/// <summary>
	/// Provides access to the Assets singleton instance.
	/// </summary>
	protected Assets Assets => Service.GetService<Assets>();

	/// <summary>
	/// Provides access to the Engine singleton instance.
	/// </summary>
	protected Engine Engine => Service.GetService<Engine>();

	/// <summary>
	/// Provides access to the InputMap instance from the Engine.
	/// </summary>
	protected InputMap Input => Engine.Input;

	/// <summary>
	/// Provides access to the Clock singleton instance.
	/// </summary>
	protected Clock Clock => Service.GetService<Clock>();

	/// <summary>
	/// Provides access to the Signal singleton instance.
	/// </summary>
	protected Signal Signal => Service.GetService<Signal>();

	/// <summary>
	/// Provides access to the Coroutine singleton instance.
	/// </summary>
	protected Coroutine Coroutine => Service.GetService<Coroutine>();

	/// <summary>
	/// Provides access to the Renderer singleton instance.
	/// </summary>
	protected Renderer Renderer => Service.GetService<Renderer>();

	/// <summary>
	/// Provides access to the Rand singleton instance.
	/// </summary>
	protected FastRandom Rand => Service.GetService<FastRandom>();

	/// <summary>
	/// Provides access to the SoundManager singleton instance.
	/// </summary>
	protected SoundManager SoundManager => Service.GetService<SoundManager>();

	/// <summary>
	/// Provides access to the Log singleton instance.
	/// </summary>
	protected Log Log => Service.GetService<Log>();

	/// <summary>
	/// Gets the instance of the <see cref="ServiceManager"/> associated with the engine.
	/// </summary>
	/// <remarks>
	/// This property provides access to the <see cref="ServiceManager"/> for managing and resolving services
	/// in the engine. It acts as a convenient way to interact with the engine's service container.
	/// </remarks>
	protected ServiceCollection Service => Engine.Services;

	/// <summary>
	/// Gets the current size of the viewport as a <see cref="Vect2"/>.
	/// </summary>
	/// <remarks>
	/// The viewport size is determined by the renderer and represents the visible dimensions of the screen.
	/// </remarks>
	public Vect2 Viewport => Renderer.Size;

	/// <summary>
	/// Gets the width of the viewport in pixels.
	/// </summary>
	/// <remarks>
	/// This value is derived from the X component of the <see cref="Viewport"/> property.
	/// </remarks>
	public int Width => (int)Viewport.X;

	/// <summary>
	/// Gets the height of the viewport in pixels.
	/// </summary>
	/// <remarks>
	/// This value is derived from the Y component of the <see cref="Viewport"/> property.
	/// </remarks>
	public int Height => (int)Viewport.Y;

	/// <summary>
	/// Retrieves the singleton instance of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the singleton to retrieve.</typeparam>
	/// <returns>The singleton instance of the specified type.</returns>
	protected T GetService<T>() where T : GameService => Engine.GetService<T>();

	/// <summary>
	/// Retrieves a Surface asset by its name.
	/// </summary>
	/// <param name="name">The name of the Surface asset to retrieve.</param>
	/// <returns>The Surface asset associated with the specified name.</returns>
	protected Surface GetSurface(string name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Retrieves a Surface asset by its name.
	/// </summary>
	/// <param name="name">The name of the Surface asset to retrieve.</param>
	/// <returns>The Surface asset associated with the specified name.</returns>
	protected Surface GetSurface(Enum name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Loads a Surface asset from a file.
	/// </summary>
	/// <param name="filename">The filename of the Surface asset to load.</param>
	/// <returns>The Surface asset loaded from the specified file.</returns>
	protected Surface GetSurfaceFromFile(string filename) => Assets.GetFromFile<Surface>(filename);

	/// <summary>
	/// Retrieves a surface from the specified tileset using its filename.
	/// </summary>
	/// <param name="tileset">The tileset containing the filename of the surface.</param>
	/// <returns>The surface associated with the tileset's filename.</returns>
	protected Surface GetSurfaceFromTileset(MapTileset tileset) => Assets.GetFromFile<Surface>(tileset.Filename);

	/// <summary>
	/// Retrieves a <see cref="Surface"/> from a tileset in the given <see cref="Map"/> by its unique tileset ID.
	/// </summary>
	/// <param name="map">The map containing the tileset collection.</param>
	/// <param name="id">The unique ID of the tileset to retrieve the surface for.</param>
	/// <returns>The <see cref="Surface"/> associated with the specified tileset ID.</returns>
	/// <exception cref="KeyNotFoundException">
	/// Thrown if no tileset with the specified ID is found in the map.
	/// </exception>
	protected Surface GetSurfaceFromTileset(Map map, int id)
	{
		var element = map.Tilesets
			.Select(x => x.Value)
			.FirstOrDefault(x => x.Id == id);

		if (element.IsEmpty)
			throw new KeyNotFoundException($"Unable to find tileset with ID '{id}' in the provided map.");

		return GetSurfaceFromTileset(element);
	}

	/// <summary>
	/// Retrieves a Map asset by its name.
	/// </summary>
	/// <param name="name">The name of the Map asset to retrieve.</param>
	/// <returns>The Map asset associated with the specified name.</returns>
	protected Map GetMap(string name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Map asset by its name.
	/// </summary>
	/// <param name="name">The name of the Map asset to retrieve.</param>
	/// <returns>The Map asset associated with the specified name.</returns>
	protected Map GetMap(Enum name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Sound asset by its name.
	/// </summary>
	/// <param name="name">The name of the Sound asset to retrieve.</param>
	/// <returns>The Sound asset associated with the specified name.</returns>
	protected Sound GetSound(string name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Sound asset by its name.
	/// </summary>
	/// <param name="name">The name of the Sound asset to retrieve.</param>
	/// <returns>The Sound asset associated with the specified name.</returns>
	protected Sound GetSound(Enum name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Font asset by its name.
	/// </summary>
	/// <param name="name">The name of the Font asset to retrieve.</param>
	/// <returns>The Font asset associated with the specified name.</returns>
	protected BoxFont GetFont(string name) => Assets.GetFont(name);

	/// <summary>
	/// Retrieves a Font asset by its name.
	/// </summary>
	/// <param name="name">The name of the Font asset to retrieve.</param>
	/// <returns>The Font asset associated with the specified name.</returns>
	protected BoxFont GetFont(Enum name) => Assets.GetFont(name);

	/// <summary>
	/// Retrieves a Spritesheet asset by its name.
	/// </summary>
	/// <param name="name">The name of the Spritesheet asset to retrieve.</param>
	/// <returns>The Spritesheet asset associated with the specified name.</returns>
	protected Spritesheet GetSheet(string name) => Assets.Get<Spritesheet>(name);

	/// <summary>
	/// Retrieves a Spritesheet asset by its name.
	/// </summary>
	/// <param name="name">The name of the Spritesheet asset to retrieve.</param>
	/// <returns>The Spritesheet asset associated with the specified name.</returns>
	protected Spritesheet GetSheet(Enum name) => Assets.Get<Spritesheet>(name);



	/// <summary>
	/// Loads a bitmap font from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the bitmap font.</param>
	/// <param name="spacing">The spacing between characters. Default is 0.</param>
	/// <param name="linespacing">The spacing between lines of text. Default is 0.</param>
	/// <returns>A <see cref="BitmapFont"/> object representing the loaded font.</returns>
	public BitmapFont LoadBitmapFont(string path, int spacing = 0, int linespacing = 0)
		=> Assets.LoadBitmapFont(path, spacing, linespacing);

	/// <summary>
	/// Loads a generic font with the specified settings.
	/// </summary>
	/// <param name="path">The file path to the font.</param>
	/// <param name="size">The size of the font.</param>
	/// <param name="useSmoothing">Whether to use smoothing. Default is false.</param>
	/// <param name="bold">Whether to render the font as bold. Default is false.</param>
	/// <param name="thickness">The thickness of the font. Default is 0.</param>
	/// <param name="spacing">The spacing between characters. Default is 0.</param>
	/// <param name="lineSpacing">The spacing between lines of text. Default is 0.</param>
	/// <returns>A <see cref="GenericFont"/> object representing the loaded font.</returns>
	public GenericFont LoadFont(string path, int size, bool useSmoothing = false, bool bold = false, int thickness = 0, int spacing = 0, int lineSpacing = 0)
		=> Assets.LoadFont(path, size, useSmoothing, bold, thickness, spacing, lineSpacing);

	/// <summary>
	/// Loads a map from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the map.</param>
	/// <returns>A <see cref="Map"/> object representing the loaded map.</returns>
	public Map LoadMap(string path) => Assets.LoadMap(path);

	/// <summary>
	/// Loads a pack of assets from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the asset pack.</param>
	public void LoadPack(string path) => Assets.LoadPack(path);

	/// <summary>
	/// Loads a sound from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the sound.</param>
	/// <returns>A <see cref="Sound"/> object representing the loaded sound.</returns>
	public Sound LoadSound(string path) => Assets.LoadSound(path);

	/// <summary>
	/// Loads a sprite sheet from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the sprite sheet.</param>
	/// <returns>A <see cref="Spritesheet"/> object representing the loaded sprite sheet.</returns>
	public Spritesheet LoadSpriteSheet(string path) => Assets.LoadSpriteSheet(path);

	/// <summary>
	/// Loads a subsection of a surface (texture) from the specified file path and region.
	/// </summary>
	/// <param name="path">The file path to the texture.</param>
	/// <param name="region">The rectangular region of the surface to load.</param>
	/// <param name="repeat">Whether the texture should repeat. Default is false.</param>
	/// <param name="smooth">Whether to use smoothing on the texture. Default is false.</param>
	/// <returns>A <see cref="Surface"/> object representing the loaded subsection.</returns>
	public Surface LoadSubSurface(string path, Rect2 region, bool repeat = false, bool smooth = false)
		=> Assets.LoadSubSurface(path, region, repeat, smooth);

	/// <summary>
	/// Loads a subsurface from a given spritesheet.
	/// </summary>
	/// <param name="surface">The base <see cref="Surface"/> to extract the subsurface from.</param>
	/// <param name="sheet">The <see cref="Spritesheet"/> containing the subsurface definition.</param>
	/// <param name="name">The name of the subsurface to load.</param>
	/// <param name="repeat">Optional. Specifies whether the subsurface should repeat when rendered. Defaults to <c>false</c>.</param>
	/// <param name="smooth">Optional. Specifies whether smoothing should be applied to the subsurface. Defaults to <c>false</c>.</param>
	/// <returns>The loaded <see cref="Surface"/> representing the subsurface.</returns>
	/// <remarks>
	/// This method simplifies the process of extracting and managing subsurfaces from a spritesheet,
	/// leveraging the <see cref="Assets.LoadSubSurface"/> method for asset management.
	/// </remarks>
	public Surface LoadSubSurface(Surface surface, Spritesheet sheet, string name, bool repeat = false, bool smooth = false)
		=> Assets.LoadSubSurface(surface, sheet, name, repeat, smooth);

	/// <summary>
	/// Loads a surface (texture) from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the texture.</param>
	/// <param name="repeat">Whether the texture should repeat. Default is false.</param>
	/// <param name="smooth">Whether to use smoothing on the texture. Default is false.</param>
	/// <returns>A <see cref="Surface"/> object representing the loaded texture.</returns>
	public Surface LoadSurface(string path, bool repeat = false, bool smooth = false)
		=> Assets.LoadSurface(path, repeat, smooth);

	#endregion


	#region Engine
	// internal QuadtreeNode<Entity> _entityTree = new(new(-10000, -10000, 10000, 10000));
	// private List<Entity> _visibleBuffer = new();

	internal unsafe void EngineUpdate()
	{
		Camera.Update();

		if (IsDirty)
		{
			_orderedEntities = _entities
				.Where(x => x is not null && !x.IsExiting && (x.KeepAlive || Camera.InViewport(x)) || x.AnyParentOfType<BoxRenderTarget>(out _))
				.OrderBy(x => x.Layer)
				.ToList()
			;

			IsDirty = false;
		}

		if (!IsDirty && Visible && _orderedEntities is not null)
		{
			_entityCount = 0;
			_entityRoutineCount = 0;

			foreach (var item in _orderedEntities)
			{
				_entityCount += item.Children.Count();
				_entityRoutineCount += item._coroutines.Count;

				if (!item.Visible)
					continue;

				item.EngineUpdate();
			}
		}

		Update();
	}



	internal void EngineOnEnter()
	{
		Connect(EngineSignals.ScreenDirty, OnScreenDirty);

		OnEnter();
	}

	private void OnScreenDirty(SignalHandle handle)
	{
		var screen = handle.Get<Screen>(0);

		if (screen != this)
			return;

		IsDirty = true;
	}

	internal void EngineOnExit()
	{
		if (IsExiting)
			return;

		IsExiting = true;

		// Always clear first if any entities 
		// trigger a signal on remove.
		ClearSignals();

		ClearTimers();
		ClearRoutines();
		ClearEntities();

		OnExit();
	}

	#endregion


	#region OnEnter, OnExit, Update
	/// <summary>
	/// Called when entering a state or screen. Override to provide custom behavior.
	/// </summary>
	protected virtual void OnEnter() { }

	/// <summary>
	/// Called when exiting a state or screen. Override to provide custom behavior.
	/// </summary>
	protected virtual void OnExit() { }

	/// <summary>
	/// Called on each frame update and draw. Override to provide custom behavior.
	/// </summary>
	protected virtual void Update() { }
	#endregion


	#region Screen
	/// <summary>
	/// Adds a screen to the screen manager.
	/// </summary>
	/// <param name="screen">The screen to add.</param>
	public void AddScreen(Screen screen) => ScreenManager.Add(screen);

	/// <summary>
	/// Adds multiple screens to the screen manager.
	/// </summary>
	/// <param name="screens">The screens to add.</param>
	public void AddScreen(params Screen[] screens) => ScreenManager.Add(screens);

	/// <summary>
	/// Removes a screen from the screen manager.
	/// </summary>
	/// <param name="screen">The screen to remove.</param>
	/// <returns>True if the screen was successfully removed; otherwise, false.</returns>
	public bool RemoveScreen(Screen screen) => ScreenManager.Remove(screen);

	/// <summary>
	/// Removes multiple screens from the screen manager.
	/// </summary>
	/// <param name="screens">The screens to remove.</param>
	/// <returns>True if all screens were successfully removed; otherwise, false.</returns>
	public bool RemoveScreen(params Screen[] screens) => ScreenManager.Remove(screens);

	/// <summary>
	/// Checks if a screen of the specified type is present in the screen manager.
	/// </summary>
	/// <typeparam name="T">The type of the screen to check for.</typeparam>
	/// <returns>True if a screen of the specified type is present; otherwise, false.</returns>
	public bool HasScreen<T>() where T : Screen => ScreenManager.Has<T>();

	/// <summary>
	/// Checks if the specified screen is present in the screen manager.
	/// </summary>
	/// <param name="screen">The screen to check for.</param>
	/// <returns>True if the specified screen is present; otherwise, false.</returns>
	public bool HasScreen(Screen screen) => ScreenManager.Has(screen);

	/// <summary>
	/// Retrieves a screen of the specified type from the screen manager.
	/// </summary>
	/// <typeparam name="T">The type of the screen to retrieve.</typeparam>
	/// <returns>The screen of the specified type.</returns>
	public T GetScreen<T>() where T : Screen => ScreenManager.Get<T>();

	/// <summary>
	/// Tries to retrieve a screen of the specified type from the screen manager.
	/// </summary>
	/// <typeparam name="T">The type of the screen to retrieve.</typeparam>
	/// <param name="screen">When this method returns, contains the screen of the specified type, if found; otherwise, the default value for the type of the screen parameter.</param>
	/// <returns>True if a screen of the specified type was found; otherwise, false.</returns>
	public bool TryGetScreen<T>(out T screen) where T : Screen => ScreenManager.TryGet<T>(out screen);




	/// <summary>
	/// Exits the current screen.
	/// </summary>
	public void ExitScreen()
	{
		if (_internalExitScreen)
			return;

		StartTimer(0.001f, false, () => ScreenManager.Remove(this));

		_internalExitScreen = true;
	}
	#endregion


	#region Entity
	/// <summary>
	/// Adds an entity to the entity manager.
	/// </summary>
	/// <param name="entity">The entity to add.</param>
	public void AddEntity(Entity entity) => AddEntity(entities: entity);

#nullable enable
	/// <summary>
	/// Adds an entity conditionally based on a boolean condition.
	/// </summary>
	/// <typeparam name="T">The type of entity to add.</typeparam>
	/// <param name="condition">The condition that determines whether to add the entity.</param>
	/// <param name="onTrue">The entity to add if the condition is true.</param>
	/// <param name="onFalse">The entity to add if the condition is false.</param>
	/// <returns>The added entity if the condition was met; otherwise, null.</returns>
	public T? AddEntityCondition<T>(bool condition, T? onTrue = null, T? onFalse = null) where T : Entity
	{
		if (condition)
		{
			if (onTrue is not null)
			{
				AddEntity(onTrue);

				return onTrue;
			}
		}
		else
		{
			if (onFalse is not null)
			{
				AddEntity(onFalse);

				return onFalse;
			}
		}

		return null;
	}
#nullable disable

	/// <summary>
	/// Adds multiple entities to the entity manager.
	/// </summary>
	/// <param name="entities">The entities to add.</param>
	public unsafe void AddEntity(params Entity[] entities)
	{
		fixed (Entity* ptr = entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				var entity = ptr + i;

				if (*entity is null)
					continue;
				if (entity->IsExiting)
					continue;

				// _chunks.Add(*entity);

				entity->Screen = this;
				entity->EngineOnEnter();

				_entities.Add(*entity);

				Emit(EngineSignals.EntityAdded, *entity, entity->Parent);
			}
		}

		IsDirty = true;
	}

	/// <summary>
	/// Retrieves an entity of the specified type from the entity manager.
	/// </summary>
	/// <typeparam name="T">The type of entity to retrieve.</typeparam>
	/// <returns>The entity of the specified type.</returns>
	public T GetEntity<T>() where T : Entity
	{
		return (T)Entities.FirstOrDefault(x => x is T);
	}

	/// <summary>
	/// Retrieves an entity of the specified type at the specified index from the entity manager.
	/// </summary>
	/// <typeparam name="T">The type of entity to retrieve.</typeparam>
	/// <param name="index">The index of the entity.</param>
	/// <returns>The entity of the specified type at the specified index.</returns>
	public T GetEntity<T>(int index) where T : Entity
	{
		return (T)Entities.ElementAtOrDefault(index);
	}

	/// <summary>
	/// Checks if an entity of the specified type exists in the entity manager.
	/// </summary>
	/// <typeparam name="T">The type of entity to check for.</typeparam>
	/// <returns>True if an entity of the specified type exists; otherwise, false.</returns>
	public bool HasEntity<T>() where T : Entity
	{
		return GetEntity<T>() is not null;
	}

	/// <summary>
	/// Checks if the specified entity exists in the entity manager.
	/// </summary>
	/// <param name="entity">The entity to check for.</param>
	/// <returns>True if the specified entity exists; otherwise, false.</returns>
	public bool HasEntity(Entity entity)
	{
		return entity is not null && _entities.Contains(entity) && !entity.IsExiting;
	}

	/// <summary>
	/// Removes the specified entity from the entity manager.
	/// </summary>
	/// <param name="entity">The entity to remove.</param>
	/// <returns>True if the entity was successfully removed; otherwise, false.</returns>
	public bool RemoveEntity(Entity entity)
	{
		if (entity is null)
			return false;
		if (entity.IsExiting)
			return false;

		if (_entities.Remove(entity))
		{
			entity.EngineOnExit();

			Emit(EngineSignals.EntityRemoved, entity, entity.Parent);

			IsDirty = true;

			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes multiple entities from the entity manager.
	/// </summary>
	/// <param name="entities">The entities to remove.</param>
	/// <returns>True if all entities were successfully removed; otherwise, false.</returns>
	public bool RemoveEntity(params Entity[] entities)
	{
		if (entities.IsEmpty())
			return false;

		bool[] check = new bool[entities.Length];
		bool[] result = new bool[entities.Length];

		for (int i = 0; i < entities.Length; i++)
		{
			check[i] = true;
			result[i] = RemoveEntity(entities[i]);
		}

		return result.SequenceEqual(check);
	}

	/// <summary>
	/// Clears all entities from the entity manager.
	/// </summary>
	public void ClearEntities()
	{
		if (_entities.IsEmpty())
			return;

		var items = _entities.ToArray();

		unsafe
		{
			fixed (Entity* ptr = items)
			{
				for (int i = 0; i < items.Length; i++)
				{
					var item = ptr + i;

					RemoveEntity(*item);
				}
			}
		}
	}
	#endregion


	#region Coroutines
	/// <summary>
	/// Starts a coroutine with a delay before execution.
	/// </summary>
	/// <param name="delay">The delay in seconds before starting the coroutine.</param>
	/// <param name="routine">The coroutine routine to start.</param>
	/// <returns>A handle to the coroutine.</returns>
	public CoroutineHandle StartRoutineDelayed(float delay, IEnumerator routine)
	{
		if (IsExiting)
			return default;

		var handle = Engine.GetService<Coroutine>().RunDelayed(delay, routine);

		_coroutines.Add(handle);

		return handle;
	}

	/// <summary>
	/// Starts a coroutine immediately.
	/// </summary>
	/// <param name="routine">The coroutine routine to start.</param>
	/// <returns>A handle to the coroutine.</returns>
	public CoroutineHandle StartRoutine(IEnumerator routine)
		=> StartRoutineDelayed(0f, routine);

	/// <summary>
	/// Checks if a coroutine with the specified handle is currently active.
	/// </summary>
	/// <param name="handle">The handle of the coroutine to check.</param>
	/// <returns>True if the coroutine is active; otherwise, false.</returns>
	public bool HasRoutine(CoroutineHandle handle)
		=> _coroutines.Any(x => x == handle);

	/// <summary>
	/// Checks if a specific coroutine enumerator is currently active.
	/// </summary>
	/// <param name="enumerator">The coroutine enumerator to check.</param>
	/// <returns>True if the coroutine is active; otherwise, false.</returns>
	public bool HasRoutine(IEnumerator enumerator)
		=> _coroutines.Any(x => x.Enumerator == enumerator);

	/// <summary>
	/// Stops a coroutine with the specified handle.
	/// </summary>
	/// <param name="handle">The handle of the coroutine to stop.</param>
	/// <returns>True if the coroutine was successfully stopped; otherwise, false.</returns>
	public bool StopRoutine(CoroutineHandle handle)
		=> StopRoutine(handle.Enumerator);

	/// <summary>
	/// Stops a coroutine specified by its enumerator.
	/// </summary>
	/// <param name="handle">The coroutine enumerator to stop.</param>
	/// <returns>True if the coroutine was successfully stopped; otherwise, false.</returns>
	public bool StopRoutine(IEnumerator handle)
	{
		Coroutine.Stop(handle);

		if (!HasRoutine(handle))
			return false;

		var routine = _coroutines.FirstOrDefault(x => x.Enumerator == handle);

		return Coroutine.Stop(routine);
	}

	/// <summary>
	/// Clears all active coroutines.
	/// </summary>
	public void ClearRoutines()
	{
		if (_coroutines.Count == 0)
			return;

		foreach (CoroutineHandle routine in _coroutines.ToList())
		{
			StopRoutine(routine);

			_coroutines.Remove(routine);
		}
	}
	#endregion


	#region Signal
	/// <summary>
	/// Connects a handler function to a signal by name.
	/// </summary>
	/// <param name="name">The name of the signal to connect to.</param>
	/// <param name="handle">The handler function to invoke when the signal is emitted.</param>
	public void Connect(string name, Action<SignalHandle> handle)
	{
		if (!_signals.TryGetValue(name, out var value))
			_signals.Add(name, new() { handle });
		else
			value.Add(handle);

		Signal.Connect(name, handle);
	}

	/// <summary>
	/// Connects a handler function to a signal using an enumeration value.
	/// </summary>
	/// <param name="name">The enumeration representing the signal to connect to.</param>
	/// <param name="handle">The handler function to invoke when the signal is emitted.</param>
	public void Connect(Enum name, Action<SignalHandle> handle)
		=> Connect(name.ToEnumString(), handle);

	/// <summary>
	/// Emits a signal by name with optional data.
	/// </summary>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass along with the signal.</param>
	public void Emit(string name, params object[] data)
	{
		// if (!HasSignal(name))
		//     return;

		Signal.Emit(name, data);
	}

	/// <summary>
	/// Emits a signal using an enumeration value with optional data.
	/// </summary>
	/// <param name="name">The enumeration representing the signal to emit.</param>
	/// <param name="data">Optional data to pass along with the signal.</param>
	public void Emit(Enum name, params object[] data)
	{
		// if (!HasSignal(name))
		//     return;

		Signal.Emit(name, data);
	}

	/// <summary>
	/// Emits a signal by name after a specified delay, with optional data.
	/// </summary>
	/// <param name="delay">The delay in seconds before emitting the signal.</param>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass along with the signal.</param>
	public void EmitDelayed(float delay, string name, params object[] data)
	{
		// if (!HasSignal(name))
		//     return;

		Signal.EmitDelayed(delay, name, data);
	}

	/// <summary>
	/// Emits a signal using an enumeration value after a specified delay, with optional data.
	/// </summary>
	/// <param name="delay">The delay in seconds before emitting the signal.</param>
	/// <param name="name">The enumeration representing the signal to emit.</param>
	/// <param name="data">Optional data to pass along with the signal.</param>
	public void EmitDelayed(float delay, Enum name, params object[] data)
	{
		// if (!HasSignal(name))
		//     return;

		Signal.EmitDelayed(delay, name, data);
	}

	/// <summary>
	/// Checks if a signal with the specified name is connected.
	/// </summary>
	/// <param name="name">The name of the signal to check.</param>
	/// <returns>True if the signal is connected; otherwise, false.</returns>
	public bool HasSignal(string name) => _signals.ContainsKey(name);

	/// <summary>
	/// Checks if a signal represented by an enumeration value is connected.
	/// </summary>
	/// <param name="name">The enumeration representing the signal to check.</param>
	/// <returns>True if the signal is connected; otherwise, false.</returns>
	public bool HasSignal(Enum name)
		=> HasSignal(name.ToEnumString());

	/// <summary>
	/// Disconnects all handlers from a signal by name.
	/// </summary>
	/// <param name="name">The name of the signal to disconnect.</param>
	/// <returns>True if the signal was successfully disconnected; otherwise, false.</returns>
	public bool Disconnect(string name)
	{
		if (_signals.TryGetValue(name, out var signals))
		{
			foreach (Action<SignalHandle> signal in signals)
			{
				Signal.Disconnect(name, signal);
			}

			_signals.Remove(name);

			return true;
		}

		return false;
	}

	/// <summary>
	/// Disconnects all handlers from a signal using an enumeration value.
	/// </summary>
	/// <param name="name">The enumeration representing the signal to disconnect.</param>
	/// <returns>True if the signal was successfully disconnected; otherwise, false.</returns>
	public bool Disconnect(Enum name)
		=> Disconnect(name.ToEnumString());

	/// <summary>
	/// Clears all connected signals and their handlers.
	/// </summary>
	public void ClearSignals()
	{
		if (_signals.Count == 0)
			return;

		foreach (var signal in new Dictionary<string, List<Action<SignalHandle>>>(_signals))
		{
			foreach (Action<SignalHandle> action in signal.Value)
			{
				Signal.Disconnect(signal.Key, action);
			}

			_signals.Remove(signal.Key);
		}
	}
	#endregion


	#region Timers
	/// <summary>
	/// Adds a timer with the specified name, delay, repeat flag, and action.
	/// </summary>
	/// <param name="name">The name of the timer.</param>
	/// <param name="delay">The delay in seconds before the timer executes.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer triggers.</param>
	public void StartTimer(string name, float delay, bool repeat, Action action)
		=> Timers.Add(name, delay, repeat, action);

	/// <summary>
	/// Adds a timer with the specified enumeration value, delay, repeat flag, and action.
	/// </summary>
	/// <param name="name">The enumeration representing the timer.</param>
	/// <param name="delay">The delay in seconds before the timer executes.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer triggers.</param>
	public void StartTimer(Enum name, float delay, bool repeat, Action action)
		=> Timers.Add(name, delay, repeat, action);

	/// <summary>
	/// Adds a timer with a delay, repeat flag, and action.
	/// </summary>
	/// <param name="delay">The delay in seconds before the timer executes.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer triggers.</param>
	public void StartTimer(float delay, bool repeat, Action action)
		=> Timers.Add(delay, repeat, action);

	/// <summary>
	/// Checks if a timer with the specified name exists.
	/// </summary>
	/// <param name="name">The name of the timer to check.</param>
	/// <returns>True if the timer exists; otherwise, false.</returns>
	public bool TimerExists(string name) => Timers.Exists(name);

	/// <summary>
	/// Checks if a timer represented by an enumeration value exists.
	/// </summary>
	/// <param name="name">The enumeration representing the timer to check.</param>
	/// <returns>True if the timer exists; otherwise, false.</returns>
	public bool TimerExists(Enum name) => Timers.Exists(name);

	/// <summary>
	/// Stops a timer with the specified name.
	/// </summary>
	/// <param name="name">The name of the timer to stop.</param>
	/// <returns>True if the timer was successfully stopped; otherwise, false.</returns>
	public bool StopTimer(string name) => Timers.Stop(name);

	/// <summary>
	/// Stops a timer represented by an enumeration value.
	/// </summary>
	/// <param name="name">The enumeration representing the timer to stop.</param>
	/// <returns>True if the timer was successfully stopped; otherwise, false.</returns>
	public bool StopTimer(Enum name) => Timers.Stop(name);

	/// <summary>
	/// Clears all active timers.
	/// </summary>
	public void ClearTimers() => Timers.Clear();
	#endregion
}

