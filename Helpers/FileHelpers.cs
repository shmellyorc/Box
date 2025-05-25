namespace Box.Helpers;

/// <summary>
/// FileUtils provides helper functions for locating content files, application data paths, and other utility file operations.
/// </summary>
public static class FileHelpers
{
	/// <summary>
	/// Retrieves the application content root folder path. For MacOS bundles, it accesses resources within the bundle.
	/// <para>Note: Uses the EngineSettings.ContentRoot to determine the content root folder path.</para>
	/// </summary>
	/// <returns>The string path of the content root folder.</returns>
	public static string GetApplicationContentPath() // GetApplicationPath to GetApplicationContentPath
	{
		var basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

		// Is it a MacOS bundle?
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && Engine.GetService<EngineSettings>().IsMacBundle)
		{
			var pathComponents = basePath.Split('/');
			var macPath = string.Join("/", pathComponents, 0, pathComponents.Length - 2);
			var resourcePath = Path.Combine(macPath, "Resources", Engine.GetService<EngineSettings>().AppContentRoot);

			return resourcePath;
		}

		return Path.Combine(basePath, Engine.GetService<EngineSettings>().AppContentRoot);
	}

	/// <summary>
	/// Retrieves the application data folder path based on the current operating system.
	/// <para>Example paths:</para>
	/// <para>Windows: C:/Users/[UserName]/AppData/Roaming</para>
	/// <para>MacOS: Library/Application Support</para>
	/// <para>Linux or BSD: Typically ~/.config</para>
	/// </summary>
	/// <returns>The path to the operating system's application data folder.</returns>
	public static string GetApplicationDataPath()
	{

		if (Engine.GetService<EngineSettings>().UseApplicationData)
		{
			string path;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// Get the path to the Application Data folder on Windows
				path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
					Environment.SpecialFolderOption.DoNotVerify);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				// Get the path to the Library/Application Support folder on macOS
				path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support");
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				// Get the path to the Application Data folder on Linux (typically ~/.config)
				path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
					Environment.SpecialFolderOption.DoNotVerify);
			}
			else
			{
				// Default to the base directory if the OS is not recognized
				path = AppDomain.CurrentDomain.BaseDirectory;
			}

			return Path.Combine(path, Engine.GetService<EngineSettings>().AppName);
		}

		return AppDomain.CurrentDomain.BaseDirectory;
	}

	/// <summary>
	/// Ensures that the directory exists; creates it if it does not already exist.
	/// </summary>
	/// <param name="path">The path to the directory.</param>
	public static void EnsureDirectoryExists(string path)
	{
		string directory = Path.GetDirectoryName(path);

		if (string.IsNullOrEmpty(directory))
		{
			// This can happen if path is a root directory (like "C:\").
			throw new ArgumentException("Invalid path", nameof(path));
		}

		// Create the directory if it doesn't exist.
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			Directory.CreateDirectory(path);
		else
			Directory.CreateDirectory(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
	}


	/// <summary>
	/// Retrieves the size of a file.
	/// </summary>
	/// <param name="filename">The filename to retrieve the size of.</param>
	/// <returns>Returns 0 if the file does not exist; otherwise, returns the size of the file in bytes.</returns>
	public static long GetFileSize(string filename)
	{
		if (!File.Exists(filename))
			return 0;

		FileInfo fileInfo = new(filename);

		return fileInfo.Length;
	}

	/// <summary>
	/// Retrieves the path to the save folder.
	/// <para>Note: This method utilizes EngineSettings.AppSaveRoot and EngineSettings.UseApplicationData. If UseApplicationData is enabled,</para>
	/// <para>it retrieves the path based on the application data folder. Otherwise, it uses the current application directory.</para>
	/// </summary>
	/// <param name="filename">filename to append to the save folder path.</param>
	/// <returns>The string path of the save folder location.</returns>
	public static string SaveFilePath(string filename) => Path.Combine(Engine.GetService<Engine>().AppSaveFolder, filename);

	/// <summary>
	/// Checks if a file exists in the save folder path.
	/// <para>Note: This method utilizes EngineSettings.AppSaveRoot and EngineSettings.UseApplicationData. If UseApplicationData is enabled,</para>
	/// <para>it retrieves the path based on the application data folder. Otherwise, it uses the current application directory.</para>
	/// </summary>
	/// <param name="filename">Optional filename to append to the save folder path.</param>
	/// <returns>True if the file exists in the save folder location; otherwise, false.</returns>
	public static bool SaveFileExists(string filename) => File.Exists(SaveFilePath(filename));

	/// <summary>
	/// Checks if an asset content file exists.
	/// <para>Only used for content files.</para>
	/// <para>Note: This method checks the folder where the file is located.</para>
	/// </summary>
	/// <param name="filename">The filename to check for existence.</param>
	/// <returns>True if the content asset file exists; otherwise, false.</returns>
	public static bool AssetFileExists(string filename)
	{
		string file;

		if (Path.HasExtension(filename))
			file = Path.GetFileNameWithoutExtension(filename);
		else
			file = filename;

		var matchingFiles = Directory.GetFiles(GetApplicationContentPath(), $"{file}.*");

		if (matchingFiles.Length > 0)
			return true;

		return false;
	}

	/// <summary>
	/// Checks if the specified file is currently being read from or written to.
	/// <para>Note: Returns false if the file does not exist.</para>
	/// </summary>
	/// <param name="filename">The path to the file.</param>
	/// <returns>True if the file exists and is being read from or written to; otherwise, false.</returns>
	public static bool IsFileLocked(string filename)
	{
		if (!File.Exists(filename))
			return false;

		try
		{
			using var stream = File.OpenRead(filename);

			return false;
		}
		catch (IOException)
		{
			return true;
		}
	}



	/// <summary>
	/// Waits until the specified file is no longer locked by another process.
	/// </summary>
	/// <param name="filename">The full path to the file to monitor.</param>
	/// <param name="timeout">Optional timeout duration in seconds. If set to 0 or less, waits indefinitely.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that yields until the file becomes unlocked or the timeout is reached.</returns>
	/// <remarks>
	/// This method repeatedly checks if the file is locked using <see cref="IsFileLocked"/> and yields each frame until the file becomes accessible.
	/// If the timeout is exceeded, the coroutine exits silently.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return FileHelper.WaitUntilFileUnlocked("export.tmp", 5f);
	/// LoadExportedData();
	/// </code>
	/// </example>
	public static IEnumerator WaitUntilFileUnlocked(string filename, float timeout = 0f)
	{
		if (!File.Exists(filename))
			yield break;

		float elapsed = 0f;

		while (IsFileLocked(filename))
		{
			yield return null;

			if (timeout > 0f)
			{
				elapsed += Engine.GetService<Clock>().DeltaTime;

				if (elapsed >= timeout)
					yield break;
			}
		}
	}
}
