using Box.Services.Types;

namespace Box;

/// <summary>
/// Engine settings used to configure window size, viewport size, vsync, culling, and other options.
/// </summary>
public class EngineSettings : GameService
{
	/// <summary>
	/// Title of the application to be displayed on the game window.
	/// <para>Default value: Game</para>
	/// </summary>
	public string AppTitle { get; internal set; } = "Game";

	/// <summary>
	/// Sets the application title for the engine.
	/// </summary>
	/// <param name="value">The title to be set for the application window.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the title of the application window. It returns the current instance of the 
	/// <see cref="EngineSettings"/> class to support a fluent interface, enabling multiple settings to be configured in a chain.
	/// </remarks>
	public EngineSettings WithAppTitle(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Application title cannot be empty or null.", nameof(value));

		AppTitle = value;

		return this;
	}


	/// <summary>
	/// Folder or name used for ApplicationData.
	/// <para>Default value: Game</para>
	/// </summary>
	public string AppName { get; internal set; } = "Game";

	/// <summary>
	/// Sets the application name for the engine.
	/// </summary>
	/// <param name="value">The name to be set for the application.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the name of the application. It returns the current instance of the 
	/// <see cref="EngineSettings"/> class, enabling multiple settings to be configured in a fluent, chained manner.
	/// </remarks>
	public EngineSettings WithAppName(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Application name cannot be empty or null.", nameof(value));

		AppName = value;

		return this;
	}


	/// <summary>
	/// Root folder for game content used for loading assets.
	/// <para>Default value: Content</para>
	/// </summary>
	public string AppContentRoot { get; internal set; } = "Content";

	/// <summary>
	/// Sets the root directory for the application's content files.
	/// </summary>
	/// <param name="value">The directory path for the application's content root.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the root directory for the application’s content, such as textures, models,
	/// and other assets. It returns the current instance of the <see cref="EngineSettings"/> class, enabling
	/// fluent configuration of engine settings.
	/// </remarks>
	public EngineSettings WithAppContentRoot(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Content root directory cannot be empty or null.", nameof(value));

		AppContentRoot = value;

		return this;
	}


	/// <summary>
	/// Main folder for storing and accessing game saves.
	/// <para>Default value: Save</para>
	/// </summary>
	public string AppSaveRoot { get; internal set; } = "Save";

	/// <summary>
	/// Sets the root directory for the application's save data.
	/// </summary>
	/// <param name="value">The directory path for the application's save data root.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the root directory where the application’s save data will be stored.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithAppSaveRoot(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Save data root directory cannot be empty or null.", nameof(value));

		AppSaveRoot = value;

