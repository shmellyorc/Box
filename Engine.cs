using Box.Graphics.Batch;
using Box.Resources;
using Box.Services;

namespace Box;

/// <summary>
/// The game engine responsible for managing all systems and subsystems to facilitate game development.
/// </summary>
public class Engine : IDisposable
{
	private const float FrameDelay = 1.0f;
	private const int MaxFrameSamples = 100;

	internal SFMLRenderWindow Window;
	internal SFMLRenderTexture RenderTexture;

	private readonly Queue<float> _frameSamples = new();
	private readonly EngineSettings _settings;
	private readonly bool _initialized;
	private SFMLStyles _styles;
	private SFMLVideoMode _video;
	private readonly SFMLContextSettings _contextSettings;
	private bool _isClosing, _isDisposed, _oldFullscreen, _fullscreen;
	private float _timeout;
	private Vect2 _oldWindowSize, _windowSize;
	private ulong _totalFrames;
	private SFMLSprite _renderTextureSprite;
	private readonly Clock _clock;
	private readonly Assets _assets;
	private readonly Renderer _renderer;
	private readonly FastRandom _rand;
	private readonly ScreenManager _screenManager;
	private readonly InputMap _input;
	private readonly Signal _signal;
	private readonly SoundManager _soundManager;
	private readonly Log _log;
	private readonly Coroutine _coroutine;
	private readonly SFMLImage _icon;
	private readonly ServiceManager _service;


	/// <summary>
	/// Current active input map.
	/// </summary>
	public InputMap Input { get; private set; }

	/// <summary>
	/// Delegate used to detect when the application is exiting.
	/// </summary>
	public Action<Engine> OnExiting;

	/// <summary>
	/// Gets the current instance of the engine.
	/// </summary>
	public static Engine Instance { get; private set; }

	/// <summary>
	/// Determines whether the game window has focus.
	/// </summary>
	public bool IsActive { get; private set; }

	/// <summary>
	/// The embedded engine font.
	/// </summary>
	public BoxFont EngineFont { get; private set; }

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

		if (Window != null && Window.IsOpen)
		{
			Window.Closed -= (_, _) => Quit();
			Window.GainedFocus -= (_, _) => IsActive = true;
			Window.LostFocus -= (_, _) => IsActive = false;

			_renderTextureSprite?.Dispose();

			Window.Close();

		}

		Window?.Dispose();
		Window = null;

		if (Fullscreen)
		{
			var desktop = SFMLVideoMode.DesktopMode;

			_styles = SFMLStyles.Fullscreen;
			_video = new SFMLVideoMode(desktop.Width, desktop.Height);
			Window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);
			RenderTexture = new SFMLRenderTexture(desktop.Width, desktop.Height, _contextSettings);
		}
		else
		{
			_styles = SFMLStyles.Titlebar | SFMLStyles.Close;
			_video = new SFMLVideoMode((uint)_windowSize.X, (uint)_windowSize.Y);
			Window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);
			RenderTexture = new SFMLRenderTexture((uint)_windowSize.X, (uint)_windowSize.Y, _contextSettings);
		}

		Window.Closed += (_, _) => Quit();
		Window.GainedFocus += (_, _) => IsActive = true;
		Window.LostFocus += (_, _) => IsActive = false;

		Window.SetVerticalSyncEnabled(_settings.VSync);
		Window.SetMouseCursorVisible(_settings.Mouse);
		Window.SetIcon(_icon.Size.X, _icon.Size.Y, _icon.Pixels);

		if (_windowSize != _oldWindowSize)
		{
			Signal.Instance.Emit(EngineSignals.WindowSizeChanged, _windowSize);
			_settings.Window = _windowSize;

			_oldWindowSize = _windowSize;
		}

		if (_fullscreen != _oldFullscreen)
		{
			Signal.Instance.Emit(EngineSignals.WindowFullscreenChanged, _fullscreen);
			_settings.Fullscreen = _fullscreen;

			_oldFullscreen = _fullscreen;
		}
	}

	/// <summary>
	/// Initializes the game engine with the specified settings.
	/// </summary>
	public Engine(EngineSettings settings)
	{
		Instance ??= this;
		_settings = settings;

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
		EngineFont = new GenericFont("font.ttf", ResourceLoader.GetResourceBytes("font.ttf"), 8, false, false, 0, 0, 3);
		Window = new SFMLRenderWindow(_video, _settings.AppTitle, _styles, _contextSettings);

		Window.Closed += (_, _) => Quit();
		Window.GainedFocus += (_, _) => IsActive = true;
		Window.LostFocus += (_, _) => IsActive = false;

		Window.SetVerticalSyncEnabled(_settings.VSync);
		Window.SetMouseCursorVisible(_settings.Mouse);
		Window.SetIcon(_icon.Size.X, _icon.Size.Y, _icon.Pixels);

		WindowSize = new Vect2(_settings.Window);
		Fullscreen = _settings.Fullscreen;

		RenderTexture = new SFMLRenderTexture((uint)_settings.Window.X, (uint)_settings.Window.Y, _contextSettings);

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

			_log = new();
			_rand = new();
			_clock = new();
			_assets = new();
			_signal = new();
			_service = new();
			_renderer = new();
			_coroutine = new();
			_soundManager = new();
			_screenManager = new();

			_service.RegisterManyServices(settings.Services);

			if (_settings.Screens is not null && _settings.Screens.Length > 0)
				ScreenManager.Instance.Add(_settings.Screens);

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
		IsActive = Window.HasFocus();

		Input.LoadInputs();
		EngineFont.Initialize();

		if (_initialized)
			_renderTextureSprite?.Dispose();

		_renderTextureSprite = new SFMLSprite(RenderTexture.Texture);

		do
		{
			if (!Window.IsOpen)
				break;

			Window.DispatchEvents();
			_clock.Update();
			_coroutine.Update();

			UpdateTitle();

			RenderTexture.Clear(SFMLColor.Transparent);
			_screenManager.Update();
			RenderTexture.Display();

			Window.Clear(_settings.ClearColor.ToSFML());
			Window.Draw(_renderTextureSprite);
			Window.Display();

			// _frameCount++;
			_totalFrames++;
		} while (Window.IsOpen);
	}

	/// <summary>
	/// Shuts down the game engine and exits the application.
	/// </summary>
	public void Quit()
	{
		if (Window == null)
			return;
		if (!Window.IsOpen)
			return;

		OnExiting?.Invoke(this);

		Window.Close();

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
		if (_frameSamples.Count >= MaxFrameSamples)
			_frameSamples.Dequeue();

		_frameSamples.Enqueue(1f / _clock.DeltaTime);

		if (_timeout < 0f)
		{
			var sb = new StringBuilder();
			var sm = _screenManager;
			var re = _renderer;
			var @as = _assets;
			var co = _coroutine;
			var sl = _signal;
			var sd = _soundManager;

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

			Window.SetTitle(sb.ToString());

			_timeout += FrameDelay;
		}
		else
			_timeout -= _clock.DeltaTime;
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
		_signal.Clear();

		_coroutine.StopAll();
		_screenManager.Clear();
		_soundManager.EngineClear();
		_assets.Clear();

		Window?.Dispose();
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
