namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents a map asset that implements the IAsset interface.
/// </summary>
public struct Map : IAsset // Root
{
	public uint Id { get; private set; }

	/// <summary>
	/// Gets the version of the map.
	/// </summary>
	public string Version { get; internal set; }

	/// <summary>
	/// Gets the collection of tilesets associated with the map.
	/// </summary>
	public Dictionary<int, MapTileset> Tilesets { get; internal set; }

	/// <summary>
	/// Gets the list of levels defined within the map.
	/// </summary>
	public List<MapLevel> Levels { get; internal set; }

	/// <summary>
	/// Gets or sets the global levels associated with the map.
	/// </summary>
	public Dictionary<string, MapLevel> GlobalLevels { get; internal set; }

	/// <summary>
	/// Gets or sets the global layers associated with the map.
	/// </summary>
	public Dictionary<string, MapLayer> GlobalLayers { get; internal set; }

	/// <summary>
	/// Gets or sets the global entity layers associated with the map.
	/// </summary>
	public Dictionary<string, MapEntityLayer> GlobalEntities { get; internal set; }

	/// <summary>
	/// Gets the filename of the map.
	/// </summary>
	public string Filename { get; }

	/// <summary>
	/// Indicates whether the map is considered empty based on its filename and levels count.
	/// </summary>
	public readonly bool IsEmpty => Filename is null || Levels.Count == 0;

	/// <summary>
	/// Gets or sets a value indicating whether the map is disposed.
	/// </summary>
	public bool IsDisposed { get; private set; }

	/// <summary>
	/// Gets or sets a value indicating whether the map is initialized.
	/// </summary>
	public bool Initialized { get; internal set; }

	/// <summary>
	/// Retrieves the tileset from the specified map by its index.
	/// </summary>
	/// <param name="map">The map containing the tilesets.</param>
	/// <param name="index">The index of the tileset to retrieve.</param>
	/// <returns>The tileset at the specified index.</returns>
	/// <exception cref="Exception">Thrown when the specified tileset index doesn't exist.</exception>
	public static MapTileset GetTileset(Map map, int index)
	{
		if (!map.Tilesets.TryGetValue(index, out var tileset))
			throw new Exception("Tileset doesnt exist");

		return tileset;
	}

	/// <summary>
	/// Tries to retrieve the tileset from the specified map by its index.
	/// </summary>
	/// <param name="map">The map containing the tilesets.</param>
	/// <param name="index">The index of the tileset to retrieve.</param>
	/// <param name="tileset">When this method returns, contains the tileset at the specified index, if found; otherwise, default.</param>
	/// <returns><c>true</c> if the tileset was successfully retrieved; otherwise, <c>false</c>.</returns>
	public static bool TryGetTileset(Map map, int index, out MapTileset tileset)
	{
		tileset = GetTileset(map, index);

		return !tileset.IsEmpty;
	}

	/// <summary>
	/// Retrieves the level from the specified map by its name.
	/// </summary>
	/// <param name="map">The map containing the levels.</param>
	/// <param name="name">The name of the level to retrieve.</param>
	/// <param name="ignoreCase">Optional. Determines whether to ignore case when matching the level name. Default is <c>true</c>.</param>
	/// <returns>The level with the specified name, or <c>null</c> if not found.</returns>
	public static MapLevel GetLevel(Map map, string name, bool ignoreCase = true)
	{
		if (name is null)
			return default;

		foreach (var item in map.Levels)
		{
			var result = ignoreCase
				? item.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
				: item.Name == name;

			if (result)
				return item;
		}

		return default;
	}

	/// <summary>
	/// Tries to retrieve the level from the specified map by its name.
	/// </summary>
	/// <param name="map">The map containing the levels.</param>
	/// <param name="name">The name of the level to retrieve.</param>
	/// <param name="ignoreCase">Determines whether to ignore case when matching the level name.</param>
	/// <param name="level">When this method returns, contains the level with the specified name, if found; otherwise, default.</param>
	/// <returns><c>true</c> if the level was successfully retrieved; otherwise, <c>false</c>.</returns>
	public static bool TryGetLevel(Map map, string name, bool ignoreCase, out MapLevel level)
	{
		level = GetLevel(map, name, ignoreCase);

		return !level.IsEmpty;
	}

