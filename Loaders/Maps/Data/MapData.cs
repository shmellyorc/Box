namespace Box.Loaders.Maps.Data;

internal sealed class MapData
{
	public Map Map { get; }
	public string Filename { get; } // add

	public MapData(string filename, JsonObject data)
	{
		Filename = filename;

		var version = (string)data["__header__"]["appVersion"];
		var tilesets = new Dictionary<int, MapTileset>();
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

			tilesets.Add(tilesetId, new MapTileset(tilesetGrid, tilesetName, tileFilename, tilesetId, tilesetTags, tilesetPadding, tilesetSpacing, tilesetSize, tilesetGridSize));
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
				levelSettings = ProcessSettings(levelItem["fieldInstances"].AsArray(), tilesets);
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
								entitySettings = ProcessSettings(entityItem["fieldInstances"].AsArray(), tilesets);
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

			levels.Add(new MapLevel(levelColor, levelId, levelName, levelSize, new Vect2(maxGridWidth, maxGridHeight),
				levelDepth, levelCoords, levelSettings, new List<MapLayer>(layers), new MapNeighbour(levelNorth, levelEast, levelSouth, levelWest)));
		}

		// Map = new Map(filename, version, tilesets, levels);
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
}
