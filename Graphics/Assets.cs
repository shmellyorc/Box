using Box.Services.Types;

namespace Box.Graphics;

/// <summary>
/// Represents a class that manages assets within the application.
/// </summary>
public sealed class Assets : GameService
{
	private static int _id;

	internal static readonly string[] SurfaceSupportedTypes = { ".bmp", ".png", ".tga", ".jpg", ".gif", ".psd", ".hdr", ".pic", ".pnm" };
	internal static readonly string[] SoundSupportedTypes = { ".wav", ".ogg", ".flac", ".mp3" };
	internal static readonly string[] FontSupportedTypes = { ".ttf", ".pfa", ".pfb", ".cff", ".otf", ".pcf", ".bdf", ".pfr" };
	internal static readonly string[] BitmapFontSupportedTypes = { ".fnt" };
	internal static readonly string[] MapSupportedTypes = { ".ldtk" };
	internal static readonly string[] SheetTypes = { ".sheet" };
	internal static readonly string[] PackTypes = { ".pack" };
	private readonly Dictionary<uint, IAsset> _assets = new();
	private BoxPackFile _packFile;

	/// <summary>
	/// Gets the number of assets currently managed.
	/// </summary>
	public int Count => _assets.Count;

	/// <summary>
	/// Gets or sets the total size of all assets in bytes.
	/// </summary>
	public long Bytes { get; internal set; }

	internal Assets() { }/* => Instance ??= this;*/

	/// <summary>
	/// Checks if an asset with the specified name exists.
	/// </summary>
	/// <param name="name">The name of the asset to check.</param>
	/// <returns>True if an asset with the specified name exists; otherwise, false.</returns>
	public bool Exists(string name) => _assets.ContainsKey(Bx.Hash(name));

	/// <summary>
	/// Checks if an asset with the specified enum value exists.
	/// </summary>
	/// <param name="name">The enum value representing the asset to check.</param>
	/// <returns>True if an asset with the specified enum value exists; otherwise, false.</returns>
	public bool Exists(Enum name) => Exists(name.ToEnumString());






	/// <summary>
	/// Adds an asset with the specified name.
	/// </summary>
	/// <param name="name">The name of the asset to add.</param>
	/// <param name="asset">The asset to add.</param>
	public void Add(string name, IAsset asset)
	{
		if (Exists(name))
			return;

		_assets[Bx.Hash(name)] = asset;
	}

	/// <summary>
	/// Adds an asset with the specified enum value.
	/// </summary>
	/// <param name="name">The enum value representing the asset to add.</param>
	/// <param name="asset">The asset to add.</param>
	public void Add(Enum name, IAsset asset) => Add(name.ToEnumString(), asset);



	/// <summary>
	/// Retrieves the asset of type T with the specified name.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="name">The name of the asset to retrieve.</param>
	/// <returns>The asset of type T with the specified name, if found; otherwise, default(T).</returns>
	public T Get<T>(string name) where T : IAsset
	{
		if (_assets.TryGetValue(Bx.Hash(name), out var asset))
			throw new ArgumentException(paramName: nameof(name), message: $"Entry called {name}, doesn't exist");
		if (asset is not T t)
			throw new ArgumentException(paramName: nameof(T), 
				message: $"Type is '{asset.GetType().Name}' but you choosed '{nameof(T)}'");

		t.Initialize();

		return t;
	}

	/// <summary>
	/// Retrieves the asset of type T with the specified enum value.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="name">The enum value representing the asset to retrieve.</param>
	/// <returns>The asset of type T with the specified enum value, if found; otherwise, default(T).</returns>
	public T Get<T>(Enum name) where T : IAsset => Get<T>(name.ToEnumString());

	/// <summary>
	/// Retrieves the asset of type T from a file with the specified filename.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="filename">The filename of the asset to retrieve.</param>
	/// <returns>The asset of type T loaded from the specified file, if found; otherwise, default(T).</returns>
	public T GetFromFile<T>(string filename) where T : IAsset
	{
		string fullPath = Path.GetFullPath(filename.Replace("..", GetService<Engine>().AppContent));

		foreach (var asset in _assets)
		{
			if (!asset.Value.Filename.Contains(fullPath, StringComparison.OrdinalIgnoreCase))
				continue;

			asset.Value.Initialize();

			return (T)asset.Value;
		}

		return default;
	}