	/// <summary>
	/// Tries to retrieve the level from the specified map by its name.
	/// </summary>
	/// <param name="map">The map containing the levels.</param>
	/// <param name="name">The name of the level to retrieve.</param>
	/// <param name="level">When this method returns, contains the level with the specified name, if found; otherwise, default.</param>
	/// <returns><c>true</c> if the level was successfully retrieved; otherwise, <c>false</c>.</returns>
	public static bool TryGetLevel(Map map, string name, out MapLevel level)
		=> TryGetLevel(map, name, false, out level);


	/// <summary>
	/// Creates an instance of the specified type <typeparamref name="T"/> based on the provided parameters.
	/// </summary>
	/// <typeparam name="T">The type of entity to create.</typeparam>
	/// <param name="name">The name or identifier of the entity.</param>
	/// <param name="args">Optional arguments to pass to the entity's constructor.</param>
	/// <param name="ignoreCase">Optional. Determines whether to ignore case when matching the entity's type name. Default is <c>true</c>.</param>
	/// <returns>An instance of type <typeparamref name="T"/> if creation is successful; otherwise, the default value of <typeparamref name="T"/>.</returns>
	public static T CreateInstance<T>(string name, object[] args, bool ignoreCase = true) where T : Entity
	{
		args ??= Array.Empty<object>();

		var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (var type in assembly.GetTypes())
			{
				if (!string.Equals(type.Name, name, comparison))
					continue;
				if (!typeof(T).IsAssignableFrom(type))
					continue;

				return (T)Activator.CreateInstance(type, args);
			}
		}

