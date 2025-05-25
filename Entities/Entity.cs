using System.Runtime.CompilerServices;

using Box.Graphics.Batch;
using Box.Services.Types;

namespace Box.Entities;

/// <summary>
/// Represents a base class for game entities.
/// </summary>
public class Entity
{
	// Enity Engine Settings:
	private const float MoveEpsilon = 0.001f;       // i.e. ≈0.03 units tolerance
	private const float MoveEpsilonSqr = MoveEpsilon * MoveEpsilon;

	private bool _keepAlive;
	private string _name = Guid.NewGuid().ToString();
	private readonly TimeManager _timers = new();
	private readonly List<Entity> _children = new(); // Only for reference, will not be used directly.
	private readonly Dictionary<string, List<Action<SignalHandle>>> _signals = new();
	internal readonly List<CoroutineHandle> _coroutines = new();
	private Vect2 _position = Vect2.Zero;
	private Vect2 _size = Vect2.Zero;
	private int _layer;
	private bool _visible = true;
	internal bool _isDirty;

	/// <summary>
	/// Gets the collection of child entities associated with this entity.
	/// </summary>
	public IEnumerable<Entity> Children
	{
		get
		{
			if (_children.Count == 0)
				yield break;

			foreach (var item in _children.ToList())
			{
				if (item is null || item.IsExiting)
					continue;

				yield return item;
			}
		}
	}

	/// <summary>
	/// Gets or sets whether the entity is currently in the process of exiting.
	/// </summary>
	public bool IsExiting { get; private set; }

	/// <summary>
	/// Gets the parent entity that contains this entity.
	/// </summary>
	public Entity Parent { get; internal set; }

	/// <summary>
	/// Determines if this entity has a parent.
	/// </summary>
	public bool IsParent => Parent is null;

	/// <summary>
	/// Determines if this entity is a child of another entity.
	/// </summary>
	public bool IsChild => Parent is not null;

	/// <summary>
	/// Gets the screen that contains this entity.
	/// </summary>
	public Screen Screen { get; internal set; }

	/// <summary>
	/// Gets the camera associated with the screen that contains this entity.
	/// </summary>
	public Camera Camera => Screen.Camera;

	/// <summary>
	/// Gets the number of child entities that this entity contains.
	/// </summary>
	public int ChildCount => _children.Count;

	/// <summary>
	/// Gets the center point of the entity as a <see cref="Vect2"/>.
	/// </summary>
	/// <remarks>
	/// The center is calculated as half the size of the entity, assuming the size is represented as a 2D vector.
	/// </remarks>
	public Vect2 Center => Size / 2f;

	/// <summary>
	/// Gets the index of this entity among its parent's child entities, if it has a parent; otherwise, returns -1.
	/// </summary>
	public int ChildIndex => IsChild
		? Parent._children.IndexOf(this)
		: -1
		;

	/// <summary>
	/// Gets the local bounding rectangle of the entity relative to its own position and size.
	/// </summary>
	public Rect2 LocalBounds => new(_position, _size);

	/// <summary>
	/// Gets the bounding rectangle of the entity in global coordinates.
	/// </summary>
	public Rect2 Bounds => GlobalBounds;

	/// <summary>
	/// Gets the global bounding rectangle of the entity, considering its parent-child hierarchy if applicable.
	/// </summary>
	public Rect2 GlobalBounds => IsChild
		? new Rect2(Parent.Position + _position, _size)
		: new Rect2(_position, _size);

	/// <summary>
	/// Gets or sets the name of the entity. If not explicitly set, a unique name is generated based on existing names.
	/// </summary>
	public string Name
	{
		get => _name;
		set
		{
			if (value.IsEmpty() && _name.Length > 0) // cannot be null or empty
				return;

			StartRoutine(ProcessName(value));
		}
	}