	/// <summary>
	/// Retrieves the asset of type T from a file with the specified filename.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="filename">The filename of the asset to retrieve.</param>
	/// <returns>The asset of type T loaded from the specified file, if found; otherwise, default(T).</returns>
	public T GetFromPack<T>(string filename) where T : IAsset
	{
		// var fullPath = Path.GetFullPath(filename.Replace("..", Engine.Instance.AppContent));
		var fullPath = filename
			.Replace("..", string.Empty)
			.TrimStart('/')
			.Split(".").First();

		foreach (var asset in _assets)
		{
			if (!asset.Value.Filename.Contains(fullPath, StringComparison.OrdinalIgnoreCase))
				continue;

			asset.Value.Initialize();

			return (T)asset.Value;
		}

		return default;
	}

	/// <summary>
	/// Attempts to retrieve the asset of type T with the specified name.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="name">The name of the asset to retrieve.</param>
	/// <param name="asset">When this method returns, contains the asset of type T with the specified name, if found; otherwise, default(T).</param>
	/// <returns>True if the asset with the specified name was retrieved successfully; otherwise, false.</returns>
	public bool TryGet<T>(string name, out T asset) where T : IAsset
	{
		asset = Get<T>(name);

		return asset is not null;
	}

	/// <summary>
	/// Attempts to retrieve the asset of type T with the specified enum value.
	/// </summary>
	/// <typeparam name="T">The type of asset to retrieve, which must implement the IAsset interface.</typeparam>
	/// <param name="name">The enum value representing the asset to retrieve.</param>
	/// <param name="asset">When this method returns, contains the asset of type T with the specified enum value, if found; otherwise, default(T).</param>
	/// <returns>True if the asset with the specified enum value was retrieved successfully; otherwise, false.</returns>
	public bool TryGet<T>(Enum name, out T asset) where T : IAsset => TryGet<T>(name.ToEnumString(), out asset);



	/// <summary>
	/// Removes the asset with the specified name.
	/// </summary>
	/// <param name="name">The name of the asset to remove.</param>
	/// <returns>True if the asset with the specified name was successfully removed; otherwise, false.</returns>
	public bool Remove(string name)
	{
		if (_assets.TryGetValue(Bx.Hash(name), out _))
			return false;

		return Remove(Bx.Hash(name));
	}

	internal bool Remove(uint id)
	{
		if (!_assets.TryGetValue(id, out var asset))
			return false;

		if (File.Exists(asset.Filename))
			Bytes = Math.Max(Bytes - FileHelpers.GetFileSize(_assets[id].Filename), 0);

		asset.Dispose();

		return _assets.Remove(id);
	}

	/// <summary>
	/// Removes the asset with the specified enum value.
	/// </summary>
	/// <param name="name">The enum value representing the asset to remove.</param>
	/// <returns>True if the asset with the specified enum value was successfully removed; otherwise, false.</returns>
	public bool Remove(Enum name) => Remove(name.ToEnumString());



	/// <summary>
	/// Clears all assets from the collection.
	/// </summary>
	public void Clear()
	{
		if (_assets.Count == 0)
			return;

		foreach (var item in _assets.ToDictionary(a => a.Key, b => b.Value))
			Remove(item.Key);
	}

	private string GetFile(string filename)
	{
		var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
			? Path.Combine(GetService<Engine>().AppContent, filename.Replace(@"//", @"\").Replace(@"/", @"\"))
			: Path.Combine(GetService<Engine>().AppContent, filename.Replace(@"\\", @"/").Replace(@"\", @"/"))
			;

		var files = Directory.GetFiles(Path.GetDirectoryName(path));

		if (!Path.GetExtension(path).IsEmpty())
			path = path.Split(Path.GetExtension(filename)).First();

		foreach (var item in files)
		{
			var file = Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item));

			if (file.Contains(path, StringComparison.OrdinalIgnoreCase))
				return item;
		}

		return string.Empty;
	}

	/// <summary>
	/// Loads an asset pack from the specified file and overrides the content loading
	/// to use the pack instead of the local file system.
	/// </summary>
	/// <param name="filename">The path to the asset pack file.</param>
	/// <exception cref="ArgumentNullException">Thrown when the filename is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the file format is not supported.</exception>
	public void LoadPack(string filename)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		string path = GetFile(filename);

		if (path.IsEmpty() || !File.Exists(path))
			throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
		if (!PackTypes.Contains(Path.GetExtension(path).ToLower()))
			throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", PackTypes)}");

		_packFile = BoxPackLoader.LoadPack(path);
	}