		return default;
	}

	/// <summary>
	/// Tries to create an instance of the specified type <typeparamref name="T"/> based on the provided parameters.
	/// </summary>
	/// <typeparam name="T">The type of entity to create.</typeparam>
	/// <param name="name">The name or identifier of the entity.</param>
	/// <param name="args">Optional arguments to pass to the entity's constructor.</param>
	/// <param name="ignoreCase">Determines whether to ignore case when matching the entity's type name.</param>
	/// <param name="value">When this method returns, contains the created instance of type <typeparamref name="T"/>, if creation is successful; otherwise, default.</param>
	/// <returns><c>true</c> if the instance was successfully created; otherwise, <c>false</c>.</returns>
	public static bool TryCreateInstance<T>(string name, object[] args, bool ignoreCase, out T value) where T : Entity
	{
		value = CreateInstance<T>(name, args, ignoreCase);

		return value is not null;
	}

	/// <summary>
	/// Tries to create an instance of the specified type <typeparamref name="T"/> based on the provided parameters.
	/// </summary>
	/// <typeparam name="T">The type of entity to create.</typeparam>
	/// <param name="name">The name or identifier of the entity.</param>
	/// <param name="args">Optional arguments to pass to the entity's constructor.</param>
	/// <param name="value">When this method returns, contains the created instance of type <typeparamref name="T"/>, if creation is successful; otherwise, default.</param>
	/// <returns><c>true</c> if the instance was successfully created; otherwise, <c>false</c>.</returns>
	public static bool TryCreateInstance<T>(string name, object[] args, out T value) where T : Entity
		=> TryCreateInstance<T>(name, args, false, out value);

	/// <summary>
	/// Converts a map location to world coordinates based on the specified size.
	/// </summary>
	/// <param name="location">The location in map coordinates to convert.</param>
	/// <param name="size">The size of the map or tile in world units.</param>
	/// <returns>The world coordinates corresponding to the given map location.</returns>
	public static Vect2 MapToWorld(Vect2 location, Vect2 size)
		=> Vect2.Floor(location * size);

	/// <summary>
	/// Converts a map location to world coordinates based on the specified tile size.
	/// </summary>
	/// <param name="location">The location in map coordinates to convert.</param>
	/// <param name="size">Optional. The size of each tile in world units. Default is 16.</param>
	/// <returns>The world coordinates corresponding to the given map location.</returns>
	public static Vect2 MapToWorld(Vect2 location, int size = 16)
		=> MapToWorld(location, new Vect2(size));

	/// <summary>
	/// Converts world coordinates to map coordinates based on the specified tile size.
	/// </summary>
	/// <param name="position">The world coordinates to convert.</param>
	/// <param name="size">The size of each tile or map unit in world units.</param>
	/// <returns>The map coordinates corresponding to the given world position.</returns>
	public static Vect2 WorldToMap(Vect2 position, Vect2 size)
		=> Vect2.Floor(position / size);

	/// <summary>
	/// Converts world coordinates to map coordinates based on the specified tile size.
	/// </summary>
	/// <param name="location">The world coordinates to convert.</param>
	/// <param name="size">Optional. The size of each tile or map unit in world units. Default is 16.</param>
	/// <returns>The map coordinates corresponding to the given world location.</returns>
	public static Vect2 WorldToMap(Vect2 location, int size = 16)
		=> WorldToMap(location, new Vect2(size));

	/// <summary>
	/// Retrieves the tiles within a specified region based on the given parameters.
	/// </summary>
	/// <param name="position">The position of the region in world coordinates.</param>
	/// <param name="size">The size of the region in world units.</param>
	/// <param name="tileSize">The size of each tile in world units.</param>
	/// <returns>An enumerable collection of tile positions within the specified region.</returns>
	public static IEnumerable<Vect2> GetTilesFromRegion(Vect2 position, Vect2 size, Vect2 tileSize)
	{
		int countX = (int)Math.Max(size.X / tileSize.X, 1); // 2
		int countY = (int)Math.Max(size.Y / tileSize.Y, 1); // 6

		for (int i = 0; i < (countX * countY); i++)
		{
			var location = Vect2.To2D(i, countX);

			yield return location + position / tileSize;
		}
	}

	/// <summary>
	/// Retrieves the tiles within a specified region based on the given parameters.
	/// </summary>
	/// <param name="position">The position of the region in world coordinates.</param>
	/// <param name="size">The size of the region in world units.</param>
	/// <param name="tileSize">Optional. The size of each tile in world units. Default is 16.</param>
	/// <returns>An enumerable collection of tile positions within the specified region.</returns>
	public static IEnumerable<Vect2> GetTilesFromRegion(Vect2 position, Vect2 size, int tileSize = 16)
		=> GetTilesFromRegion(position, size, new Vect2(tileSize));


	internal Map(string filename, byte[] bytes)
	{
		Filename = filename;

		LoadMap(bytes);

		Id = HashHelpers.Hash32($"{filename}{bytes.Length:X8}{Levels.GetHashCode():X8}");
	}

	internal Map(string filename) : this(filename, File.ReadAllBytes(filename)) { }

	private void LoadMap(byte[] bytes)
	{
		Version = string.Empty;
		Tilesets = new Dictionary<int, MapTileset>();
		Levels = new List<MapLevel>();
		GlobalLevels = new Dictionary<string, MapLevel>();
		GlobalLayers = new Dictionary<string, MapLayer>();
		GlobalEntities = new Dictionary<string, MapEntityLayer>();

		var data = JsonSerializer.Deserialize<JsonObject>(bytes);
		var version = (string)data["__header__"]["appVersion"];
		var levels = new List<MapLevel>();

		foreach (var item in data["defs"]["tilesets"].AsArray())
		{
			var tilesetId = (int)item["uid"];
			var tilesetGrid = new Vect2((float)item["__cWid"], (float)item["__cHei"]);
			var tilesetName = (string)item["identifier"];
			var tileFilename = (string)item["relPath"]; // add
			var tilesetSize = new Vect2((float)item["pxWid"], (float)item["pxHei"]);
			var tilesetGridSize = (int)item["tileGridSize"];
			var tilesetSpacing = (int)item["spacing"];
			var tilesetPadding = (int)item["padding"];
			var tilesetTags = item["tags"].AsArray().Select(x => (string)x).ToList();

			Tilesets.Add(tilesetId, new MapTileset(tilesetGrid, tilesetName, tileFilename, tilesetId, tilesetTags, tilesetPadding, tilesetSpacing, tilesetSize, tilesetGridSize));
		}

		foreach (var levelItem in data["levels"].AsArray())
		{
			var levelName = (string)levelItem["identifier"];
			var levelId = (string)levelItem["iid"];
			var levelCoords = new Vect2((int)levelItem["worldX"], (int)levelItem["worldY"]);
			var levelDepth = (int)levelItem["worldDepth"];
			var levelColor = new BoxColor((string)levelItem["__bgColor"]);
			var levelSize = new Vect2((int)levelItem["pxWid"], (int)levelItem["pxHei"]);
			var levelSettings = new Dictionary<string, MapSetting>();
			var layers = new List<MapLayer>();

			string levelNorth = string.Empty, levelEast = string.Empty, levelSouth = string.Empty, levelWest = string.Empty;

			var maxGridWidth = 0;
			var maxGridHeight = 0;

			if (levelItem["fieldInstances"] is not null && levelItem["fieldInstances"].AsArray().Any())
			{
				levelSettings = ProcessSettings(levelItem["fieldInstances"].AsArray(), Tilesets);
			}

			foreach (var nItem in levelItem["__neighbours"].AsArray())
			{
				switch ((string)nItem["dir"])
				{
					case "n":
						levelNorth = (string)nItem["levelIid"];
						break;
					case "e":
						levelEast = (string)nItem["levelIid"];
						break;
					case "s":
						levelSouth = (string)nItem["levelIid"];
						break;
					case "w":
						levelWest = (string)nItem["levelIid"];
						break;
				}
			}

			var neighbours = new MapNeighbour(levelNorth, levelEast, levelSouth, levelWest);

			foreach (var layerItem in levelItem["layerInstances"].AsArray())
			{
				var layerId = (string)layerItem["iid"];
				var layerName = (string)layerItem["__identifier"]; // done
				var layerType = Enum.Parse<MapLayerType>((string)layerItem["__type"]);
				var layerGrid = new Vect2((float)layerItem["__cWid"], (float)layerItem["__cHei"]); // done
				var layerGridSize = (int)layerItem["__gridSize"]; // done
				var layerTilesetFile = layerItem["__tilesetRelPath"] is not null ? (string)layerItem["__tilesetRelPath"] : string.Empty; // add
				var layerOffset = new Vect2((float)layerItem["__pxTotalOffsetX"], (float)layerItem["__pxTotalOffsetY"]); // done
				var layerTileset = layerItem["__tilesetDefUid"] is null ? -1 : (int)layerItem["__tilesetDefUid"]; // done
				var layerInstances = new List<IMapLayer>();

				if (maxGridWidth < layerGrid.X)
					maxGridWidth = (int)layerGrid.X;
				if (maxGridHeight < layerGrid.Y)
					maxGridHeight = (int)layerGrid.Y;

				switch (layerType)
				{
					case MapLayerType.IntGrid:
						{
							var count = 0;

							foreach (var value in layerItem["intGridCsv"].AsArray().Select(x => (int)x))
							{
								var intGridLocation = Vect2.To2D(count, layerGrid.X); // add
								layerInstances.Add(new MapIntGridLayer(value, intGridLocation));

								count++;
							}
						}

						break;

					case MapLayerType.Entities:
						foreach (var entityItem in layerItem["entityInstances"].AsArray())
						{
							var entityId = (string)entityItem["iid"];
							var entityName = (string)entityItem["__identifier"];
							var entityLocation = new Vect2((float)entityItem["__grid"][0], (float)entityItem["__grid"][1]);
							var entityPivot = new Vect2((float)entityItem["__pivot"][0], (float)entityItem["__pivot"][1]);
							var entityTags = entityItem["__tags"].AsArray().Any()
								? entityItem["__tags"].AsArray().Select(x => (string)x).ToList() : new List<string>();
							var entitySize = new Vect2((float)entityItem["width"], (float)entityItem["height"]);
							var entityPosition = new Vect2((float)entityItem["px"][0], (float)entityItem["px"][1]);
							// var entitySettings = ProcessSettings(entityItem["fieldInstances"].AsArray());
							var entitySettings = new Dictionary<string, MapSetting>();
							var entityGrid = new Vect2(layerGridSize);

							if (entityItem["fieldInstances"] is not null && entityItem["fieldInstances"].AsArray().Any())
							{
								entitySettings = ProcessSettings(entityItem["fieldInstances"].AsArray(), Tilesets);
							}

							layerInstances.Add(new MapEntityLayer(entityLocation, entityId, entityName, entityPivot, entityTags, entitySettings, entityPosition, entitySize, entityGrid));
						}
						break;

					case MapLayerType.AutoLayer:
					case MapLayerType.Tiles:
						var instanceType = layerType == MapLayerType.AutoLayer ? "autoLayerTiles" : "gridTiles";

						foreach (var tileItem in layerItem[instanceType].AsArray())
						{
							SurfaceEffects effects = SurfaceEffects.None;

							switch ((int)tileItem["f"])
							{
								case 1:
									effects = SurfaceEffects.FlipHorizontally;
									break;
								case 2:
									effects = SurfaceEffects.FlipVErtically;
									break;
								case 3:
									effects = SurfaceEffects.FlipHorizontally | SurfaceEffects.FlipVErtically;
									break;
							}

							var tileLocation = Vect2.Floor(new Vect2((float)tileItem["px"][0], (float)tileItem["px"][1]) / layerGridSize);
							var tilePosition = new Vect2((float)tileItem["px"][0], (float)tileItem["px"][1]);
							var tileSource = new Rect2(new Vect2((int)tileItem["src"][0], (int)tileItem["src"][1]), new(layerGridSize, layerGridSize));
							var tileIndex = (int)tileItem["t"];

							layerInstances.Add(new MapTileLayer(effects, tilePosition, tileSource, tileIndex, tileLocation));
						}

						break;
				}

				layers.Add(new MapLayer(layerId, layerGrid, layerGridSize, layerName, layerOffset, layerTileset, layerTilesetFile, layerType, new List<IMapLayer>(layerInstances)));

				layerInstances.Clear();
			}

			Levels.Add(new MapLevel(levelColor, levelId, levelName, levelSize, new Vect2(maxGridWidth, maxGridHeight),
				levelDepth, levelCoords, levelSettings, new List<MapLayer>(layers), new MapNeighbour(levelNorth, levelEast, levelSouth, levelWest)));
		}

		// Map = new Map(filename, version, tilesets, levels);

		foreach (MapLevel level in Levels)
		{
			GlobalLevels.Add(level.Id, level);

			foreach (MapLayer layer in level.Layers)
			{
				if (layer.Type == MapLayerType.Entities)
				{
					foreach (var entity in layer.InstanceAs<MapEntityLayer>())
					{
						var item = entity;

						item.Layer = layer;

						GlobalEntities.Add(entity.Id, entity);
					}
				}

				if (layer.Type == MapLayerType.Tiles)
				{
					foreach (var entity in layer.InstanceAs<MapTileLayer>())
					{
						var item = entity;

						item.Layer = layer;
					}
				}

				GlobalLayers.Add(layer.Id, layer);
			}
		}
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
	/// </summary>
	public void Dispose()
	{
		if (IsDisposed)
			return;

		IsDisposed = true;
	}



	private Dictionary<string, MapSetting> ProcessSettings(JsonArray settings, Dictionary<int, MapTileset> tilesets)
	{
		var result = new Dictionary<string, MapSetting>();

		foreach (var s in settings)
		{
			var type = (string)s["__type"];
			var name = (string)s["__identifier"];

			switch (type)
			{
				case var _ when type.StartsWith("Int"):
					result.Add(name, new IntSetting((int)s["__value"])); break;

				case var _ when type.StartsWith("Float"):
					result.Add(name, new FloatSetting((float)s["__value"])); break;

				case var _ when type.StartsWith("Bool"):
					result.Add(name, new BoolSetting((bool)s["__value"])); break;

				case var _ when type.StartsWith("String"): // Strubg + Multistring
					result.Add(name, new StringSetting(s["__value"] is null ? string.Empty : (string)s["__value"])); break;

				case var _ when type.StartsWith("Color"):
					result.Add(name, new ColorSetting(new BoxColor((string)s["__value"]))); break;

				case var _ when type.StartsWith("Point"):
					{
						var value = s["__value"];

						if (value is not null)
							result.Add(name, new PointSetting(new Vect2((int)value["cx"], (int)value["cy"])));
						else
							result.Add(name, new PointSetting(Vect2.Zero));
					}
					break;

				case var _ when type.StartsWith("FilePath"):
					result.Add(name, new FilepathSetting(s["__value"] is null ? string.Empty : (string)s["__value"])); break;

				case var _ when type.StartsWith("LocalEnum"):
					result.Add(name, new EnumSetting(s["__value"] is null ? string.Empty : (string)s["__value"])); break;

				case var _ when type.StartsWith("EntityRef"):
					{
						var value = s["__value"];

						if (value is not null)
						{
							result.Add(name, new EntityRefSetting(new MapEntityRef(
								(string)value["entityIid"], (string)value["layerIid"], (string)value["levelIid"], (string)value["worldIid"]
							)));
						}
					}
					break;

				case var _ when type.StartsWith("Tile"):
					{
						if (s["__value"] is not null)
						{
							var tilesetId = (int)s["__value"]["tilesetUid"];
							var tilesetFile = tilesets[tilesetId].Filename;
							var x = (int)s["__value"]["x"];
							var y = (int)s["__value"]["y"];
							var w = (int)s["__value"]["w"];
							var h = (int)s["__value"]["h"];

							result.Add(name, new TileSetting(new MapTile(tilesetId, tilesetFile, new(x, y, w, h))));
						}
					}
					break;

				//
				// ### Array Version:
				//

				case var _ when type.StartsWith("Array<Int>"):
					{
						var array = s["__value"].AsArray().Select(x => (int)x).ToList();
						result.Add(name, new IntArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<Float>"):
					{
						var array = s["__value"].AsArray().Select(x => (float)x).ToList();
						result.Add(name, new FloatArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<Bool>"):
					{
						var array = s["__value"].AsArray().Select(x => (bool)x).ToList();
						result.Add(name, new BoolArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<String>"):
					{
						var array = s["__value"].AsArray().Select(x => x is null ? string.Empty : (string)x).ToList();
						result.Add(name, new StringArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<Color>"):
					{
						var array = s["__value"].AsArray().Select(x => new BoxColor((string)x)).ToList();
						result.Add(name, new ColorArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<Point>"):
					{
						var value = s["__value"].AsArray();

						var array = value
							.Where(x => x is not null)
							.Select(x => new Vect2((int)x["cx"], (int)x["cy"]))
							.ToList();

						result.Add(name, new PointArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<FilePath>"):
					{
						var value = s["__value"].AsArray();

						var array = value
							.Where(x => x is not null)
							.Select(x => (string)x)
							.ToList();

						result.Add(name, new FilepathArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<LocalEnum."):
					{
						var value = s["__value"].AsArray();

						var array = value
							.Where(x => x is not null)
							.Select(x => (string)x)
							.ToList();

						result.Add(name, new EnumArraySetting(array));
					}
					break;

				case var _ when type.StartsWith("Array<EntityRef>"):
					{
						var value = s["__value"].AsArray();

						if (!value.IsEmpty())
						{
							var array = value
								.Where(x => x is not null)
								.Select(x => new MapEntityRef(
									(string)x["entityIid"], (string)x["layerIid"], (string)x["levelIid"], (string)x["worldIid"])
								)
								.ToList();

							result.Add(name, new EntityRefArraySetting(array));
						}
					}
					break;

				case var _ when type.StartsWith("Array<Tile>"):
					{
						var value = s["__value"].AsArray();

						if (!value.IsEmpty())
						{
							var array = value
								.Where(x => x is not null)
								.Select(x => new MapTile(
									(int)x["tilesetUid"], tilesets[(int)x["tilesetUid"]].Filename, new((int)x["x"], (int)x["y"], (int)x["w"], (int)x["h"])
								))
								.ToList();

							result.Add(name, new TileArraySetting(array));
						}
					}
					break;
			}
		}

		return result;
	}

	/// <summary>
	/// Initializes the object or class instance.
	/// </summary>
	public readonly void Initialize() { }
}
