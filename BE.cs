using Box.Graphics.Batch;
using Box.Services;
using Box.Services.Types;

namespace Box;

public static class BE
{
	/// <summary>
	/// Gets the current viewport width from the renderer.
	/// </summary>
	public static int ViewportWidth = Renderer.Width;

	/// <summary>
	/// Gets the current viewport height from the renderer.
	/// </summary>
	public static int ViewportHeight = Renderer.Height;

	/// <summary>
	/// Gets the current viewport size (width and height) from the renderer.
	/// </summary>
	public static Vect2 ViewportSize => Renderer.Size;

	/// <summary>
	/// Gets the center point of the current viewport from the renderer.
	/// </summary>
	public static Vect2 ViewportCenter => Renderer.Center;

	/// <summary>
	/// Computes a 32-bit hash for the specified string name.
	/// </summary>
	/// <param name="name">The string to hash.</param>
	/// <returns>A 32-bit hash code.</returns>
	public static uint Hash(string name) => HashHelpers.Hash32(name);

	/// <summary>
	/// Computes a 32-bit hash for the specified enum name.
	/// </summary>
	/// <param name="name">The enum value to hash, using its string representation.</param>
	/// <returns>A 32-bit hash code.</returns>
	public static uint Hash(Enum name) => HashHelpers.Hash32(name.ToEnumString());

	/// <summary>
	/// Adds a surface asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the surface.</param>
	/// <param name="filename">The file path of the surface image.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSurface(string name, string filename, bool repeat = false, bool smooth = false)
		=> Assets.AddSurface(name, filename, repeat, smooth);

	/// <summary>
	/// Adds a surface asset to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the surface.</param>
	/// <param name="filename">The file path of the surface image.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSurface(Enum name, string filename, bool repeat = false, bool smooth = false)
		=> Assets.AddSurface(name, filename, repeat, smooth);

	/// <summary>
	/// Adds a bitmap font asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the font.</param>
	/// <param name="filename">The file path of the font file.</param>
	/// <param name="spacing">The character spacing override.</param>
	/// <param name="lineSpacing">The line spacing override.</param>
	public static void AddBitmapFont(string name, string filename, int spacing = 0, int lineSpacing = 0)
		=> Assets.AddBitmapFont(name, filename, spacing, lineSpacing);

	/// <summary>
	/// Adds a bitmap font asset to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the font.</param>
	/// <param name="filename">The file path of the font file.</param>
	/// <param name="spacing">The character spacing override.</param>
	/// <param name="lineSpacing">The line spacing override.</param>
	public static void AddBitmapFont(Enum name, string filename, int spacing = 0, int lineSpacing = 0)
		=> Assets.AddBitmapFont(name, filename, spacing, lineSpacing);

	/// <summary>
	/// Adds a subsurface (region of a texture) to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the subsurface.</param>
	/// <param name="filename">The file path of the source image.</param>
	/// <param name="region">The rectangular region to extract.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSubSurface(string name, string filename, Rect2 region, bool repeat = false, bool smooth = false)
		=> Assets.AddSubSurface(name, filename, region, repeat, smooth);

	/// <summary>
	/// Adds a subsurface (region of a texture) to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the subsurface.</param>
	/// <param name="filename">The file path of the source image.</param>
	/// <param name="region">The rectangular region to extract.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSubSurface(Enum name, string filename, Rect2 region, bool repeat = false, bool smooth = false)
		=> Assets.AddSubSurface(name, filename, region, repeat, smooth);

	/// <summary>
	/// Adds a subsurface from an existing surface and sprite sheet.
	/// </summary>
	/// <param name="name">The key name for the subsurface.</param>
	/// <param name="surface">The source surface object.</param>
	/// <param name="sheet">The sprite sheet containing the region.</param>
	/// <param name="sheetName">The name of the region in the sprite sheet.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSubSurface(string name, Surface surface, Spritesheet sheet, string sheetName, bool repeat = false, bool smooth = false)
		=> Assets.AddSubSurface(name, surface, sheet, sheetName, repeat, smooth);