	/// <summary>
	/// Loads a surface from the specified file.
	/// </summary>
	/// <param name="filename">The filename of the surface file to load.</param>
	/// <param name="repeat">A boolean indicating whether the region should be repeated to fill the new surface. Default is <c>false</c>.</param>
	/// <param name="smooth">A boolean indicating whether the new surface should be smoothed (anti-aliased). Default is <c>false</c>.</param>
	/// <returns>The loaded Surface object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the file type of <paramref name="filename"/> is not supported.</exception>
	public Surface LoadSurface(string filename, bool repeat = false, bool smooth = false)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		Assets a = GetService<Assets>();

		if (_packFile == null)
		{
			var path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!SurfaceSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", SurfaceSupportedTypes)}");

			a.Bytes += FileHelpers.GetFileSize(path);

			return new Surface(path, File.ReadAllBytes(path), repeat, smooth);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackSurface>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new Surface(pack.Filename, pack.Bytes, repeat, smooth);
		}
	}

	/// <summary>
	/// Loads a subsurface from the specified file based on the given region.
	/// </summary>
	/// <param name="filename">The filename of the image file to load.</param>
	/// <param name="region">The rectangular region specifying the area of the image to load.</param>
	/// <param name="repeat">A boolean indicating whether the region should be repeated to fill the new surface. Default is <c>false</c>.</param>
	/// <param name="smooth">A boolean indicating whether the new surface should be smoothed (anti-aliased). Default is <c>false</c>.</param>
	/// <returns>The loaded Surface object representing the subsurface.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="region"/> is empty or invalid.</exception>
	/// <exception cref="Exception">Thrown when an error occurs during the subsurface loading process.</exception>
	public Surface LoadSubSurface(string filename, Rect2 region, bool repeat = false, bool smooth = false)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		string path = GetFile(filename);
		Assets a = GetService<Assets>();

		if (path.IsEmpty() || !File.Exists(path))
			throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
		if (!SurfaceSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
			throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", SurfaceSupportedTypes)}");
		if (region.IsEmpty)
			throw new ArgumentException("Region is empty!", nameof(region));

		a.Bytes += FileHelpers.GetFileSize(path);

		return new Surface(path, File.ReadAllBytes(path), region, repeat, smooth);
	}

	/// <summary>
	/// Loads a subsurface from the specified spritesheet onto the given surface.
	/// </summary>
	/// <param name="surface">The Surface object onto which the subsurface will be loaded.</param>
	/// <param name="sheet">The Spritesheet containing the subsurface.</param>
	/// <param name="name">The name of the subsurface within the spritesheet.</param>
	/// <param name="repeat">A boolean indicating whether the region should be repeated to fill the new surface. Default is <c>false</c>.</param>
	/// <param name="smooth">A boolean indicating whether the new surface should be smoothed (anti-aliased). Default is <c>false</c>.</param>
	/// <returns>The loaded Surface object representing the subsurface.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when:
	/// - <paramref name="surface"/> is null or empty.
	/// - <paramref name="sheet"/> is null or empty.
	/// - The specified <paramref name="name"/> does not exist within the <paramref name="sheet"/>.
	/// </exception>
	public Surface LoadSubSurface(Surface surface, Spritesheet sheet, string name, bool repeat = false, bool smooth = false)
	{
		if (surface == null)
			throw new ArgumentException("Surface is empty!", nameof(surface));
		if (sheet.IsEmpty)
			throw new ArgumentException("Sheet is empty!", nameof(sheet));
		if (!sheet.Contains(name))
			throw new ArgumentException($"'{name}' doesn't exist!", nameof(name));

		return new Surface(surface, sheet.GetBounds(name), repeat, smooth);
	}

	/// <summary>
	/// Loads a generic font from the specified file with optional parameters for customization.
	/// </summary>
	/// <param name="filename">The filename of the font file to load.</param>
	/// <param name="size">The size of the font.</param>
	/// <param name="useSmoothing">Optional. Indicates whether to use smoothing for the font.</param>
	/// <param name="bold">Optional. Indicates whether the font should be bold.</param>
	/// <param name="thickness">Optional. Specifies the thickness of the font.</param>
	/// <param name="spacing">Optional. Specifies the spacing of the font.</param>
	/// <param name="lineSpacing">Optional. Specifies the line spacing of the font.</param>
	/// <returns>The loaded GenericFont object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the font file type is not supported.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="size"/> is zero.</exception>
	public GenericFont LoadFont(string filename, int size, bool useSmoothing = false, bool bold = false, int thickness = 0, int spacing = 0, int lineSpacing = 0)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		Assets a = GetService<Assets>();

		if (_packFile is null)
		{
			string path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!FontSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", FontSupportedTypes)}");
			if (size == 0)
				throw new ArgumentException("Size is zero!", nameof(size));

			a.Bytes += FileHelpers.GetFileSize(path);

			return new GenericFont(path, File.ReadAllBytes(path), size, useSmoothing, bold, thickness, spacing, lineSpacing);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackFont>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new GenericFont(pack.Filename, pack.Bytes, size, useSmoothing, bold, thickness, spacing, lineSpacing);
		}
	}

	/// <summary>
	/// Loads a bitmap font from the specified file with optional parameters for customization.
	/// </summary>
	/// <param name="filename">The filename of the bitmap font file to load.</param>
	/// <param name="spacing">Optional. Specifies the spacing of the bitmap font.</param>
	/// <param name="lineSpacing">Optional. Specifies the line spacing of the bitmap font.</param>
	/// <returns>The loaded BitmapFont object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the bitmap font file type is not supported.</exception>
	public Fonts.BitmapFont LoadBitmapFont(string filename, int spacing = 0, int lineSpacing = 0)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		Assets a = GetService<Assets>();

		if (_packFile == null)
		{
			var path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!BitmapFontSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", BitmapFontSupportedTypes)}");

			a.Bytes += FileHelpers.GetFileSize(path);

			return new BitmapFont(path, spacing, lineSpacing);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackBitmapFont>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new BitmapFont(pack.Filename, pack.Bytes, pack.PageAsset, spacing, lineSpacing);
		}
	}

	/// <summary>
	/// Loads a sound from the specified file.
	/// </summary>
	/// <param name="filename">The filename of the sound file to load.</param>
	/// <param name="looped">A flag indicating whether the sound should loop when played. Default is <c>false</c>.</param>
	/// <returns>The loaded <see cref="Sound"/> object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the sound file type is not supported.</exception>
	/// <remarks>
	/// This method attempts to load a sound file either from the local file system or a packed asset file.
	/// If the sound is found in a packed file, it is loaded from there. Otherwise, it is loaded from the file system.
	/// The sound file must be of a supported type, and the filename must not be empty or null.
	/// </remarks>
	public Sound LoadSound(string filename, bool looped = false)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		var a = GetService<Assets>();

		if (_packFile == null)
		{
			var path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!SoundSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'. Can support: {string.Join(", ", SoundSupportedTypes)}");

			a.Bytes += FileHelpers.GetFileSize(path);

			return new Sound(path, File.ReadAllBytes(path), looped);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackSound>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new Sound(pack.Filename, pack.Bytes, looped);
		}
	}

	/// <summary>
	/// Loads a map from the specified file.
	/// </summary>
	/// <param name="filename">The filename of the map file to load.</param>
	/// <returns>The loaded Map object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the map file type is not supported.</exception>
	public Map LoadMap(string filename)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		Assets a = GetService<Assets>();

		if (_packFile == null)
		{
			var path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!MapSupportedTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", MapSupportedTypes)}");

			a.Bytes += FileHelpers.GetFileSize(path);

			return new Map(path);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackMap>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new Map(pack.Filename, pack.Bytes);
		}
	}

	/// <summary>
	/// Loads a spritesheet from the specified file.
	/// </summary>
	/// <param name="filename">The filename of the spritesheet file to load.</param>
	/// <returns>The loaded Spritesheet object.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
	/// <exception cref="NotSupportedException">Thrown when the spritesheet file type is not supported.</exception>
	public Spritesheet LoadSpriteSheet(string filename)
	{
		if (filename.IsEmpty())
			throw new ArgumentNullException(nameof(filename), "Filename is null or empty");

		Assets a = GetService<Assets>();

		if (_packFile == null)
		{
			var path = GetFile(filename);

			if (path.IsEmpty() || !File.Exists(path))
				throw new FileNotFoundException($"Unable to find asset '{filename}' from '{GetService<Engine>().AppContent}'", path);
			if (!SheetTypes.Contains(Path.GetExtension(path).ToLower()))
				throw new NotSupportedException($"Unsupported file type from '{Path.GetFileName(path)}'!. Can support: {string.Join(", ", SheetTypes)}");

			a.Bytes += FileHelpers.GetFileSize(path);

			return new Spritesheet(path);
		}
		else
		{
			if (!_packFile.TryGet<BoxPackSpritesheet>(filename, out var pack))
				throw new FileNotFoundException($"$unable to find packed asset '{filename}' from {_packFile.BoxPackPath}.");

			a.Bytes += pack.Bytes.Length;

			return new Spritesheet(pack.Filename, pack.Bytes);
		}
	}






	/// <summary>
	/// Retrieves a Surface object by its name.
	/// </summary>
	/// <param name="name">The name of the Surface object to retrieve.</param>
	/// <returns>The Surface object identified by the specified name.</returns>
	/// <exception cref="ArgumentException">Thrown when the specified name does not correspond to an existing Surface object.</exception>
	public static Surface GetSurface(string name)
	{
		Assets a = Engine.GetService<Assets>();

		if (!a.Exists(name))
			throw new ArgumentException($"Unable to find asset of '{name}'!", nameof(name));

		return a.Get<Surface>(name);
	}

	/// <summary>
	/// Retrieves a Map object by its name.
	/// </summary>
	/// <param name="name">The name of the Map object to retrieve.</param>
	/// <returns>The Map object identified by the specified name.</returns>
	/// <exception cref="ArgumentException">Thrown when the specified name does not correspond to an existing Map object.</exception>
	public static Map GetMap(string name)
	{
		var a = Engine.GetService<Assets>();

		if (!a.Exists(name))
			throw new ArgumentException($"Unable to find asset of '{name}'!", nameof(name));

		return a.Get<Map>(name);
	}

	/// <summary>
	/// Retrieves a Sound object by its name.
	/// </summary>
	/// <param name="name">The name of the Sound object to retrieve.</param>
	/// <returns>The Sound object identified by the specified name.</returns>
	/// <exception cref="ArgumentException">Thrown when the specified name does not correspond to an existing Sound object.</exception>
	public static Sound GetSound(string name)
	{
		var a = Engine.GetService<Assets>();

		if (!a.Exists(name))
			throw new ArgumentException($"Unable to find asset of '{name}'!", nameof(name));

		return a.Get<Sound>(name);
	}

	/// <summary>
	/// Retrieves a Font object by its name.
	/// </summary>
	/// <param name="name">The name of the Font object to retrieve.</param>
	/// <returns>The Font object identified by the specified name.</returns>
	/// <exception cref="ArgumentException">Thrown when the specified name does not correspond to an existing Font object.</exception>
	public static BoxFont GetFont(string name)
	{
		var a = Engine.GetService<Assets>();

		if (!a.Exists(name))
			throw new ArgumentException($"Unable to find asset of '{name}'!", nameof(name));

		return a.Get<BoxFont>(name);
	}

	/// <summary>
	/// Retrieves a Spritesheet object by its name.
	/// </summary>
	/// <param name="name">The name of the Spritesheet object to retrieve.</param>
	/// <returns>The Spritesheet object identified by the specified name.</returns>
	/// <exception cref="ArgumentException">Thrown when the specified name does not correspond to an existing Spritesheet object.</exception>
	public static Spritesheet GetSheet(string name)
	{
		var a = Engine.GetService<Assets>();

		if (!a.Exists(name))
			throw new ArgumentException($"Unable to find asset of '{name}'!", nameof(name));

		return a.Get<Spritesheet>(name);
	}


	/// <summary>
	/// Retrieves a Surface object by its enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Surface object to retrieve.</param>
	/// <returns>The Surface object identified by the specified enum name.</returns>
	public static Surface GetSurface(Enum name) => GetSurface(name.ToEnumString());

	/// <summary>
	/// Retrieves a Map object by its enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Map object to retrieve.</param>
	/// <returns>The Map object identified by the specified enum name.</returns>
	public static Map GetMap(Enum name) => GetMap(name.ToEnumString());

	/// <summary>
	/// Retrieves a Sound object by its enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Sound object to retrieve.</param>
	/// <returns>The Sound object identified by the specified enum name.</returns>
	public static Sound GetSound(Enum name) => GetSound(name.ToEnumString());

	/// <summary>
	/// Retrieves a Font object by its enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Font object to retrieve.</param>
	/// <returns>The Font object identified by the specified enum name.</returns>
	public static BoxFont GetFont(Enum name) => GetFont(name.ToEnumString());

	/// <summary>
	/// Retrieves a Spritesheet object by its enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the Spritesheet object to retrieve.</param>
	/// <returns>The Spritesheet object identified by the specified enum name.</returns>
	public static Spritesheet GetSheet(Enum name) => GetSheet(name.ToEnumString());
}
