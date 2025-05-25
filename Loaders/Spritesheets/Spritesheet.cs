namespace Box.Loaders.Spritesheets;

/// <summary>
/// Represents a spritesheet asset that contains multiple sprites arranged in a grid or other layout.
/// </summary>
public struct Spritesheet : IAsset
{
	private Dictionary<string, Slice> _data;

	public uint Id { get; private set; }

	/// <summary>
	/// Gets the filename associated with this asset.
	/// </summary>
	public readonly string Filename { get; }

	/// <summary>
	/// Gets a value indicating whether the asset data is empty.
	/// </summary>
	public readonly bool IsEmpty => _data.IsEmpty();

	/// <summary>
	/// Gets or sets a value indicating whether the asset has been disposed.
	/// </summary>
	public bool IsDisposed { get; private set; }

	/// <summary>
	/// Gets or sets a value indicating whether the asset has been initialized.
	/// </summary>
	public bool Initialized { get; internal set; }

	internal Spritesheet(string filename, byte[] bytes)
	{
		Filename = filename;

		LoadSpritesheet(bytes);

		Id = HashHelpers.Hash32($"{filename}{bytes.Length:X8}{_data.GetHashCode():X8}");
	}

	internal Spritesheet(string filename) : this(filename, File.ReadAllBytes(filename)) { }

	private void LoadSpritesheet(byte[] rawData)
	{
		var data = Encoding.UTF8.GetString(rawData);
		var json = JsonSerializer.Deserialize<JsonObject>(data);
		var sliceArray = json["meta"]["slices"].AsArray();

		_data = new Dictionary<string, Slice>();

		for (int i = 0; i < sliceArray.Count; i++)
		{
			var name = sliceArray[i]["name"].ToString();
			var keys = sliceArray[i]["keys"].AsArray();

			Rect2 bounds = Rect2.Empty;
			Rect2 ninepatch = Rect2.Empty;
			Vect2 pivot = Vect2.Zero;

			for (int x = 0; x < keys.Count; x++)
			{
				if (keys[x]["bounds"] is not null)
				{
					bounds = new Rect2(
						(float)keys[x]["bounds"]["x"],
						(float)keys[x]["bounds"]["y"],
						(float)keys[x]["bounds"]["w"],
						(float)keys[x]["bounds"]["h"]
					);
				}

				if (keys[x]["center"] is not null)
				{
					ninepatch = new Rect2(
						(float)keys[x]["center"]["x"],
						(float)keys[x]["center"]["y"],
						(float)keys[x]["center"]["w"],
						(float)keys[x]["center"]["h"]
					);
				}

				if (keys[x]["pivot"] is not null)
				{
					pivot = new Vect2(
						(float)keys[x]["pivot"]["x"],
						(float)keys[x]["pivot"]["y"]
					);
				}
			}

			if (_data.ContainsKey(name))
			{
				Debug.WriteLine($"slice '{name}' already exists. Cannot add this value, please change slice name.");

				continue;
			}

			_data.Add(name, new Slice(bounds, ninepatch, pivot));
		}
	}

	/// <summary>
	/// Checks if the spritesheet contains a sprite with the specified name.
	/// </summary>
	/// <param name="name">The name of the sprite to check.</param>
	/// <returns>True if the spritesheet contains the sprite; otherwise, false.</returns>
	public readonly bool Contains(string name) => _data.ContainsKey(name);

	/// <summary>
	/// Retrieves the bounding rectangle of the sprite with the specified name.
	/// </summary>
	/// <param name="name">The name of the sprite.</param>
	/// <returns>The bounding rectangle of the sprite, or Rect2.Empty if the sprite doesn't exist.</returns>
	public readonly Rect2 GetBounds(string name)
	{
		if (!Contains(name))
			return Rect2.Empty;

		return _data[name].Bounds;
	}

	/// <summary>
	/// Retrieves the ninepatch data of the sprite with the specified name.
	/// </summary>
	/// <param name="name">The name of the sprite.</param>
	/// <returns>The ninepatch data of the sprite, or Rect2.Empty if the sprite doesn't exist.</returns>
	public readonly Rect2 GetNinepatch(string name)
	{
		if (!Contains(name))
			return Rect2.Empty;

		return _data[name].Ninepatch;
	}

	/// <summary>
	/// Retrieves the pivot point of the sprite with the specified name.
	/// </summary>
	/// <param name="name">The name of the sprite.</param>
	/// <returns>The pivot point of the sprite, or Vect2.Zero if the sprite doesn't exist.</returns>
	public readonly Vect2 GetPivot(string name)
	{
		if (!Contains(name))
			return Vect2.Zero;

		return _data[name].Pivot;
	}

	/// <summary>
	/// Disposes of the spritesheet, releasing any resources it holds.
	/// </summary>
	public void Dispose()
	{
		if (IsDisposed)
			return;

		IsDisposed = true;
	}

	/// <summary>
	/// Initializes the spritesheet.
	/// </summary>
	public readonly void Initialize() { }
}