		return this;
	}


	/// <summary>
	/// Main folder for storing and accessing game logs.
	/// <para>Default value: Logs</para>
	/// </summary>
	public string AppLogRoot { get; internal set; } = "Logs";

	/// <summary>
	/// Sets the root directory for the application's log files.
	/// </summary>
	/// <param name="value">The directory path for the application's log files root.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the root directory where the application’s log files will be stored.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithAppLogRoot(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Log root directory cannot be empty or null.", nameof(value));

		AppLogRoot = value;

		return this;
	}


	/// <summary>
	/// File used for storing game settings.
	/// <para>Default value: settings.json</para>
	/// </summary>
	public string AppSettings { get; internal set; } = "settings.json";

	/// <summary>
	/// Sets the file name for storing the application's settings.
	/// </summary>
	/// <param name="value">The file name for storing the application's settings.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the file name where the application's settings will be stored. 
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithAppSettings(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("Settings file name cannot be empty or null.", nameof(value));

		AppSettings = value;

		return this;
	}


	/// <summary>
	/// Version of the game application, also converted to a hash within the Engine.
	/// <para>Default value: 1.0</para>
	/// </summary>
	public string AppVersion { get; internal set; } = "1.0";

	/// <summary>
	/// Sets the version of the application.
	/// </summary>
	/// <param name="value">The version string to be set for the application.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the <paramref name="value"/> is empty or null.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the version of the application. It returns the current instance of the 
	/// <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithAppVersion(string value)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException("App version cannot be empty or null.", nameof(value));

		AppVersion = value;

		return this;
	}


	/// <summary>
	/// Specifies whether to use Mac bundles for asset loading instead of custom configurations.
	/// <para>Default value: false</para>
	/// </summary>
	public bool IsMacBundle { get; internal set; } = false;

	/// <summary>
	/// Sets whether to use Mac bundles for asset loading instead of custom configurations.
	/// </summary>
	/// <param name="value">A boolean indicating whether to use Mac bundles (true) or custom configurations (false).</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the engine should load assets using Mac bundles (true) or rely on 
	/// custom configurations (false). It returns the current instance of the <see cref="EngineSettings"/> class, enabling
	/// fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithIsMacBundle(bool value)
	{
		IsMacBundle = value;

		return this;
	}


	/// <summary>
	/// Determines whether to use the current application folder or OS application data for saving game data and log files.
	/// <para>Default value: false</para>
	/// </summary>
	public bool UseApplicationData { get; internal set; } = false;

	/// <summary>
	/// Sets whether to use the current application folder or OS application data for saving game data and log files.
	/// </summary>
	/// <param name="value">A boolean indicating whether to use application data (true) or the current application folder (false).</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the engine should use the OS's application data folder (true) 
	/// or the current application folder (false) for saving game data and log files.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithUseApplicationData(bool value)
	{
		UseApplicationData = value;

		return this;
	}


	/// <summary>
	/// Automatically switch the game to full screen mode on engine startup.
	/// <para>Default value: false</para>
	/// </summary>
	public bool Fullscreen { get; internal set; } = false;

	/// <summary>
	/// Sets whether to automatically switch the game to full-screen mode on engine startup.
	/// </summary>
	/// <param name="value">A boolean indicating whether the game should start in full-screen mode (true) or windowed mode (false).</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the game should automatically switch to full-screen mode upon startup.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithFullscreen(bool value)
	{
		Fullscreen = value;

		return this;
	}


	/// <summary>
	/// Used for pixelated or tile-based games to fix bleeding gaps in textures.
	/// <para>Default value: false</para>
	/// </summary>
	public bool UseTextureHalfOffset { get; internal set; } = false;

	/// <summary>
	/// Sets whether to use a texture half offset to fix bleeding gaps in textures, typically used for pixelated or tile-based games.
	/// </summary>
	/// <param name="value">A boolean indicating whether to use a texture half offset (true) to prevent texture gaps, or not (false).</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method helps prevent **texture bleeding** issues in pixelated or tile-based games by applying a half offset to textures.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithUseTextureHalfOffset(bool value)
	{
		UseTextureHalfOffset = value;

		return this;
	}


	/// <summary>
	/// The clear color when the engine clears the screen.
	/// <para>Default value: CornFlowerBlue</para>
	/// </summary>
	public BoxColor ClearColor { get; internal set; } = BoxColor.AllShades.CornFlowerBlue;


	/// <summary>
	/// Sets the clear color used when the engine clears the screen.
	/// </summary>
	/// <param name="value">The <see cref="BoxColor"/> to be used as the clear color for the engine's screen clearing process.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to set the clear color for the engine's screen clearing process, typically used as the background color
	/// when the screen is refreshed each frame. It returns the current instance of the <see cref="EngineSettings"/> class, enabling
	/// fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithClearColor(BoxColor value)
	{
		ClearColor = value;

		return this;
	}


	/// <summary>
	/// Represents the size of the game window.
	/// <para>Default value: 1280x720</para>
	/// </summary>
	public Vect2 Window { get; internal set; } = new(1280, 720);

	/// <summary>
	/// Sets the window size for the application.
	/// </summary>
	/// <param name="width">The width of the application window.</param>
	/// <param name="height">The height of the application window.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when either <paramref name="width"/> or <paramref name="height"/> is less than zero.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the size of the application window. It ensures that the provided width and height
	/// are non-negative, and returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithWindow(int width, int height)
	{
		if (width < 0)
			throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than or equal to zero.");
		if (height < 0)
			throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than or equal to zero.");

		Window = new Vect2(width, height);

		return this;
	}


	/// <summary>
	/// Represents the game window's viewport size. Adjusts rendering quality based on window size relative to viewport.
	/// <para>Default value: 320x180</para>
	/// </summary>
	public Vect2 Viewport { get; internal set; } = new(320, 180);

	/// <summary>
	/// Sets the viewport size for the application, which determines how content is rendered on the screen.
	/// </summary>
	/// <param name="width">The width of the viewport.</param>
	/// <param name="height">The height of the viewport.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when either <paramref name="width"/> or <paramref name="height"/> is less than zero.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the size of the application's viewport. The viewport defines the rendering area
	/// for the game’s content. It returns the current instance of the <see cref="EngineSettings"/> class, enabling
	/// fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithViewport(int width, int height)
	{
		if (width < 0)
			throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than or equal to zero.");
		if (height < 0)
			throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than or equal to zero.");

		Viewport = new Vect2(width, height);

		return this;
	}


	/// <summary>
	/// Specifies the screen to load when the engine starts.
	/// <para>Default value: null</para>
	/// </summary>
	public Screen[] Screens { get; internal set; }
	public EngineSettings WithScreens(params Screen[] values)
	{
		if (values.Length == 0)
			throw new Exception();

		Screens = new Screen[values.Length];

		for (int i = 0; i < values.Length; i++)
			Screens[i] = values[i];

		return this;
	}

	/// <summary>
	/// Maximum allowed draw calls to be drawn. Going too low may cut off drawn textures.
	/// <para>Default value: 128</para>
	/// </summary>
	public int MaxDrawCalls { get; internal set; } = 256;

	/// <summary>
	/// Sets the maximum number of draw calls allowed in the engine.
	/// </summary>
	/// <param name="value">The maximum number of draw calls.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when the <paramref name="value"/> is less than 1.
	/// </exception>
	/// <remarks>
	/// This method allows you to configure the maximum number of draw calls that the engine can perform each frame. 
	/// Setting a value that is too low may cause performance issues or cut off drawn textures.
	/// It returns the current instance of the <see cref="EngineSettings"/> class, enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithMaxDrawCalls(int value)
	{
		if (value < 1)
			throw new ArgumentOutOfRangeException(nameof(value), "Max draw calls must be greater than or equal to 1.");

		MaxDrawCalls = value;

		return this;
	}


	/// <summary>
	/// Determines whether to display the shapes of all added entities.
	/// Useful for entities without visual surfaces or alignment panels.
	/// <para>Note: Enabling this may reduce framerate due to drawing overhead.</para>
	/// <para>Default value: false</para>
	/// </summary>
	public bool DebugDraw { get; internal set; } = false;

	/// <summary>
	/// Sets whether to display the shapes of all added entities for debugging purposes.
	/// </summary>
	/// <param name="value">A boolean indicating whether to enable or disable the display of entity shapes (true to enable, false to disable).</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to enable or disable the display of shapes for all added entities. This is useful for debugging,
	/// particularly for entities that do not have visual surfaces or alignment panels. 
	/// <para>Note: Enabling this feature may reduce the framerate due to the additional drawing overhead.</para>
	/// </remarks>
	public EngineSettings WithDebugDraw(bool value)
	{
		DebugDraw = value;

		return this;
	}

	/// <summary>
	/// The level of Antialiasing used. The maximum level depends on your graphics card capabilities.
	/// <para>Default value: 0</para>
	/// </summary>
	public int AntialiasingLevel { get; internal set; } = 0;

	/// <summary>
	/// Sets the level of antialiasing to be used by the engine.
	/// </summary>
	/// <param name="value">The antialiasing level to be set. The valid range is from 0 (no antialiasing) to the maximum supported by the graphics hardware.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when the <paramref name="value"/> is less than 0.
	/// </exception>
	/// <remarks>
	/// This method allows you to configure the level of antialiasing used by the engine to smooth out jagged edges in rendered graphics.
	/// Setting a higher value results in better visual quality but may impact performance. The valid range depends on the graphics card capabilities.
	/// </remarks>
	public EngineSettings WithAntialiasingLevel(int value)
	{
		if (value < 0)
			throw new ArgumentOutOfRangeException(nameof(value), "Antialiasing level cannot be negative.");

		AntialiasingLevel = value;

		return this;
	}

	/// <summary>
	/// Enables or disables Vertical Sync (Vsync). When Vsync is disabled, coroutines may require delta time adjustments to synchronize movements or other processes.
	/// <para>Default value: true</para>
	/// </summary>
	public bool VSync { get; internal set; } = true;

	/// <summary>
	/// Sets whether to enable or disable vertical synchronization (V-Sync) for the engine.
	/// </summary>
	/// <param name="value">A boolean indicating whether to enable (true) or disable (false) V-Sync.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the engine should synchronize its frame rate with the monitor’s refresh rate.
	/// Enabling V-Sync can help reduce screen tearing, but may also introduce input lag or affect performance.
	/// </remarks>
	public EngineSettings WithVSync(bool value)
	{
		VSync = value;

		return this;
	}


	/// <summary>
	/// Controls whether the mouse cursor is displayed within the viewport.
	/// <para>Default value: true</para>
	/// </summary>
	public bool Mouse { get; internal set; } = true;

	/// <summary>
	/// Sets whether the mouse cursor should be displayed within the viewport.
	/// </summary>
	/// <param name="value">A boolean indicating whether to display the mouse cursor (true) or hide it (false) within the viewport.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the mouse cursor should be visible within the application’s viewport.
	/// Disabling the mouse cursor is useful in certain games or applications where the mouse is not required, such as first-person shooters or custom input systems.
	/// </remarks>
	public EngineSettings WithMouse(bool value)
	{
		Mouse = value;

		return this;
	}


	/// <summary>
	/// Sets a cull size around the camera viewing area to prevent large entities from instantly appearing within the viewport.
	/// <para>Default value: 16</para>
	/// </summary>
	public int CullSize { get; internal set; } = 16;

	/// <summary>
	/// Sets the culling size around the camera viewing area to prevent large entities from instantly appearing in the viewport.
	/// </summary>
	/// <param name="value">The size of the cull area to be applied around the camera’s viewing area.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when the <paramref name="value"/> is less than zero.
	/// </exception>
	/// <remarks>
	/// This method allows you to set the culling size around the camera’s viewing area. Culling is useful to prevent large
	/// or off-screen entities from being rendered instantly as they enter the viewport, which can help improve performance.
	/// </remarks>
	public EngineSettings WithCullSize(int value)
	{
		if (value < 0)
			throw new ArgumentOutOfRangeException(nameof(value), "Cull size cannot be negative.");

		CullSize = value;

		return this;
	}


	/// <summary>
	/// Sets the default gap for user interface (UI) entities. Can also be used with Renderer alignments.
	/// <para>Default value: 8</para>
	/// </summary>
	public int SafeRegion { get; internal set; } = 8;

	/// <summary>
	/// Sets the default gap for user interface (UI) entities to ensure they stay within a safe region.
	/// </summary>
	/// <param name="value">The gap size (in pixels) around the UI elements to maintain a safe region.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when the <paramref name="value"/> is less than zero.
	/// </exception>
	/// <remarks>
	/// This method sets a gap around the user interface elements, preventing UI elements from overlapping the screen edges
	/// or from being obscured by the edges of the window. It returns the current instance of the <see cref="EngineSettings"/> class,
	/// enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithSafeRegion(int value)
	{
		if (value < 0)
			throw new ArgumentOutOfRangeException(nameof(value), "Safe region value cannot be negative.");

		SafeRegion = value;

		return this;
	}


	/// <summary>
	/// Defines the dead zone for gamepad thumbsticks and trigger buttons. 
	/// <para>Note: Setting it too low may trigger unintended inputs, while setting it too high may fail to detect inputs.</para>
	/// <para>Valid range: 0.0 to 1.0.</para>
	/// <para>Default value: 0.2</para>
	/// </summary>
	public float GamepadDeadzone { get; internal set; } = 0.2f;

	/// <summary>
	/// Sets the dead zone for the gamepad's thumbsticks and trigger buttons.
	/// </summary>
	/// <param name="value">The dead zone value between 0.0 and 1.0. A value closer to 0.0 will make the thumbsticks and triggers more sensitive, while a value closer to 1.0 will make them less sensitive.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure the sensitivity threshold for gamepad input. It ensures that small movements of the thumbsticks
	/// or trigger buttons don't result in unintended actions, by setting a dead zone where input below the threshold is ignored.
	/// The value is clamped between 0.0 and 1.0 to prevent invalid input values.
	/// </remarks>
	public EngineSettings WithGamepadDeadzone(float value)
	{
		GamepadDeadzone = Math.Clamp(value, 0f, 1.0f);

		return this;
	}


	/// <summary>
	/// Specifies the input used for the engine.
	/// <para>Note: Multiple inputs can be defined, but only one active input can be used at a time.</para>
	/// <para>Default value: DefaultInputMap</para>
	/// </summary>
	public InputMap InputMap { get; internal set; } = new DefaultInputMap();
	public EngineSettings WithInputMap(InputMap value)
	{
		if (value == null)
			throw new Exception();

		InputMap = value;

		return this;
	}

	/// <summary>
	/// Specifies whether signals should be logged and displayed through the engine.
	/// <para>Default value: false</para>
	/// </summary>
	public bool LogSignalEvents { get; internal set; } = false;

	/// <summary>
	/// Sets whether signal events should be logged by the engine.
	/// </summary>
	/// <param name="value">A boolean indicating whether to enable (true) or disable (false) logging of signal events.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the engine should log signal events. Enabling this can be useful for debugging or
	/// monitoring the behavior of signals within the engine. It returns the current instance of the <see cref="EngineSettings"/> class,
	/// enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithLogSignalEvents(bool value)
	{
		LogSignalEvents = value;

		return this;
	}


	/// <summary>
	/// Singletons are special classes that can be accessed globally within entities or screens.
	/// <para>Info: For components not associated with entities or screens, use 'Engine.GetSingleton'. Singletons are useful for storing data without needing to create additional instances.</para>
	/// </summary>
	public GameService[] Services { get; internal set; }
	public EngineSettings WithServices(params GameService[] values)
	{
		if (values.Length == 0)
			throw new Exception();

		Services = new GameService[values.Length];

		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] == null)
				throw new Exception();

			Services[i] = values[i];
		}

		return this;
	}

	/// <summary>
	/// Specifies whether logged messages should include the time and date.
	/// <para>Default value: true</para>
	/// </summary>
	public bool LogDateTime { get; internal set; } = true;

	/// <summary>
	/// Sets whether to include the date and time in the logged messages.
	/// </summary>
	/// <param name="value">A boolean indicating whether to include (true) or exclude (false) the date and time in log messages.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <remarks>
	/// This method allows you to configure whether the log messages should include the current date and time. Enabling this can be useful for
	/// tracking events more accurately, especially for debugging or logging purposes. It returns the current instance of the <see cref="EngineSettings"/> class,
	/// enabling fluent configuration of settings.
	/// </remarks>
	public EngineSettings WithLogDateTime(bool value)
	{
		LogDateTime = value;

		return this;
	}


	/// <summary>
	/// Action to handle error or crash detection when the game crashes.
	/// <para>Info: It can be used for internal reporting to your game server or other reporting mechanisms for remote error fixing.</para>
	/// <para>Default value: null</para>
	/// </summary>
	public Action<Engine, Exception> OnError { get; internal set; } = null;

	/// <summary>
	/// Sets the error handler action to be called when an error or exception occurs in the engine.
	/// </summary>
	/// <param name="value">The action to handle errors, which takes the engine instance and the exception as parameters.</param>
	/// <returns>The current instance of <see cref="EngineSettings"/> to allow method chaining.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when the <paramref name="value"/> is null.
	/// </exception>
	/// <remarks>
	/// This method allows you to configure a custom error handler that will be triggered when the engine encounters an exception.
	/// The provided action receives the engine instance and the exception as parameters, allowing you to handle the error as needed.
	/// </remarks>
	public EngineSettings WithOnError(Action<Engine, Exception> value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value), "Error handler cannot be null.");

		OnError += value;

		return this;
	}



	public EngineSettings Build()
	{
		// check settings...

		return this;
	}


	/// <summary>
	/// Engine settings used to configure window size, viewport size, vsync, culling, and other options.
	/// </summary>
	public EngineSettings() { } /*=> Instance ??= this;*/
}
