using System;
using System.IO;
using System.Runtime.CompilerServices;

using Box.Graphics.Batch;
using Box.Resources;
using Box.Services;
using Box.Services.Types;

namespace Box;

/// <summary>
/// The game engine responsible for managing all systems and subsystems to facilitate game development.
/// </summary>
public class Engine : GameService, IDisposable
{
	private const float FrameDelay = 1.0f;
	private const int MaxFrameSamples = 100;
	private const float SMoothingFpsFactor = 0.98f;

	internal SFMLRenderWindow _window;
	internal SFMLRenderTexture _renderTexture;

	// private static Engine _instance;

	// private readonly StringBuilder sbTitle = new();
	private readonly Queue<float> _frameSamples = new();
	private readonly EngineSettings _settings;
	private SFMLStyles _styles;
	private SFMLVideoMode _video;
	private readonly SFMLContextSettings _contextSettings;
	private bool _isClosing;
	private bool _initialized;
	private bool _oldFullscreen, _fullscreen;
	private Vect2 _oldWindowSize, _windowSize;
	private bool _isDisposed;
	private float _timeout;
	// private int _frameCount;
	private long _totalFrames;
	private SFMLSprite _renderTextureSprite;

	internal SFMLRenderTexture GetRenderTarget() => _renderTexture;

	// private readonly Clock _clock;
	// private readonly Assets _assets;
	// private readonly Renderer _renderer;
	// private readonly Rand _rand;
	// private readonly ScreenManager _screenManager;
	// private readonly InputMap _input;
	// private readonly Signal _signal;
	// private readonly SoundManager _soundManager;
	// private readonly Log _log;
	// private readonly Coroutine _coroutine;

	private readonly SFMLImage _icon;
	private readonly BoxFont _font;


	/// <summary>
	/// Current active input map.
	/// </summary>
	public InputMap Input { get; private set; }

	/// <summary>
	/// Gets or sets the <see cref="ServiceManager"/> instance used by the engine.
	/// </summary>
	/// <remarks>
	/// This property provides access to the <see cref="ServiceManager"/> which is responsible for managing the
	/// services, including registration and resolution, within the engine. The property is set internally, but it can
	/// be accessed externally for interacting with the engine's services.
	/// </remarks>
	public static ServiceCollection Services { get; internal set; }

	/// <summary>
	/// Delegate used to detect when the application is exiting.
	/// </summary>
	public Action<Engine> OnExiting;

	// /// <summary>
	// /// Gets the current instance of the engine.
	// /// </summary>
	// public static Engine Instance => _instance;

	/// <summary>
	/// Determines whether the game window has focus.
	/// </summary>
	public bool IsActive { get; private set; }

	/// <summary>
	/// The embedded engine font.
	/// </summary>
	public BoxFont EngineFont => _font;

	/// <summary>
	/// Retrieves a singleton game data object of the specified type.
	/// </summary>
	/// <remarks>
	/// Singletons can be used within entities and screens without needing to wrap data around the engine.
	/// For other types, use Engine.GetSingleton to access data or important methods.
	/// </remarks>
	/// <typeparam name="T">The type of the singleton to retrieve.</typeparam>
	/// <returns>The singleton object if it exists; otherwise, null.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetService<T>() where T : GameService
	{
		if (!Services.TryGetService<T>(out var service))
			return default;

		return service;
	}

	/// <summary>
	/// Retrieves a singleton service as an object based on the specified type.
	/// </summary>
	/// <param name="singleton">The type of the singleton service to retrieve.</param>
	/// <returns>
	/// An instance of the singleton service, or <c>null</c> if no matching service is found.
	/// </returns>
	/// <remarks>
	/// This method searches through the registered services and returns the first instance that matches the specified type.
	/// If no matching service is found, it returns <c>null</c>.
	/// </remarks>
	public static object GetServiceAsObject(Type singleton)
	{
		if (singleton == null)
			return null;

		return Services.Services.FirstOrDefault(x => x.GetType() == singleton);
	}

	/// <summary>
	/// Retrieves the application data folder path based on the current operating system.
	/// <para>Example paths:</para>
	/// <para>Windows: C:/Users/[UserName]/AppData/Roaming</para>
	/// <para>MacOS: Library/Application Support</para>
	/// <para>Linux or BSD: Typically ~/.config</para>
	/// </summary>
	/// <returns>The path to the operating system's application data folder.</returns>
	public string AppFolder => FileHelpers.GetApplicationDataPath();

	/// <summary>
	/// Retrieves the application content root folder path. For MacOS bundles, it accesses resources within the bundle.
	/// <para>Note: Uses the EngineSettings.ContentRoot to determine the content root folder path.</para>
	/// </summary>
	/// <returns>The string path of the content root folder.</returns>
	public string AppContent => FileHelpers.GetApplicationContentPath();