	private IEnumerator ProcessName(string name)
	{
		if (Screen is null)
		{
			while (Screen is null)
				yield return null;
		}

		var items = Screen.Entities
			.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

		if (items.Any(x => x == this)) // name already set to this
			yield break;

		if (!items.IsEmpty())
		{
			string orginalName = name.RemoveNumbers();
			int index = 0;

			while (true)
			{
				string tempName = $"{orginalName}{index}";

				if (Screen.Entities.Any(x => x.Name.Contains(tempName, StringComparison.OrdinalIgnoreCase)))
				{
					index++;
					continue;
				}

				_name = tempName;

				break;
			}
		}
		else
			_name = name;

		Emit(EngineSignals.EntityNameChanged, this, _name);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the entity is visible.
	/// </summary>
	public bool Visible
	{
		get
		{
			if (IsChild)
				return Parent.Visible && _visible;
			else
				return _visible;
		}
		set
		{
			if (_visible == value)
				return;

			_visible = value;

			if (Screen is null)
				// Coroutine.Instance.Run(CoroutineHelper.WaitForNullObject(() => Screen, (screen) => screen._isDirty = true));
				Coroutine.Run(CoroutineHelper.WaitForObject<Screen>(() => Screen,
					(screen) => Emit(EngineSignals.ScreenDirty, screen)));
			else
				// Screen._isDirty = true;
				Emit(EngineSignals.ScreenDirty, Screen);
		}
	}

	/// <summary>
	/// Gets or sets whether the entity should be kept alive even if not currently not visible to the screen.
	/// </summary>
	public bool KeepAlive
	{
		get
		{
			if (IsChild)
				return Parent.KeepAlive;// && _keepAlive;

			return _keepAlive;
		}
		set
		{
			if (_keepAlive == value)
				return;

			_keepAlive = value;

			if (Screen is null)
				Coroutine.Run(CoroutineHelper.WaitForObject<Screen>(() => Screen, (screen)
					=> Emit(EngineSignals.ScreenDirty, screen)));
			else
				Emit(EngineSignals.ScreenDirty, Screen);
		}
	}

	/// <summary>
	/// Gets or sets the layer index of the entity.
	/// </summary>
	public int Layer
	{
		get
		{
			if (IsChild)
				return Parent.Layer + _layer;
			else
				return _layer;
		}
		set
		{
			if (_layer == value)
				return;

			_layer = value;

			if (Screen is null)
				// Coroutine.Instance.Run(CoroutineHelper.WaitForNullObject(() => Screen, (screen) => screen._isDirty = true));
				Coroutine.Run(CoroutineHelper.WaitForObject<Screen>(()
					=> Screen, (screen) => Emit(EngineSignals.ScreenDirty, screen)));
			else
				// Screen._isDirty = true;
				Emit(EngineSignals.ScreenDirty, Screen);
		}
	}

	/// <summary>
	/// Gets or sets the local position relative to the parent entity.
	/// </summary>
	public Vect2 LocalPosition
	{
		get => GetLocalPosition(this);
		set => GlobalPosition = value;
	}

	/// <summary>
	/// Gets or sets the position relative to the screen or world coordinates.
	/// </summary>
	public Vect2 Position
	{
		get => GetGlobalPosition(this);
		set => GlobalPosition = value;
	}

	/// <summary>
	/// Gets the global position of the entity in the world coordinates.
	/// </summary>
	public Vect2 GlobalPosition
	{
		get => GetGlobalPosition(this);
		set
		{
			if (Vect2.DistanceSquared(_position, value) < 0.025f)
				return;

			_position = value;

			if (Screen is null)
				Coroutine.Run(CoroutineHelper.WaitForObject<Screen>(() => Screen, (screen) =>
					Emit(EngineSignals.ScreenDirty, screen)));
			else
			{
				Emit(EngineSignals.ScreenDirty, Screen);
			}
		}
	}

	/// <summary>
	/// Gets the width of the entity.
	/// </summary>
	public float Width => Size.X;

	/// <summary>
	/// Gets the height of the entity.
	/// </summary>
	public float Height => Size.Y;

	/// <summary>
	/// Gets or sets the size of the entity.
	/// </summary>
	public Vect2 Size
	{
		get => _size;
		set
		{
			if (Vect2.DistanceSquared(_size, value) < MoveEpsilonSqr)
				return;

			_size = value;

			if (Screen is null)
			{
				Coroutine.Run(
					CoroutineHelper.WaitForObject<Screen>(()
						=> Screen, (screen) => Emit(EngineSignals.ScreenDirty, screen))
				);
			}
			else
				Emit(EngineSignals.ScreenDirty, Screen);
		}
	}



	#region Helpers
	/// <summary>
	/// Gets the instance of the screen manager.
	/// </summary>
	protected ScreenManager ScreenManager => Engine.GetService<ScreenManager>();

	/// <summary>
	/// Gets the instance of the assets manager.
	/// </summary>
	protected Assets Assets => Engine.GetService<Assets>();

	/// <summary>
	/// Gets the instance of the game engine.
	/// </summary>
	protected Engine Engine => Engine.GetService<Engine>();

	/// <summary>
	/// Gets the input map from the game engine.
	/// </summary>
	protected InputMap Input => Engine.Input;

	/// <summary>
	/// Gets the instance of the game clock.
	/// </summary>
	protected Clock Clock => Engine.GetService<Clock>();

	/// <summary>
	/// Gets the instance of the signal manager.
	/// </summary>
	protected Signal Signal => Engine.GetService<Signal>();

	/// <summary>
	/// Gets the instance of the coroutine manager.
	/// </summary>
	protected Coroutine Coroutine => Engine.GetService<Coroutine>();

	/// <summary>
	/// Gets the instance of the renderer.
	/// </summary>
	protected Renderer Renderer => Engine.GetService<Renderer>();

	/// <summary>
	/// Gets the instance of the random number generator.
	/// </summary>
	protected FastRandom Rand => Engine.GetService<FastRandom>();

	/// <summary>
	/// Gets the instance of the sound manager.
	/// </summary>
	protected SoundManager SoundManager => Engine.GetService<SoundManager>();

	/// <summary>
	/// Gets the instance of the logging system.
	/// </summary>
	protected Log Log => Engine.GetService<Log>();

	/// <summary>
	/// Gets a singleton instance of a specified type.
	/// </summary>
	/// <typeparam name="T">The type of singleton to retrieve.</typeparam>
	/// <returns>The singleton instance of type T.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T GetService<T>() where T : GameService => Box.Engine.GetService<T>();

	/// <summary>
	/// Retrieves a Surface object by name from the Assets manager.
	/// </summary>
	/// <param name="name">The name of the Surface.</param>
	/// <returns>The Surface object associated with the name.</returns>
	protected Surface GetSurface(string name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Retrieves a Surface object by enum value from the Assets manager.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Surface.</param>
	/// <returns>The Surface object associated with the enum value.</returns>
	protected Surface GetSurface(Enum name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Retrieves a Surface object by filename from the Assets manager.
	/// </summary>
	/// <param name="filename">The filename of the Surface.</param>
	/// <returns>The Surface object loaded from the specified file.</returns>
	protected Surface GetSurfaceFromFile(string filename) => Assets.GetFromFile<Surface>(filename);

	/// <summary>
	/// Retrieves a Surface object from a MapTileset object.
	/// </summary>
	/// <param name="tileset">The MapTileset containing the filename of the Surface.</param>
	/// <returns>The Surface object loaded from the tileset's filename.</returns>
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
	/// Retrieves a Surface object from a MapTileset object from BoxPack.
	/// </summary>
	/// <param name="tileset">The MapTileset containing the filename of the Surface.</param>
	/// <returns>The Surface object loaded from the tileset's filename.</returns>
	protected Surface GetTilesetSurfaceFromPack(MapTileset tileset) => Assets.GetFromPack<Surface>(tileset.Filename);

	/// <summary>
	/// Retrieves a Map object by name from the Assets manager.
	/// </summary>
	/// <param name="name">The name of the Map.</param>
	/// <returns>The Map object associated with the name.</returns>
	protected Map GetMap(string name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Map object by enum value from the Assets manager.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Map.</param>
	/// <returns>The Map object associated with the enum value.</returns>
	protected Map GetMap(Enum name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Sound object by name from the Assets manager.
	/// </summary>
	/// <param name="name">The name of the Sound.</param>
	/// <returns>The Sound object associated with the name.</returns>
	protected Sound GetSound(string name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Sound object by enum value from the Assets manager.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Sound.</param>
	/// <returns>The Sound object associated with the enum value.</returns>
	protected Sound GetSound(Enum name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Font object by name from the Assets manager.
	/// </summary>
	/// <param name="name">The name of the Font.</param>
	/// <returns>The Font object associated with the name.</returns>
	protected BoxFont GetFont(string name) => Assets.Get<BoxFont>(name);

	/// <summary>
	/// Retrieves a Font object by enum value from the Assets manager.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Font.</param>
	/// <returns>The Font object associated with the enum value.</returns>
	protected BoxFont GetFont(Enum name) => Assets.Get<BoxFont>(name);

	/// <summary>
	/// Retrieves a Spritesheet object by name from the Assets manager.
	/// </summary>
	/// <param name="name">The name of the Spritesheet.</param>
	/// <returns>The Spritesheet object associated with the name.</returns>
	protected Spritesheet GetSheet(string name) => Assets.Get<Spritesheet>(name);

	/// <summary>
	/// Retrieves a Spritesheet object by enum value from the Assets manager.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Spritesheet.</param>
	/// <returns>The Spritesheet object associated with the enum value.</returns>
	protected Spritesheet GetSheet(Enum name) => Assets.Get<Spritesheet>(name);

	/// <summary>
	/// Gets the global position of the specified entity.
	/// </summary>
	/// <param name="entity">The entity to get the global position for.</param>
	/// <returns>The global position as a Vect2 (assuming Vect2 is a vector type).</returns>
	public Vect2 GetGlobalPosition(Entity entity)
	{
		if (entity.IsChild)
			return entity.Parent?.GlobalPosition + entity._position ?? entity._position;
		else
			return entity._position;
	}

	/// <summary>
	/// Calculates the global position of the current entity by combining its local position with the specified global position vector.
	/// </summary>
	/// <param name="position">The local position of the entity as a <see cref="Vect2"/>.</param>
	/// <returns>The global position of the entity as a <see cref="Vect2"/>.</returns>
	public Vect2 GetGlobalPosition(Vect2 position)
	{
		if (IsChild)
			return Parent?.GlobalPosition + position ?? position;
		else
			return position;
	}

	/// <summary>
	/// Gets the local position of the specified entity.
	/// </summary>
	/// <param name="entity">The entity to get the local position for.</param>
	/// <returns>The local position as a Vect2 (assuming Vect2 is a vector type).</returns>
	public Vect2 GetLocalPosition(Entity entity) => entity._position;

	/// <summary>
	/// Calculates the local position of the current entity relative to a specified global position vector.
	/// </summary>
	/// <param name="position">The global position of the entity as a <see cref="Vect2"/>.</param>
	public Vect2 GetLocalPosition(Vect2 position)
	{
		if (IsChild)
			return position - _position;
		else
			return position;
	}

	/// <summary>
	/// Checks if there is any parent of the specified type T.
	/// </summary>
	/// <typeparam name="T">The type of parent entity to check for.</typeparam>
	/// <param name="result">If found, returns the parent entity of type T.</param>
	/// <returns>True if a parent of type T was found, false otherwise.</returns>
	public bool AnyParentOfType<T>(out T result) where T : Entity
	{
		result = GetParents<T>().FirstOrDefault();

		return result != null;
	}


	/// <summary>
	/// Retrieves all parents of the specified type T for this entity.
	/// </summary>
	/// <typeparam name="T">The type of parent entities to retrieve.</typeparam>
	/// <returns>An enumerable collection of parent entities of type T.</returns>
	public IEnumerable<T> GetParents<T>() where T : Entity
	{
		Entity parent = Parent;

		while (parent != null)
		{
			if (parent is T typedParent)
				yield return typedParent;

			parent = parent.Parent;
		}
	}


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
	/// Loads a surface (texture) from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the texture.</param>
	/// <param name="repeat">Whether the texture should repeat. Default is false.</param>
	/// <param name="smooth">Whether to use smoothing on the texture. Default is false.</param>
	/// <returns>A <see cref="Surface"/> object representing the loaded texture.</returns>
	public Surface LoadSurface(string path, bool repeat = false, bool smooth = false)
		=> Assets.LoadSurface(path, repeat, smooth);

	/// <summary>
	/// Casts the child entities to the specified type.
	/// </summary>
	/// <typeparam name="T">The target type to cast the children to. Must be a subclass of <see cref="Entity"/>.</typeparam>
	/// <returns>An <see cref="IEnumerable{T}"/> containing the children cast to the specified type.</returns>
	/// <exception cref="InvalidCastException">
	/// Thrown if any child entity cannot be cast to the specified type <typeparamref name="T"/>.
	/// </exception>
	public IEnumerable<T> ChildrenAs<T>() where T : Entity => Children.Cast<T>();
	#endregion


	#region Engine
	internal void EngineOnExit()
	{
		if (IsExiting)
			return;

		IsExiting = true;

		ClearChildren();
		ClearSignals();
		ClearRoutines();
		ClearTimers();

		OnExit();
	}

	internal void EngineOnEnter()
	{
		OnEnter();
	}

	internal void EngineUpdate()
	{
		if (Engine.GetService<EngineSettings>().DebugDraw)
			// Renderer.Instance.DrawRectangleOutline(GlobalBounds, BoxColor.AllShades.Red);
			Renderer.DrawRectangleOutline(GlobalBounds.X, GlobalBounds.Y, GlobalBounds.Width,
				GlobalBounds.Height, 1f, Color.AllShades.Red);

		Update();
	}
	#endregion


	#region Update, OnEnter, OnExit
	/// <summary>
	/// Update method to be overridden by subclasses for per-frame update logic and draw logic.
	/// </summary>
	protected virtual void Update() { }

	/// <summary>
	/// Method to be overridden by subclasses for handling entry into a state or context.
	/// </summary>
	protected virtual void OnEnter() { }

	/// <summary>
	/// Method to be overridden by subclasses for handling exit from a state or context.
	/// </summary>
	protected virtual void OnExit() { }
	#endregion


	#region Children

	public void AddChild(Entity entity) => AddChild(entities: entity);

	/// <summary>
	/// Adds multiple child entities to this parent entity.
	/// </summary>
	/// <param name="entities">The array of child entities to add.</param>
	public unsafe void AddChild(params Entity[] entities)
	{
		if (entities.IsEmpty())
			return;

		Action<Entity[]> execute = (Entity[] rEntities) =>
		{
			fixed (Entity* ptr = rEntities)
			{
				// Cant reverse this, keep as is:
				for (int i = 0; i < rEntities.Length; i++)
				{
					var entity = ptr + i;

					if (*entity is null)
						continue;
					if (entity->IsExiting)
						continue;
					if (Screen.HasEntity(*entity))
						continue;

					entity->Parent = this;
					entity->Screen = Screen;

					_children.Add(*entity);

					Screen.AddEntity(*entity);
				}
			}

			_isDirty = true;
		};

		IEnumerator routine(Entity[] iEntity)
		{
			if (Screen is null)
			{
				while (Screen is null)
					yield return null;
			}

			execute?.Invoke(iEntity);
		}

		StartRoutine(routine(entities));
	}

	/// <summary>
	/// Adds multiple child entities of type T to this parent entity.
	/// </summary>
	/// <typeparam name="T">The type of child entities to add.</typeparam>
	/// <param name="entities">The array of child entities to add.</param>
	/// <returns>The first child entity of type T added.</returns>
	public T AddChild<T>(params Entity[] entities) where T : Entity
	{
		if (entities.IsEmpty())
			return (T)this;

		for (int i = entities.Length - 1; i >= 0; i--)
			entities[i].Layer++;

		AddChild(entities);

		return (T)this;
	}

	/// <summary>
	/// Inserts a child entity at the specified index in this parent entity.
	/// </summary>
	/// <param name="index">The index at which to insert the child entity.</param>
	/// <param name="entity">The child entity to insert.</param>
	public void InsertChild(int index, Entity entity)
	{
		if (entity is null)
			return;
		if (entity.IsExiting)
			return;

		int clampIndex = Math.Clamp(index, 0, ChildCount);

		entity.Parent = this;
		entity.Screen = Screen;

		_children.Insert(clampIndex, entity);

		Screen.AddEntity(entity);
	}

	/// <summary>
	/// Retrieves the child entity of type T at the specified index.
	/// </summary>
	/// <typeparam name="T">The type of child entity to retrieve.</typeparam>
	/// <param name="index">The index of the child entity to retrieve.</param>
	/// <returns>The child entity of type T at the specified index, or null if not found.</returns>
	public T GetChild<T>(int index) where T : Entity
	{
		if (!Children.Any())
			return default;

		return (T)Children.ElementAtOrDefault(index);
	}

	/// <summary>
	/// Checks if the parent entity has any child of type T.
	/// </summary>
	/// <typeparam name="T">The type of child entity to check for.</typeparam>
	/// <returns>True if the parent entity has a child of type T, false otherwise.</returns>
	public bool HasChild<T>() where T : Entity
	{
		if (!Children.Any())
			return false;

		return Children.Any(x => x is T);
	}

	/// <summary>
	/// Checks if the parent entity contains the specified child entity.
	/// </summary>
	/// <param name="entity">The child entity to check for.</param>
	/// <returns>True if the parent entity contains the specified child entity, false otherwise.</returns>
	public bool HasChild(Entity entity)
	{
		if (!Children.Any())
			return false;
		if (entity is null)
			return false;

		return Children.Any(x => x == entity);
	}

	/// <summary>
	/// Removes a specific child entity from the parent entity.
	/// </summary>
	/// <param name="entity">The child entity to remove.</param>
	/// <returns>True if the child entity was successfully removed; false if the child entity was not found.</returns>
	public bool RemoveChild(Entity entity)
	{
		if (entity is null)
			return false;
		if (entity.IsExiting)
			return false;

		if (entity.Children.Any())
		{
			foreach (var child in entity.Children)
				Screen.RemoveEntity(child);
		}

		// remove all referenced children from all paths:
		_children.Remove(entity);
		entity._children.Remove(entity);
		entity.Parent?._children.Remove(entity);
		Parent?._children.Remove(entity);

		return Screen.RemoveEntity(entity);
	}

	/// <summary>
	/// Removes multiple child entities from the parent entity.
	/// </summary>
	/// <param name="entities">The array of child entities to remove.</param>
	/// <returns>True if all specified child entities were successfully removed; false if any of the child entities were not found.</returns>
	public bool RemoveChild(params Entity[] entities)
	{
		if (!Children.Any())
			return false;

		var check = new bool[entities.Length];
		var result = new bool[entities.Length];

		for (int i = 0; i < entities.Length; i++)
		{
			check[i] = true;
			result[i] = RemoveChild(entities[i]);
		}

		return result.SequenceEqual(check);
	}

	/// <summary>
	/// Clears all child entities from the parent entity.
	/// </summary>
	public void ClearChildren()
	{
		if (!Children.Any())
			return;

		var children = _children.ToArray();

		unsafe
		{
			fixed (Entity* ptr = children)
			{
				for (int i = children.Length - 1; i >= 0; i--)
					RemoveChild(*(ptr + i));
			}
		}
	}

	/// <summary>
	/// Destroys the parent entity and its children, cleaning up resources.
	/// </summary>
	public void Destroy()
	{
		if (IsExiting)
			return;

		RemoveChild(this);
	}
	#endregion


	#region Coroutines
	/// <summary>
	/// Starts a coroutine with a delay.
	/// </summary>
	/// <param name="delay">The delay in seconds before starting the coroutine.</param>
	/// <param name="routine">The IEnumerator coroutine routine to start.</param>
	/// <returns>A handle to the coroutine, which can be used for further management.</returns>
	public CoroutineHandle StartRoutineDelayed(float delay, IEnumerator routine)
	{
		if (IsExiting)
			return default;

		var handle = Coroutine.RunDelayed(delay, routine);

		_coroutines.Add(handle);

		return handle;
	}

	/// <summary>
	/// Starts a coroutine immediately.
	/// </summary>
	/// <param name="routine">The IEnumerator coroutine routine to start.</param>
	/// <returns>A handle to the coroutine, which can be used for further management.</returns>
	public CoroutineHandle StartRoutine(IEnumerator routine) => StartRoutineDelayed(0f, routine);

	/// <summary>
	/// Checks if a coroutine with the specified handle is currently running.
	/// </summary>
	/// <param name="handle">The handle of the coroutine to check.</param>
	/// <returns>True if the coroutine with the specified handle is running; otherwise, false.</returns>
	public bool HasRoutine(CoroutineHandle handle) => _coroutines.Any(x => x == handle);

	/// <summary>
	/// Checks if a coroutine with the specified IEnumerator is currently running.
	/// </summary>
	/// <param name="enumerator">The IEnumerator coroutine to check.</param>
	/// <returns>True if the coroutine with the specified IEnumerator is running; otherwise, false.</returns>
	public bool HasRoutine(IEnumerator enumerator) => _coroutines.Any(x => x.Enumerator == enumerator);

	/// <summary>
	/// Stops a coroutine with the specified handle.
	/// </summary>
	/// <param name="handle">The handle of the coroutine to stop.</param>
	/// <returns>True if the coroutine was successfully stopped; otherwise, false.</returns>
	public bool StopRoutine(CoroutineHandle handle) => StopRoutine(handle.Enumerator);

	/// <summary>
	/// Stops a coroutine with the specified IEnumerator.
	/// </summary>
	/// <param name="handle">The IEnumerator of the coroutine to stop.</param>
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
	/// Clears all running coroutines.
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
	/// Connects a handler function to a signal identified by name.
	/// </summary>
	/// <param name="name">The name of the signal to connect to.</param>
	/// <param name="handle">The action (handler) to be called when the signal is emitted.</param>
	public void Connect(string name, Action<SignalHandle> handle)
	{
		if (!_signals.TryGetValue(name, out var value))
			_signals.Add(name, new List<Action<SignalHandle>> { handle });
		else
			value.Add(handle);

		Signal.Connect(name, handle);
	}

	/// <summary>
	/// Connects a handler function to a signal identified by an enum value.
	/// </summary>
	/// <param name="name">The enum value representing the signal to connect to.</param>
	/// <param name="handle">The action (handler) to be called when the signal is emitted.</param>
	public void Connect(Enum name, Action<SignalHandle> handle) => Connect(name.ToEnumString(), handle);

	/// <summary>
	/// Emits (triggers) a signal identified by name, passing optional data to its handlers.
	/// </summary>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass to the signal handlers.</param>
	public void Emit(string name, params object[] data) => Signal.Emit(name, data);

	/// <summary>
	/// Emits (triggers) a signal identified by an enum value, passing optional data to its handlers.
	/// </summary>
	/// <param name="name">The enum value representing the signal to emit.</param>
	/// <param name="data">Optional data to pass to the signal handlers.</param>
	public void Emit(Enum name, params object[] data) => Signal.Emit(name, data);

	/// <summary>
	/// Emits (triggers) a signal with a delay, identified by its name, passing optional data to its handlers.
	/// </summary>
	/// <param name="delay">The delay in seconds before emitting the signal.</param>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass to the signal handlers.</param>
	public void EmitDelayed(float delay, string name, params object[] data)
		=> Signal.EmitDelayed(delay, name, data);

	/// <summary>
	/// Emits (triggers) a signal with a delay, identified by an enum value, passing optional data to its handlers.
	/// </summary>
	/// <param name="delay">The delay in seconds before emitting the signal.</param>
	/// <param name="name">The enum value representing the signal to emit.</param>
	/// <param name="data">Optional data to pass to the signal handlers.</param>
	public void EmitDelayed(float delay, Enum name, params object[] data)
		=> Signal.EmitDelayed(delay, name, data);

	/// <summary>
	/// Checks if a signal with the specified name has any connected handlers.
	/// </summary>
	/// <param name="name">The name of the signal to check.</param>
	/// <returns>True if there are handlers connected to the signal; otherwise, false.</returns>
	public bool HasSignal(string name) => _signals.ContainsKey(name);

	/// <summary>
	/// Checks if a signal identified by an enum value has any connected handlers.
	/// </summary>
	/// <param name="name">The enum value representing the signal to check.</param>
	/// <returns>True if there are handlers connected to the signal; otherwise, false.</returns>
	public bool HasSignal(Enum name) => HasSignal(name.ToEnumString());

	/// <summary>
	/// Disconnects (removes) all handlers connected to a signal identified by its name.
	/// </summary>
	/// <param name="name">The name of the signal to disconnect from.</param>
	/// <returns>True if handlers were disconnected successfully; otherwise, false.</returns>
	public bool Disconnect(string name)
	{
		if (_signals.TryGetValue(name, out var signals))
		{
			foreach (Action<SignalHandle> signal in signals)
				Signal.Disconnect(name, signal);

			_signals.Remove(name);

			return true;
		}

		return false;
	}

	/// <summary>
	/// Disconnects (removes) all handlers connected to a signal identified by an enum value.
	/// </summary>
	/// <param name="name">An enum value representing the signal to disconnect from.</param>
	/// <returns>True if handlers were disconnected successfully; otherwise, false.</returns>
	public bool Disconnect(Enum name) => Disconnect(name.ToEnumString());

	/// <summary>
	/// Clears (removes) all signals and their connected handlers.
	/// </summary>
	public void ClearSignals()
	{
		if (_signals.Count == 0)
			return;

		foreach (var signal in new Dictionary<string, List<Action<SignalHandle>>>(_signals))
		{
			foreach (Action<SignalHandle> action in signal.Value)
				Signal.Disconnect(signal.Key, action);

			_signals.Remove(signal.Key);
		}
	}
	#endregion


	#region Timers
	/// <summary>
	/// Adds a timer with a specified name, delay, repeat flag, and action to execute.
	/// </summary>
	/// <param name="name">The name of the timer.</param>
	/// <param name="delay">The delay in seconds before the timer action is executed.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer expires.</param>
	public void StartTimer(string name, float delay, bool repeat, Action action)
		=> _timers.Add(name, delay, repeat, action);

	/// <summary>
	/// Adds a timer with a specified enum name, delay, repeat flag, and action to execute.
	/// </summary>
	/// <param name="name">An enum value representing the name of the timer.</param>
	/// <param name="delay">The delay in seconds before the timer action is executed.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer expires.</param>
	public void StartTimer(Enum name, float delay, bool repeat, Action action)
		=> _timers.Add(name, delay, repeat, action);

	/// <summary>
	/// Adds a timer with an automatically generated name, specified delay, repeat flag, and action to execute.
	/// </summary>
	/// <param name="delay">The delay in seconds before the timer action is executed.</param>
	/// <param name="repeat">True if the timer should repeat; false otherwise.</param>
	/// <param name="action">The action to execute when the timer expires.</param>
	public void StartTimer(float delay, bool repeat, Action action) => _timers.Add(delay, repeat, action);

	/// <summary>
	/// Checks if a timer with the specified name exists.
	/// </summary>
	/// <param name="name">The name of the timer to check.</param>
	/// <returns>True if a timer with the specified name exists; false otherwise.</returns>
	public bool TimerExists(string name) => _timers.Exists(name);

	/// <summary>
	/// Checks if a timer identified by an enum value exists.
	/// </summary>
	/// <param name="name">An enum value representing the name of the timer to check.</param>
	/// <returns>True if a timer with the specified enum name exists; otherwise, false.</returns>

	public bool TimerExists(Enum name) => _timers.Exists(name);

	/// <summary>
	/// Stops (removes) a timer identified by its name.
	/// </summary>
	/// <param name="name">The name of the timer to stop.</param>
	/// <returns>True if the timer was successfully stopped; otherwise, false.</returns>
	public bool StopTimer(string name) => _timers.Stop(name);

	/// <summary>
	/// Stops (removes) a timer identified by an enum value.
	/// </summary>
	/// <param name="name">An enum value representing the name of the timer to stop.</param>
	/// <returns>True if the timer was successfully stopped; otherwise, false.</returns>
	public bool StopTimer(Enum name) => _timers.Stop(name);

	/// <summary>
	/// Clears (removes) all timers associated with the entity.
	/// </summary>
	public void ClearTimers() => _timers.Clear();
	#endregion






	public T SetPosition<T>(Vect2 position) where T : Entity
	{
		Position = position;

		return (T)this;
	}
	public T SetPosition<T>(float x, float y) where T : Entity
	{
		Position = new Vect2(x, y); ;

		return (T)this;
	}
	public T SetSize<T>(Vect2 size) where T : Entity
	{
		Size = size;

		return (T)this;
	}
}
