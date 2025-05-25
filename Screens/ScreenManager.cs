using Box.Graphics.Batch;
using Box.Services.Types;

namespace Box.Screens;

/// <summary>
/// Manages the lifecycle and navigation of screens in the application.
/// </summary>
public sealed class ScreenManager : UpdatableService
{
	internal int TotalEntities, TotalTimers/*, _totalRoutines*/;

	private readonly TimeManager _timers = new();
	private readonly List<Screen> _screens = new();
	private IEnumerable<Screen> _orderedScreens;
	private bool _isDirty = true;

	// /// <summary>
	// /// Gets the singleton instance of the ScreenManager.
	// /// </summary>
	// public static ScreenManager Instance { get; private set; }

	/// <summary>
	/// Gets the number of screens currently managed by the ScreenManager.
	/// </summary>
	public int Count => _screens.Count;

	/// <summary>
	/// Gets or sets the active screen currently being displayed.
	/// </summary>
	public Screen ActiveScreen { get; private set; }

	/// <summary>
	/// Gets an enumerable collection of all screens managed by the ScreenManager.
	/// </summary>
	public IEnumerable<Screen> Screens
	{
		get
		{
			if (_screens.Count == 0)
				yield break;

			foreach (Screen screen in _screens.ToList())
			{
				if (screen is null || screen.IsExiting)
					continue;

				yield return screen;
			}
		}
	}


	#region Engine
	internal ScreenManager() {} /*=> Instance ??= this;*/

	public override void Update()
	{
		if (_isDirty)
		{
			_orderedScreens = _screens
				.Where(x => x is not null && !x.IsExiting)
				.OrderBy(x => x.Layer)
				;

			_isDirty = false;
		}

		if (!_isDirty && _orderedScreens is not null)
		{
			TotalEntities = 0;
			TotalTimers = 0;

			bool isActive = GetService<Engine>().IsActive;
			ActiveScreen = _orderedScreens
				.Reverse()
				.FirstOrDefault(x => !x.IsUiScreen);

			foreach (var screen in _orderedScreens)
			{
				TotalEntities += screen.Entities.Count();
				TotalTimers += screen.Timers.Count;

				screen.IsActive = isActive;
				screen.IsTopmostScreen = screen == ActiveScreen;

				Renderer.Instance.Begin(screen);

				screen.EngineUpdate();

				Renderer.Instance.End();
			}
		}

		base.Update();
	}
	#endregion


	#region Add
	/// <summary>
	/// Adds a single screen to the ScreenManager.
	/// </summary>
	/// <param name="screen">The screen to add.</param>
	public void Add(Screen screen) => Add(screens: screen);

	/// <summary>
	/// Adds multiple screens to the ScreenManager.
	/// </summary>
	/// <param name="screens">The screens to add.</param>
	public unsafe void Add(params Screen[] screens)
	{
		if (screens.IsEmpty())
			return;

		fixed (Screen* ptr = screens)
		{
			for (int i = 0; i < screens.Length; i++)
			{
				var screen = ptr + i;

				if (*screen is null)
					continue;
				if (screen->IsExiting)
					continue;

				screen->Camera = new Camera(*screen);

				screen->EngineOnEnter();

				_screens.Add(*screen);
			}
		}

		_isDirty = true;
	}
	#endregion


	#region Remove

	/// <summary>
	/// Removes a specific screen from the ScreenManager.
	/// </summary>
	/// <param name="screen">The screen to remove.</param>
	/// <returns>True if the screen was successfully removed; otherwise, false.</returns>
	public bool Remove(Screen screen) => Remove(screens: screen);

	/// <summary>
	/// Removes multiple screens from the ScreenManager.
	/// </summary>
	/// <param name="screens">The screens to remove.</param>
	/// <returns>True if all screens were successfully removed; otherwise, false.</returns>
	public bool Remove(params Screen[] screens)
	{
		if (screens.Length == 0)
			return false;

		bool[] check = new bool[screens.Length];
		bool[] result = new bool[screens.Length];

		for (int i = 0; i < screens.Length; i++)
		{
			check[i] = true;

			if (screens[i] is null || screens[i].IsExiting)
				result[i] = false;
			else
				result[i] = _screens.Remove(screens[i]);

			if (result[i])
				screens[i].EngineOnExit();
		}

		_isDirty = true;

		return result.SequenceEqual(check);
	}

	#endregion


	#region Clear

	/// <summary>
	/// Clears all screens from the ScreenManager.
	/// </summary>
	public void Clear()
	{
		if (_screens.Count == 0)
			return;

		for (int i = 0; i < _screens.Count; i++)
			Remove(_screens[i]);
	}

	#endregion


	#region Get

	/// <summary>
	/// Retrieves a screen of the specified type <typeparamref name="T"/> from the ScreenManager.
	/// </summary>
	/// <typeparam name="T">The type of screen to retrieve.</typeparam>
	/// <returns>The screen of the specified type, if found; otherwise, throws an exception.</returns>
	public T Get<T>() where T : Screen => (T)Screens.FirstOrDefault(x => x is T);

	/// <summary>
	/// Attempts to retrieve a screen of the specified type <typeparamref name="T"/> from the ScreenManager.
	/// </summary>
	/// <typeparam name="T">The type of screen to retrieve.</typeparam>
	/// <param name="screen">When this method returns, contains the screen of the specified type if it is found; otherwise, default(T).</param>
	/// <returns>True if the screen was successfully retrieved; otherwise, false.</returns>
	public bool TryGet<T>(out T screen) where T : Screen
	{
		screen = Get<T>();

		return screen is not null;
	}

	#endregion


	#region Has

	/// <summary>
	/// Checks if a screen of the specified type <typeparamref name="T"/> exists in the ScreenManager.
	/// </summary>
	/// <typeparam name="T">The type of screen to check for.</typeparam>
	/// <returns>True if a screen of the specified type exists; otherwise, false.</returns>
	public bool Has<T>() where T : Screen => Get<T>() is not null;

	/// <summary>
	/// Checks if the specified screen exists in the ScreenManager.
	/// </summary>
	/// <param name="screen">The screen to check for.</param>
	/// <returns>True if the screen exists in the ScreenManager; otherwise, false.</returns>
	public bool Has(Screen screen) => Screens.Contains(screen);

	#endregion
}