	/// <summary>
	/// Retrieves the application content root folder for the save folder.
	/// <para>Note: You can change the save root folder within EngineSettings.AppSaveRoot.</para>
	/// </summary>
	public string AppSaveFolder => Path.Combine(AppFolder, _settings.AppSaveRoot);

	/// <summary>
	/// Retrieves the application content root folder for the log folder.
	/// <para>Note: You can change the log root folder within EngineSettings.AppLogRoot.</para>
	/// </summary>
	public string AppLogFolder => Path.Combine(AppFolder, _settings.AppLogRoot);

	/// <summary>
	/// Retrieves the application content root folder for the settings file.
	/// <para>Note: You can change the settings file within EngineSettings.AppSettings.</para>
	/// </summary>
	public string SettingsPath => Path.Combine(AppFolder, _settings.AppSettings);

	/// <summary>
	/// Generates a hash value based on the combination of AppVersion and AppName.
	/// <para>Note: Some prefer using hashed values instead of the original string values.</para>
	/// </summary>
	public string VersionHash => $"{_settings.AppName}:{_settings.AppVersion}".ToHash();



	/// <summary>
	/// Gets or sets whether the window is in full-screen mode.
	/// <para>Note: This method does not automatically set your window to full-screen or windowed mode. 
	/// You need to call ApplyChanges to apply the full-screen settings.</para>
	/// </summary>
	public bool Fullscreen
	{
		get => _fullscreen;
		set
		{
			_oldFullscreen = _fullscreen;
			_fullscreen = value;
		}
	}

	/// <summary>
	/// Gets or sets the size of the window.
	/// <para>Note: Changing this property does not immediately resize the window. 
	/// Call ApplyChanges to apply the new window size.</para>
	/// </summary>
	public Vect2 WindowSize
	{
		get => _windowSize;
		set
		{
			_oldWindowSize = _windowSize;
			_windowSize = value;
		}
	}