	/// <summary>
	/// Adds a subsurface from an existing surface and sprite sheet using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the subsurface.</param>
	/// <param name="surface">The source surface object.</param>
	/// <param name="sheet">The sprite sheet containing the region.</param>
	/// <param name="sheetName">The name of the region in the sprite sheet.</param>
	/// <param name="repeat">Whether the texture should repeat when tiled.</param>
	/// <param name="smooth">Whether smoothing should be applied to the texture.</param>
	public static void AddSubSurface(Enum name, Surface surface, Spritesheet sheet, string sheetName, bool repeat = false, bool smooth = false)
		=> Assets.AddSubSurface(name, surface, sheet, sheetName, repeat, smooth);

	/// <summary>
	/// Adds a sound asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the sound.</param>
	/// <param name="filename">The file path of the sound file.</param>
	/// <param name="looped">Whether the sound should loop when played.</param>
	public static void AddSound(string name, string filename, bool looped = false)
		=> Assets.AddSound(name, filename, looped);

	/// <summary>
	/// Adds a sound asset to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the sound.</param>
	/// <param name="filename">The file path of the sound file.</param>
	/// <param name="looped">Whether the sound should loop when played.</param>
	public static void AddSound(Enum name, string filename, bool looped = false)
		=> Assets.AddSound(name, filename, looped);

	/// <summary>
	/// Adds a map asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the map.</param>
	/// <param name="filename">The file path of the map file.</param>
	public static void AddMap(string name, string filename)
		=> Assets.AddMap(name, filename);

	/// <summary>
	/// Adds a map asset to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the map.</param>
	/// <param name="filename">The file path of the map file.</param>
	public static void AddMap(Enum name, string filename)
		=> Assets.AddMap(name, filename);

	/// <summary>
	/// Adds a TrueType font asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the font.</param>
	/// <param name="filename">The file path of the font file.</param>
	/// <param name="size">The font size.</param>
	/// <param name="useSmoothing">Whether smoothing should be applied to the font rendering.</param>
	/// <param name="bold">Whether the font should be rendered in bold.
	/// </param>
	/// <param name="thickness">The outline thickness.
	/// </param>
	/// <param name="spacing">The character spacing override.</param>
	/// <param name="lineSpacing">The line spacing override.</param>
	public static void AddFont(string name, string filename, int size, bool useSmoothing = false, bool bold = false, int thickness = 0,
		int spacing = 0, int lineSpacing = 0)
			=> Assets.AddFont(name, filename, size, useSmoothing, bold, thickness, spacing, lineSpacing);

	/// <summary>
	/// Adds a TrueType font asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the font.</param>
	/// <param name="filename">The file path of the font file.</param>
	/// <param name="size">The font size.</param>
	/// <param name="useSmoothing">Whether smoothing should be applied to the font rendering.</param>
	/// <param name="bold">Whether the font should be rendered in bold.
	/// </param>
	/// <param name="thickness">The outline thickness.
	/// </param>
	/// <param name="spacing">The character spacing override.</param>
	/// <param name="lineSpacing">The line spacing override.</param>
	public static void AddFont(Enum name, string filename, int size, bool useSmoothing = false, bool bold = false, int thickness = 0,
		int spacing = 0, int lineSpacing = 0)
			=> Assets.AddFont(name, filename, size, useSmoothing, bold, thickness, spacing, lineSpacing);

	/// <summary>
	/// Adds a sprite sheet asset to the asset manager.
	/// </summary>
	/// <param name="name">The key name for the sheet.</param>
	/// <param name="filename">The file path of the sprite sheet image.</param>
	public static void AddSheet(string name, string filename)
		=> Assets.AddSheet(name, filename);

	/// <summary>
	/// Adds a sprite sheet asset to the asset manager using an enum key.
	/// </summary>
	/// <param name="name">The enum key for the sheet.</param>
	/// <param name="filename">The file path of the sprite sheet image.</param>
	public static void AddSheet(Enum name, string filename)
		=> Assets.AddSheet(name, filename);

	/// <summary>
	/// Aligns the child width relative to the parent width using the specified horizontal alignment and offset.
	/// </summary>
	/// <param name="parent">The width of the parent.</param>
	/// <param name="child">The width of the child.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="offset">Additional horizontal offset to apply.</param>
	/// <returns>The calculated horizontal position for alignment.</returns>
	public static float AlignWidth(float parent, float child, HAlign hAlign, float offset)
	{
		return hAlign switch
		{
			HAlign.Center => Vect2.Center(parent, child, true) + offset,
			HAlign.Right => parent - child + offset,
			_ => 0 + offset,
		};
	}