	/// <summary>
	/// Applies all video or window-related settings.
	/// <para>Note: Changes to fullscreen mode, window size, vsync, or other monitor-related settings require calling ApplyChanges.</para>
	/// </summary>
	public void ApplyChanges()
	{
		if (_oldWindowSize == _windowSize && _oldFullscreen == _fullscreen)
			return;

		if (_window.IsOpen)
		{
			_window.Closed -= (_, _) => Quit();
			_window.GainedFocus -= (_, _) => IsActive = true;
			_window.LostFocus -= (_, _) => IsActive = false;

			_renderTextureSprite?.Dispose();

			_window.Close();
		}

		if (Fullscreen)
		{
			var desktop = SFMLVideoMode.DesktopMode;

			_styles = SFMLStyles.Fullscreen;
			_video = new SFMLVideoMode(desktop.Width, desktop.Height);
			_window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);
			_renderTexture = new SFMLRenderTexture(desktop.Width, desktop.Height, _contextSettings);
		}
		else
		{
			_styles = SFMLStyles.Titlebar | SFMLStyles.Close;
			_video = new SFMLVideoMode((uint)_windowSize.X, (uint)_windowSize.Y);
			_window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);
			_renderTexture = new SFMLRenderTexture((uint)_windowSize.X, (uint)_windowSize.Y, _contextSettings);
		}

		_window.Closed += (_, _) => Quit();
		_window.GainedFocus += (_, _) => IsActive = true;
		_window.LostFocus += (_, _) => IsActive = false;

		_window.SetVerticalSyncEnabled(_settings.VSync);
		_window.SetMouseCursorVisible(_settings.Mouse);
		// _window.SetFramerateLimit(_settings.VSync ? 60u : 0u);
		_window.SetIcon(_icon.Size.X, _icon.Size.Y, _icon.Pixels);

		if (_windowSize != _oldWindowSize)
		{
			GetService<Signal>().Emit(EngineSignals.WindowSizeChanged, _windowSize);
			_settings.Window = _windowSize;

			_oldWindowSize = _windowSize;
		}

		if (_fullscreen != _oldFullscreen)
		{
			GetService<Signal>().Emit(EngineSignals.WindowFullscreenChanged, _fullscreen);
			_settings.Fullscreen = _fullscreen;

			_oldFullscreen = _fullscreen;
		}

		Start();
	}

	/// <summary>
	/// Initializes the game engine with the specified settings.
	/// </summary>
	/// <param name="serviceManager">
	/// A <see cref="ServiceManager"/> containing various engine configurations such as window size, viewport size, 
	/// culling settings, and other services needed for the engine to operate. The <see cref="ServiceManager"/> manages 
	/// the lifecycle and resolution of services used throughout the engine.
	/// </param>
	public Engine(EngineSettings settings)
	{
		_settings = settings;
		Services = new ServiceCollection();

		// Required to start here or it will crash if moved. Dont move.
		Services.RegisterManyServices(this, _settings);

		_styles = _settings.Fullscreen ? SFMLStyles.Fullscreen : SFMLStyles.Titlebar | SFMLStyles.Close;
		_video = new SFMLVideoMode((uint)_settings.Window.X, (uint)_settings.Window.Y);
		_contextSettings = new SFMLContextSettings
		{
			AntialiasingLevel = (uint)_settings.AntialiasingLevel,
			AttributeFlags = SFMLContextSettings.Attribute.Default,
			SRgbCapable = false,
			MajorVersion = 3,
			MinorVersion = 3,
		};

		_icon = new SFMLImage(ResourceLoader.GetResourceBytes("box-32.png"));
		_font = new GenericFont("font.ttf", ResourceLoader.GetResourceBytes("font.ttf"), 8, false, false, 0, 0, 3);
		_window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);

		_window.Closed += (_, _) => Quit();
		_window.GainedFocus += (_, _) => IsActive = true;
		_window.LostFocus += (_, _) => IsActive = false;

		_window.SetVerticalSyncEnabled(_settings.VSync);
		_window.SetMouseCursorVisible(_settings.Mouse);
		_window.SetIcon(_icon.Size.X, _icon.Size.Y, _icon.Pixels);

		WindowSize = new Vect2(_settings.Window);
		Fullscreen = _settings.Fullscreen;

		_renderTexture = new SFMLRenderTexture((uint)_settings.Window.X, (uint)_settings.Window.Y, _contextSettings);

		if (!_initialized)
		{
			if (_settings.UseApplicationData)
				FileHelpers.EnsureDirectoryExists(AppFolder);

			FileHelpers.EnsureDirectoryExists(AppSaveFolder);
			FileHelpers.EnsureDirectoryExists(AppLogFolder);

			if (_settings.InputMap == null)
				Input = new DefaultInputMap();
			else
				Input = _settings.InputMap;

			Services.RegisterManyServices(

				Input
			);

			Services.RegisterManyServices(
				new Log(),
				new FastRandom(),
				new Clock(),
				new Assets(),
				new Signal(),
				new Renderer(),
				new Coroutine(),
				new SoundManager(),
				new ScreenManager()
			);

			Services.RegisterManyServices(settings.Services);

			if (_settings.Screens is not null && _settings.Screens.Length > 0)
				GetService<ScreenManager>().Add(_settings.Screens);

			_initialized = true;
		}
	}

	/// <summary>
	/// Destructor for the Engine.
	/// </summary>
	~Engine() => Dispose(disposing: false);


	private const float FixedStep = 1f / 60f;  // 60 UPS
	private float _accumulator = 0f;


	/// <summary>
	/// Starts the game engine.
	/// </summary>
	public void Start()
	{
		IsActive = _window.HasFocus();

		Input.LoadInputs();
		_font.Initialize();

		if (_initialized)
			_renderTextureSprite?.Dispose();

		_renderTextureSprite = new SFMLSprite(_renderTexture.Texture);

		do
		{
			if (!_window.IsOpen)
				break;

			_window.DispatchEvents();

			UpdateTitle();

			_renderTexture.Clear(SFMLColor.Transparent);
			Services.Update();
			_renderTexture.Display();

			_window.Clear(_settings.ClearColor.ToSFML());
			_window.Draw(_renderTextureSprite);
			_window.Display();

			// _frameCount++;
			_totalFrames++;
		} while (_window.IsOpen);
	}

	/// <summary>
	/// Shuts down the game engine and exits the application.
	/// </summary>
	public void Quit()
	{
		if (_window == null)
			return;
		if (!_window.IsOpen)
			return;

		OnExiting?.Invoke(this);

		_window.Close();

		_isClosing = true;
	}

	/// <summary>
	/// Retrieves the supported monitor resolutions that match the specified aspect ratio.
	/// </summary>
	/// <param name="aspectWidth">The aspect ratio width.</param>
	/// <param name="aspectHeight">The aspect ratio height.</param>
	/// <returns>A list of supported resolutions based on the given aspect ratio.</returns>
	public List<Vect2> GetSupportedMonitors(int aspectWidth, int aspectHeight)
	{
		var modes = GetSupportedMonitors();
		var aspect = (float)aspectWidth / (float)aspectHeight;
		var result = new List<Vect2>();

		for (int i = 0; i < modes.Count; i++)
		{
			var modeAspect = modes[i].X / modes[i].Y;

			if (modeAspect != aspect)
				continue;

			result.Add(new Vect2(modes[i].X, modes[i].Y));
		}

		return result.OrderBy(x => x).ToList();
	}

	/// <summary>
	/// Retrieves all supported monitor sizes that your video card supports.
	/// </summary>
	/// <returns>A list of supported monitor sizes.</returns>
	public List<Vect2> GetSupportedMonitors()
	{
		var modes = SFMLVideoMode.FullscreenModes;
		var result = new List<Vect2>();

		for (int i = 0; i < modes.Length; i++)
			result.Add(new Vect2(modes[i].Width, modes[i].Height));

		return result;
	}

	/// <summary>
	/// Retrieves the size of your current active monitor.
	/// <para>Note: For multi-monitors, it selects the primary monitor based on your operating system's configuration.</para>
	/// </summary>
	/// <returns>The size of your active monitor.</returns>
	public Vect2 GetCurrentMonitor()
	{
		var desktop = SFMLVideoMode.DesktopMode;

		return new Vect2(desktop.Width, desktop.Height);
	}

	/// <summary>
	/// Checks if a monitor size is supported.
	/// </summary>
	/// <param name="width">The width in pixels.</param>
	/// <param name="height">The height in pixels.</param>
	/// <returns>True if the monitor size is supported; otherwise, false.</returns>
	public bool IsMonitorSupported(int width, int height)
	{
		var video = new SFMLVideoMode((uint)width, (uint)height);

		return video.IsValid();
	}

	/// <summary>
	/// Checks if a monitor size is supported.
	/// </summary>
	/// <param name="size">The width and height in pixels.</param>
	/// <returns>True if the monitor size is supported; otherwise, false.</returns>
	public bool IsMonitorSupported(Vect2 size)
		=> IsMonitorSupported((int)size.X, (int)size.Y);

	private void UpdateTitle()
	{
		if (_frameSamples.Count > MaxFrameSamples - 1)
			_frameSamples.Dequeue();

		_frameSamples.Enqueue(1f / GetService<Clock>().DeltaTime);

		if (_timeout < 0f)
		{
			StringBuilder sb = new StringBuilder();
			ScreenManager sm = GetService<ScreenManager>();
			Renderer re = GetService<Renderer>();
			Assets @as = GetService<Assets>();
			Coroutine co = GetService<Coroutine>();
			Signal sl = GetService<Signal>();
			SoundManager sd = GetService<SoundManager>();

			string activeScreen = sm.ActiveScreen == null
				? "None" : sm.ActiveScreen.GetType().Name;

			sb.Append($"{_settings.AppTitle} | ");
			sb.Append($"Fps: {MathF.Round(_frameSamples.Average())}, Min: {MathF.Round(_frameSamples.Min())}, Max: {MathF.Round(_frameSamples.Max())}, Frames: {_totalFrames} | ");
			sb.Append($"Assets: {@as.Count}, Size: ~{FormatFileSize(@as.Bytes)} |  ");
			sb.Append($"Screens: {sm.Count}, Hidden: {sm.Screens.Count(x => !x.Visible)}, Screen: {activeScreen} | ");
			sb.Append($"Entities: {sm.TotalEntities}, Culled: {Math.Abs(sm.TotalEntities - re.Count)}, Drawing: {re.Count}, Batches: {re.BatchCount} | ");
			sb.Append($"Coroutines: {co.Count} | ");
			sb.Append($"Signals: {sl.Count} | ");
			sb.Append($"Timers: {sm.TotalTimers} | ");
			sb.Append($"Sounds: {sd.PlayCount}");

			_window.SetTitle(sb.ToString());

			_timeout += FrameDelay;
		}
		else
			_timeout -= GetService<Clock>().DeltaTime;
	}


	private string FormatFileSize(long fileSizeInBytes)
	{
		const long kB = 1024;
		const long mB = 1024 * kB;
		const long gB = 1024 * mB;

		if (fileSizeInBytes < kB)
		{
			return $"{fileSizeInBytes} bytes";
		}
		else if (fileSizeInBytes < mB)
		{
			return $"{(double)fileSizeInBytes / kB:F2} KB";
		}
		else if (fileSizeInBytes < gB)
		{
			return $"{(double)fileSizeInBytes / mB:F2} MB";
		}
		else
		{
			return $"{(double)fileSizeInBytes / gB:F2} GB";
		}
	}

	/// <summary>
	/// Explicitly disposes of the object.
	/// </summary>
	/// <param name="disposing">True if the engine is disposing; otherwise, false.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (_isDisposed)
			return;

		if (!_isClosing)
			_isClosing = true;

		// Always clear singals first so it doesnt emit entities 
		// removed when it clears screens or entities.
		GetService<Signal>().Clear();

		GetService<Coroutine>().StopAll();
		GetService<ScreenManager>().Clear();
		GetService<SoundManager>().EngineClear();
		GetService<Assets>().Clear();

		_window?.Dispose();
		_icon.Dispose();

		_isDisposed = true;
	}

	/// <summary>
	/// Explicitly disposes of the object.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);

		GC.SuppressFinalize(this);
	}
}