	/// <summary>
	/// Aligns the child width relative to the parent width using the specified horizontal alignment.
	/// </summary>
	/// <param name="parent">The width of the parent.</param>
	/// <param name="child">The width of the child.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <returns>The calculated horizontal position for alignment.</returns>
	public static float AlignWidth(float parent, float child, HAlign hAlign)
		=> AlignWidth(parent, child, hAlign, 0f);



	/// <summary>
	/// Aligns the child height relative to the parent height using the specified vertical alignment and offset.
	/// </summary>
	/// <param name="parent">The height of the parent.</param>
	/// <param name="child">The height of the child.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <param name="offset">Additional vertical offset to apply.</param>
	/// <returns>The calculated vertical position for alignment.</returns>
	public static float AlignHeight(float parent, float child, VAlign vAlign, float offset)
	{
		return vAlign switch
		{
			VAlign.Center => Vect2.Center(parent, child, true) + offset,
			VAlign.Bottom => parent - child + offset,
			_ => 0 + offset,
		};
	}

	/// <summary>
	/// Aligns the child height relative to the parent height using the specified vertical alignment.
	/// </summary>
	/// <param name="parent">The height of the parent.</param>
	/// <param name="child">The height of the child.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>The calculated vertical position for alignment.</returns>
	public static float AlignHeight(float parent, float child, VAlign vAlign)
		=> AlignHeight(parent, child, vAlign, 0f);

	/// <summary>
	/// Aligns the position of the child entity relative to the size and position of the parent entity, based on the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="parent">The parent entity whose size and position are used for alignment.</param>
	/// <param name="child">The entity to be aligned.</param>
	/// <param name="vAlign">The vertical alignment type (Top, Center, Bottom).</param>
	/// <param name="hAlign">The horizontal alignment type (Left, Center, Right).</param>
	/// <param name="offset">Optional vector offset to apply after alignment (default is null).</param>
	public static void AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign, Vect2 offset)
		=> child.Position = AlignmentHelpers.AlignToEntity(parent, child, hAlign, vAlign, offset);

	/// <summary>
	/// Aligns the position of the child entity relative to the size and position of the parent entity, based on the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="parent">The parent entity whose size and position are used for alignment.</param>
	/// <param name="child">The entity to be aligned.</param>
	/// <param name="hAlign">The horizontal alignment type (Left, Center, Right).</param>
	/// <param name="vAlign">The vertical alignment type (Top, Center, Bottom).</param>
	public static void AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign)
		=> child.Position = AlignmentHelpers.AlignToEntity(parent, child, hAlign, vAlign);



	/// <summary>
	/// Aligns the position of the entity relative to the viewport (Renderer), based on 
	/// the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="entity">The entity to align.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <param name="offset">Additional offset to apply.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static void AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign, Vect2 offset)
		=> entity.Position = AlignmentHelpers.AlignToRenderer(entity, hAlign, vAlign, offset);

	/// <summary>
	/// Aligns the position of the entity relative to the viewport (Renderer), based on the 
	/// specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="entity">The entity to align.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static void AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign)
		=> entity.Position = AlignmentHelpers.AlignToRenderer(entity, hAlign, vAlign);


	public static float SafeRegion => Renderer.SafeRegion;

	/// <summary>
	/// Provides access to the ScreenManager singleton instance.
	/// </summary>
	public static ScreenManager ScreenManager => ScreenManager.Instance;

	/// <summary>
	/// Provides access to the Assets singleton instance.
	/// </summary>
	public static Assets Assets => Assets.Instance;

	/// <summary>
	/// Provides access to the Engine singleton instance.
	/// </summary>
	public static Engine Engine => Engine.Instance;

	/// <summary>
	/// Provides access to the InputMap instance from the Engine.
	/// </summary>
	public static InputMap Input => Engine.Input;

	/// <summary>
	/// Provides access to the Clock singleton instance.
	/// </summary>
	public static Clock Clock => Clock.Instance;

	/// <summary>
	/// Provides access to the Signal singleton instance.
	/// </summary>
	public static Signal Signal => Signal.Instance;

	/// <summary>
	/// Provides access to the Coroutine singleton instance.
	/// </summary>
	public static Coroutine Coroutine => Coroutine.Instance;

	/// <summary>
	/// Provides access to the Renderer singleton instance.
	/// </summary>
	public static Renderer Renderer => Renderer.Instance;

	/// <summary>
	/// Provides access to the Rand singleton instance.
	/// </summary>
	public static FastRandom Rand => FastRandom.Instance;

	/// <summary>
	/// Provides access to the SoundManager singleton instance.
	/// </summary>
	public static SoundManager SoundManager => SoundManager.Instance;

	/// <summary>
	/// Provides access to the Log singleton instance.
	/// </summary>
	public static Log Log => Log.Instance;

	/// <summary>
	/// Retrieves the singleton instance of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the singleton to retrieve.</typeparam>
	/// <returns>The singleton instance of the specified type.</returns>
	public static T GetService<T>() where T : GameService => ServiceManager.Instance.GetService<T>();

	/// <summary>
	/// Retrieves a Surface asset by its name.
	/// </summary>
	/// <param name="name">The name of the Surface asset to retrieve.</param>
	/// <returns>The Surface asset associated with the specified name.</returns>
	public static Surface GetSurface(string name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Retrieves a Surface asset by its name.
	/// </summary>
	/// <param name="name">The name of the Surface asset to retrieve.</param>
	/// <returns>The Surface asset associated with the specified name.</returns>
	public static Surface GetSurface(Enum name) => Assets.Get<Surface>(name);

	/// <summary>
	/// Loads a Surface asset from a file.
	/// </summary>
	/// <param name="filename">The filename of the Surface asset to load.</param>
	/// <returns>The Surface asset loaded from the specified file.</returns>
	public static Surface GetSurfaceFromFile(string filename) => Assets.GetFromFile<Surface>(filename);

	/// <summary>
	/// Retrieves a Map asset by its name.
	/// </summary>
	/// <param name="name">The name of the Map asset to retrieve.</param>
	/// <returns>The Map asset associated with the specified name.</returns>
	public static Map GetMap(string name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Map asset by its name.
	/// </summary>
	/// <param name="name">The name of the Map asset to retrieve.</param>
	/// <returns>The Map asset associated with the specified name.</returns>
	public static Map GetMap(Enum name) => Assets.Get<Map>(name);

	/// <summary>
	/// Retrieves a Sound asset by its name.
	/// </summary>
	/// <param name="name">The name of the Sound asset to retrieve.</param>
	/// <returns>The Sound asset associated with the specified name.</returns>
	public static Sound GetSound(string name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Sound asset by its name.
	/// </summary>
	/// <param name="name">The name of the Sound asset to retrieve.</param>
	/// <returns>The Sound asset associated with the specified name.</returns>
	public static Sound GetSound(Enum name) => Assets.Get<Sound>(name);

	/// <summary>
	/// Retrieves a Font asset by its name.
	/// </summary>
	/// <param name="name">The name of the Font asset to retrieve.</param>
	/// <returns>The Font asset associated with the specified name.</returns>
	public static BoxFont GetFont(string name) => Assets.GetFont(name);

	/// <summary>
	/// Retrieves a Font asset by its name.
	/// </summary>
	/// <param name="name">The name of the Font asset to retrieve.</param>
	/// <returns>The Font asset associated with the specified name.</returns>
	public static BoxFont GetFont(Enum name) => Assets.GetFont(name);

	/// <summary>
	/// Retrieves a Spritesheet asset by its name.
	/// </summary>
	/// <param name="name">The name of the Spritesheet asset to retrieve.</param>
	/// <returns>The Spritesheet asset associated with the specified name.</returns>
	public static Spritesheet GetSheet(string name) => Assets.Get<Spritesheet>(name);

	/// <summary>
	/// Retrieves a Spritesheet asset by its name.
	/// </summary>
	/// <param name="name">The name of the Spritesheet asset to retrieve.</param>
	/// <returns>The Spritesheet asset associated with the specified name.</returns>
	public static Spritesheet GetSheet(Enum name) => Assets.Get<Spritesheet>(name);

	/// <summary>
	/// Loads a bitmap font from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the bitmap font.</param>
	/// <param name="spacing">The spacing between characters. Default is 0.</param>
	/// <param name="linespacing">The spacing between lines of text. Default is 0.</param>
	/// <returns>A <see cref="BitmapFont"/> object representing the loaded font.</returns>
	public static BitmapFont LoadBitmapFont(string path, int spacing = 0, int lineSpacing = 0)
		=> Assets.LoadBitmapFont(path, spacing, lineSpacing);

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
	public static GenericFont LoadFont(string path, int size, bool useSmoothing = false, bool bold = false,
	int thickness = 0, int spacing = 0, int lineSpacing = 0) =>
		Assets.LoadFont(path, size, useSmoothing, bold, thickness, spacing, lineSpacing);

	/// <summary>
	/// Loads a map from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the map.</param>
	/// <returns>A <see cref="Map"/> object representing the loaded map.</returns>
	public static Map LoadMap(string path) => Assets.LoadMap(path);

	/// <summary>
	/// Loads a pack of assets from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the asset pack.</param>
	public static void LoadPack(string path) => Assets.LoadPack(path);

	/// <summary>
	/// Loads a sound from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the sound.</param>
	/// <returns>A <see cref="Sound"/> object representing the loaded sound.</returns>
	public static Sound LoadSound(string path) => Assets.LoadSound(path);

	public static Sound LoadSound(string filename, bool looped = false) =>
		Assets.LoadSound(filename, looped);

	/// <summary>
	/// Loads a sprite sheet from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the sprite sheet.</param>
	/// <returns>A <see cref="Spritesheet"/> object representing the loaded sprite sheet.</returns>
	public static Spritesheet LoadSpriteSheet(string path) => Assets.LoadSpriteSheet(path);

	/// <summary>
	/// Loads a subsection of a surface (texture) from the specified file path and region.
	/// </summary>
	/// <param name="path">The file path to the texture.</param>
	/// <param name="region">The rectangular region of the surface to load.</param>
	/// <param name="repeat">Whether the texture should repeat. Default is false.</param>
	/// <param name="smooth">Whether to use smoothing on the texture. Default is false.</param>
	/// <returns>A <see cref="Surface"/> object representing the loaded subsection.</returns>
	public static Surface LoadSubSurface(string path, Rect2 region, bool repeat = false, bool smooth = false) =>
		Assets.LoadSubSurface(path, region, repeat, smooth);

	/// <summary>
	/// Loads a subsurface from a given spritesheet.
	/// </summary>
	/// <param name="surface">The base <see cref="Surface"/> to extract the subsurface from.</param>
	/// <param name="sheet">The <see cref="Spritesheet"/> containing the subsurface definition.</param>
	/// <param name="name">The name of the subsurface to load.</param>
	/// <param name="repeat">Optional. Specifies whether the subsurface should repeat when rendered. Defaults to <c>false</c>.</param>
	/// <param name="smooth">Optional. Specifies whether smoothing should be applied to the subsurface. Defaults to <c>false</c>.</param>
	/// <returns>The loaded <see cref="Surface"/> representing the subsurface.</returns>
	/// <remarks>
	/// This method simplifies the process of extracting and managing subsurfaces from a spritesheet,
	/// leveraging the <see cref="Assets.LoadSubSurface"/> method for asset management.
	/// </remarks>
	public static Surface LoadSubSurface(Surface surface, Spritesheet sheet, string name, bool repeat = false,
	bool smooth = false) => Assets.LoadSubSurface(surface, sheet, name, repeat, smooth);

	/// <summary>
	/// Loads a surface (texture) from the specified file path.
	/// </summary>
	/// <param name="path">The file path to the texture.</param>
	/// <param name="repeat">Whether the texture should repeat. Default is false.</param>
	/// <param name="smooth">Whether to use smoothing on the texture. Default is false.</param>
	/// <returns>A <see cref="Surface"/> object representing the loaded texture.</returns>
	public static Surface LoadSurface(string path, bool repeat = false, bool smooth = false)
		=> Assets.LoadSurface(path, repeat, smooth);
}
