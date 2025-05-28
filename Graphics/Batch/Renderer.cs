using Box.Services.Types;

namespace Box.Graphics.Batch;

/// <summary>
/// Manages rendering operations for shapes and surfaces within a viewport.
/// </summary>
public sealed class Renderer
{
	private readonly Dictionary<(SFMLTexture Texture, SFMLPrimitiveType Type, SFMLBlendMode Blend, int ZOrder),
		List<SFMLVertex>> _groupedVerts = new();
	private Rect2 _viewport;
	private bool _isRunning;
	private int _batchIndex, _batchCount;
	private Screen _screen;

	#region Properties
	/// <summary>
	/// Gets the size of the viewport.
	/// </summary>
	public Vect2 Size => _viewport.Size;

	/// <summary>
	/// Gets the width of the viewport.
	/// </summary>
	public int Width => (int)_viewport.Size.X;

	/// <summary>
	/// Gets the height of the viewport.
	/// </summary>
	public int Height => (int)_viewport.Size.Y;

	/// <summary>
	/// Gets the center coordinates of the viewport.
	/// </summary>
	public Vect2 Center => _viewport.Size / 2;

	/// <summary>
	/// Gets the singleton instance of the Renderer class.
	/// </summary>
	public static Renderer Instance { get; private set; }

	/// <summary>
	/// Gets the current count.
	/// </summary>
	public int Count { get; private set; }

	/// <summary>
	/// Gets the batch count.
	/// </summary>
	public int BatchCount { get; private set; }

	/// <summary>
	/// Gets the safe region value from the engine settings.
	/// </summary>
	public float SafeRegion => EngineSettings.Instance.SafeRegion;
	#endregion



	#region Constructor
	internal Renderer()
	{
		Instance ??= this;

		_viewport = new Rect2(Vect2.Zero, EngineSettings.Instance.Viewport);
	}
	#endregion


	#region Alignment
	/// <summary>
	/// Aligns the child position vertically relative to the parent position based on the specified alignment and optional offset.
	/// </summary>
	/// <param name="parent">The position of the parent.</param>
	/// <param name="child">The position of the child.</param>
	/// <param name="align">The vertical alignment type (Top, Center, Bottom).</param>
	/// <param name="offset">Optional offset applied after alignment (default is 0).</param>
	/// <returns>The aligned position of the child relative to the parent.</returns>
	public float AlignHeight(float parent, float child, VAlign align, float offset)
		=> AlignmentHelpers.AlignHeight(parent, child, align, offset);

	/// <summary>
	/// Aligns the child position vertically relative to the parent position based on the specified alignment and optional offset.
	/// </summary>
	/// <param name="parent">The position of the parent.</param>
	/// <param name="child">The position of the child.</param>
	/// <param name="align">The vertical alignment type (Top, Center, Bottom).</param>
	/// <returns>The aligned position of the child relative to the parent.</returns>
	public float AlignHeight(float parent, float child, VAlign align)
		=> AlignmentHelpers.AlignHeight(parent, child, align);



	/// <summary>
	/// Aligns the child position horizontally relative to the parent position based on the specified alignment and optional offset.
	/// </summary>
	/// <param name="parent">The position of the parent.</param>
	/// <param name="child">The position of the child.</param>
	/// <param name="align">The horizontal alignment type (Left, Center, Right).</param>
	/// <param name="offset">Optional offset applied after alignment (default is 0).</param>
	/// <returns>The aligned position of the child relative to the parent.</returns>
	public float AlignWidth(float parent, float child, HAlign align, float offset)
		=> AlignmentHelpers.AlignWidth(parent, child, align, offset);

	/// <summary>
	/// Aligns the child position horizontally relative to the parent position based on the specified alignment and optional offset.
	/// </summary>
	/// <param name="parent">The position of the parent.</param>
	/// <param name="child">The position of the child.</param>
	/// <param name="align">The horizontal alignment type (Left, Center, Right).</param>
	/// <returns>The aligned position of the child relative to the parent.</returns>
	public float AlignWidth(float parent, float child, HAlign align)
		=> AlignmentHelpers.AlignWidth(parent, child, align);



	/// <summary>
	/// Aligns the position of the child entity relative to the size and position of the parent entity, based on the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="parent">The parent entity whose size and position are used for alignment.</param>
	/// <param name="child">The entity to be aligned.</param>
	/// <param name="vAlign">The vertical alignment type (Top, Center, Bottom).</param>
	/// <param name="hAlign">The horizontal alignment type (Left, Center, Right).</param>
	/// <param name="offset">Optional vector offset to apply after alignment (default is null).</param>
	public void AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign, Vect2 offset)
		=> child.Position = AlignmentHelpers.AlignToEntity(parent, child, hAlign, vAlign, offset);

	/// <summary>
	/// Aligns the position of the child entity relative to the size and position of the parent entity, based on the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="parent">The parent entity whose size and position are used for alignment.</param>
	/// <param name="child">The entity to be aligned.</param>
	/// <param name="hAlign">The horizontal alignment type (Left, Center, Right).</param>
	/// <param name="vAlign">The vertical alignment type (Top, Center, Bottom).</param>
	public void AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign)
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
	public void AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign, Vect2 offset)
		=> entity.Position = AlignmentHelpers.AlignToRenderer(entity, hAlign, vAlign, offset);

	/// <summary>
	/// Aligns the position of the entity relative to the viewport (Renderer), based on the 
	/// specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="entity">The entity to align.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public void AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign)
		=> entity.Position = AlignmentHelpers.AlignToRenderer(entity, hAlign, vAlign);
	#endregion


	#region Begin, End and Flush
	internal void Begin(Screen screen)
	{
		if (_isRunning)
			return;

		_screen = screen;

		if (BatchCount != _batchCount)
			BatchCount = _batchCount;
		if (Count != _batchCount)
			Count = _batchCount;

		_groupedVerts.Clear();
		_batchIndex = 0;
		_batchCount = 0;

		// Engine.Instance._window.SetView(screen.Camera.ToSFML());
		Engine.Instance.RenderTexture.SetView(screen.Camera.ToSFML());

		_isRunning = true;
	}

	internal void End()
	{
		if (!_isRunning)
			return;

		Flush();

		_isRunning = false;
	}

	internal void Flush()
	{
		if (_groupedVerts.Count == 0)
			return;

		var sorted = _groupedVerts.OrderBy(pair => pair.Key.ZOrder);

		foreach (var (key, verts) in sorted)
		{
			if (verts.Count == 0)
				continue;

			var (texture, primitive, blend, _) = key;

			// Create a temp buffer specific to this draw group
			using var vbuf = new SFMLVertexBuffer((uint)verts.Count, primitive, SFMLVertexBuffer.UsageSpecifier.Stream);
			vbuf.Update(verts.ToArray());

			Engine.Instance.RenderTexture.Draw(vbuf, new SFMLRenderStates
			{
				Texture = texture,
				BlendMode = blend,
				Transform = SFMLTransform.Identity,
			});
		}

		_groupedVerts.Clear();
		_batchCount++;
	}
	#endregion


	#region Draw Commands
	/// <summary>
	/// Draws the entire surface at the specified position with the given color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="position">The position to draw the surface.</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Vect2 position, BoxColor color)
		=> EngineBatchDraw(
			dstRect: new Rect2(position, surface.Size),
			srcRect: new Rect2(Vect2.Zero, surface.Size),
			texture: surface.ToSFML(),
			color: color,
			effects: SurfaceEffects.None,
			origin: Vect2.Zero,
			rotation: 0f,
			scale: Vect2.One
		);

	/// <summary>
	/// Draws a region of the surface within the specified rectangle with the given color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="region">The region within the surface to draw.</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Rect2 region, BoxColor color)
		=> EngineBatchDraw(
			dstRect: new Rect2(region.Position, region.Size),
			srcRect: new Rect2(Vect2.Zero, region.Size),
			texture: surface.ToSFML(),
			color: color,
			effects: SurfaceEffects.None,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One
		);

	/// <summary>
	/// Draws a region of the surface within the specified rectangle with the given color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="region">The region within the surface to draw.</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Rect2 region, BoxColor color, int zOrder)
		=> EngineBatchDraw(
			dstRect: new Rect2(region.Position, region.Size),
			srcRect: new Rect2(Vect2.Zero, region.Size),
			texture: surface.ToSFML(),
			color: color,
			effects: SurfaceEffects.None,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One,
			zOrder
		);

	/// <summary>
	/// Draws the entire surface at the specified position with the specified effects and color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="position">The position to draw the surface.</param>
	/// <param name="effects">The effects to apply when drawing the surface (e.g., rotation, scaling).</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Vect2 position, SurfaceEffects effects, BoxColor color)
		=> EngineBatchDraw(
			dstRect: new Rect2(position, surface.Size),
			srcRect: new Rect2(Vect2.Zero, surface.Size),
			texture: surface.ToSFML(),
			color: color,
			effects: effects,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One
		);

	/// <summary>
	/// Draws a portion of the surface defined by srcRect at the specified position with the given color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="position">The position to draw the surface.</param>
	/// <param name="srcRect">The source rectangle defining the portion of the surface to draw.</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Vect2 position, Rect2 srcRect, BoxColor color)
		=> EngineBatchDraw(
			dstRect: new Rect2(position, srcRect.Size),
			srcRect: srcRect,
			texture: surface.ToSFML(),
			color: color,
			effects: SurfaceEffects.None,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One
		);

	/// <summary>
	/// Draws a portion of a surface (defined by <paramref name="srcRect"/>) at the specified screen position, applying optional effects, color tint, and z-order for layering.
	/// </summary>
	/// <param name="surface">The surface (texture) to draw.</param>
	/// <param name="position">The screen position where the surface will be drawn.</param>
	/// <param name="srcRect">The source rectangle in texture space defining which part of the surface to draw.</param>
	/// <param name="effects">Optional surface effects such as flipping, rotation, or scaling.</param>
	/// <param name="color">The color tint to apply to the drawn surface.</param>
	/// <param name="zOrder">The z-order used to control draw order layering. Higher values are drawn above lower ones.</param>
	public void Draw(Surface surface, Vect2 position, Rect2 srcRect, SurfaceEffects effects, BoxColor color, int zOrder)
		=> EngineBatchDraw(
			dstRect: new Rect2(position, srcRect.Size),
			srcRect: srcRect,
			texture: surface.ToSFML(),
			color: color,
			effects: effects,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One,
			zOrder
		);

	/// <summary>
	/// Draws a portion of the surface defined by srcRect onto the destination rectangle dstRect with the given color.
	/// </summary>
	/// <param name="surface">The surface to draw.</param>
	/// <param name="dstRect">The destination rectangle defining where to draw on the surface.</param>
	/// <param name="srcRect">The source rectangle defining the portion of the surface to draw.</param>
	/// <param name="color">The color tint to apply to the surface.</param>
	public void Draw(Surface surface, Rect2 dstRect, Rect2 srcRect, BoxColor color)
		=> EngineBatchDraw(
			dstRect: dstRect,
			srcRect: srcRect,
			texture: surface.ToSFML(),
			color: color,
			effects: SurfaceEffects.None,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One
		);

	/// <summary>
	/// Draws a portion of a surface (defined by <paramref name="srcRect"/>) onto a destination rectangle on screen, with optional effects, color tint, and z-order layering.
	/// </summary>
	/// <param name="surface">The surface (texture) to draw.</param>
	/// <param name="dstRect">The destination rectangle in screen space where the surface will be drawn.</param>
	/// <param name="srcRect">The source rectangle in texture space defining which part of the surface to draw.</param>
	/// <param name="effects">Optional surface effects such as flipping, rotation, or scaling.</param>
	/// <param name="color">The color tint to apply to the drawn surface.</param>
	/// <param name="zOrder">The z-order used to control draw order layering. Higher values are drawn above lower ones.</param>
	public void Draw(Surface surface, Rect2 dstRect, Rect2 srcRect, SurfaceEffects effects, BoxColor color, int zOrder)
		=> EngineBatchDraw(
			dstRect: dstRect,
			srcRect: srcRect,
			texture: surface.ToSFML(),
			color: color,
			effects: effects,
			origin: Vect2.Zero,
			rotation: 0,
			scale: Vect2.One,
			zOrder
		);



	/// <summary>
	/// Draws a point at the specified position with the given color.
	/// </summary>
	/// <param name="position">The position to draw the point.</param>
	/// <param name="color">The color of the point.</param>
	public void DrawPoint(Vect2 position, BoxColor color)
		=> EngineBatchDrawPoint(position, color);



	/// <summary>
	/// Draws a straight line between two points with the specified thickness and color.
	/// </summary>
	/// <param name="start">The starting point of the line.</param>
	/// <param name="end">The ending point of the line.</param>
	/// <param name="thickness">The thickness of the line.</param>
	/// <param name="color">The color of the line.</param>
	public void DrawLine(Vect2 start, Vect2 end, float thickness, BoxColor color)
		=> EngineBatchDrawLine(start, end, thickness, color);



	/// <summary>
	/// Draws a rectangle defined by the specified coordinates, width, and height with the given color.
	/// </summary>
	/// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	/// <param name="color">The color of the rectangle.</param>
	public void DrawRectangleFill(float x, float y, float width, float height, BoxColor color)
		=> EngineBatchDrawRectangleFill(new Rect2(x, y, width, height), color);

	/// <summary>
	/// Draws the outline of a rectangle at the specified coordinates, using the given dimensions, outline thickness, and color.
	/// </summary>
	/// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	/// <param name="thickness">The thickness of the rectangle's outline stroke.</param>
	/// <param name="color">The color of the rectangle outline.</param>
	public void DrawRectangleOutline(float x, float y, float width, float height, float thickness, BoxColor color)
		=> EngineBatchDrawRectangleOutline(new Rect2(x, y, width, height), thickness, color);



	/// <summary>
	/// Draws a filled rounded rectangle at the specified position with the given dimensions, corner radius, and color.
	/// </summary>
	/// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	/// <param name="radius">The radius of the rounded corners.</param>
	/// <param name="color">The fill color of the rectangle.</param>
	public void DrawRoundedRectangleFill(float x, float y, float width, float height, float radius, BoxColor color)
		=> EngineDrawRoundedRectangleFill(new Rect2(x, y, width, height), radius, color);

	/// <summary>
	/// Draws the outline of a rounded rectangle at the specified position with the given dimensions, corner radius, segment count, thickness, and color.
	/// </summary>
	/// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	/// <param name="radius">The radius of the rounded corners.</param>
	/// <param name="color">The color of the outline.</param>
	/// <param name="segpemnts">The number of segments used to approximate each rounded corner.</param>
	/// <param name="thickness">The thickness of the outline.</param>
	public void DrawRoundedRectangleOUtline(float x, float y, float width, float height, float radius, BoxColor color,
	int segpemnts = 6, float thickness = 1)
		=> EngineDrawRoundedRectangleOutline(new Rect2(x, y, width, height), radius, thickness, segpemnts, color);


	/// <summary>
	/// Draws the outline of an ellipse centered at the specified position with the given radii, segment count, color, and optional transform parameters.
	/// </summary>
	/// <param name="center">The center position of the ellipse.</param>
	/// <param name="radius">The radii of the ellipse along X and Y axes.</param>
	/// <param name="segments">The number of segments used to approximate the ellipse curve.</param>
	/// <param name="color">The color of the ellipse outline.</param>
	/// <param name="origin">The origin offset for rotation and scaling (default is <c>null</c>).</param>
	/// <param name="rotation">The rotation angle in radians (default is <c>0f</c>).</param>
	/// <param name="scale">The scaling factor applied to the ellipse (default is <c>null</c>).</param>
	/// <param name="thickness">The thickness of the outline stroke (default is <c>1f</c>).</param>
	public void DrawBuildEllipseOutline(Vect2 center, Vect2 radius, int segments, BoxColor color, Vect2? origin = null,
	float rotation = 0f, Vect2? scale = null, float thickness = 1f)
		=> EngineBuildEllipseOutline(center, radius, segments, color, origin, rotation, scale, thickness);

	/// <summary>
	/// Draws a filled ellipse centered at the specified position with the given radii, segment count, color, and optional transform parameters.
	/// </summary>
	/// <param name="center">The center position of the ellipse.</param>
	/// <param name="radius">The radii of the ellipse along X and Y axes.</param>
	/// <param name="segments">The number of segments used to approximate the ellipse shape.</param>
	/// <param name="color">The fill color of the ellipse.</param>
	/// <param name="origin">The origin offset for rotation and scaling (default is <c>null</c>).</param>
	/// <param name="rotation">The rotation angle in radians (default is <c>0f</c>).</param>
	/// <param name="scale">The scaling factor applied to the ellipse (default is <c>null</c>).</param>
	public void DrawBuildEllipseFill(Vect2 center, Vect2 radius, int segments, BoxColor color, Vect2? origin = null,
	float rotation = 0f, Vect2? scale = null)
		=> EngineBuildEllipseFill(center, radius, segments, color, origin, rotation, scale);


	/// <summary>
	/// Draws the outline of a circle centered at the specified position, using the given radius, segment count, thickness, and color.
	/// </summary>
	/// <param name="position">The center position of the circle.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="segments">The number of segments used to approximate the circle's curve. Higher values result in smoother outlines.</param>
	/// <param name="thickness">The thickness of the circle's outline stroke.</param>
	/// <param name="color">The color applied to the circle outline.</param>
	public void DrawCircleOutline(Vect2 position, float radius, int segments, float thickness, BoxColor color)
		=> EngineBatchDrawCircleOutline(position, radius, segments, thickness, color);


	/// <summary>
	/// Draws a filled circle centered at the specified coordinates, using the given radius, segment count, and color.
	/// </summary>
	/// <param name="x">The x-coordinate of the circle's center.</param>
	/// <param name="y">The y-coordinate of the circle's center.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="segments">The number of triangle segments used to approximate the circle's shape. Higher values result in smoother edges.</param>
	/// <param name="color">The fill color of the circle.</param>
	public void DrawCircleFill(float x, float y, float radius, int segments, BoxColor color)
		=> EngineBatchDrawCircleFill(new Vect2(x, y), radius, segments, color);

	/// <summary>
	/// Draws a string of text at the specified position using the given font, color, and optional z-ordering.
	/// </summary>
	/// <param name="font">The font used to render each character glyph.</param>
	/// <param name="text">The text string to draw on screen.</param>
	/// <param name="position">The top-left position where the text will begin rendering.</param>
	/// <param name="color">The color tint applied to the rendered text.</param>
	/// <param name="zOrder">The z-order depth for sorting draw order. Higher values are rendered above lower values (default is <c>0</c>).</param>
	public void DrawText(BoxFont font, string text, Vect2 position, BoxColor color, int zOrder = 0)
		=> EngineDrawText(font, text, position, color, zOrder);

	/// <summary>
	/// Draws a string of text at the specified screen coordinates using the given font, color, and optional z-order.
	/// </summary>
	/// <param name="font">The font used to render each character glyph.</param>
	/// <param name="text">The text string to render on screen.</param>
	/// <param name="x">The x-coordinate of the top-left starting position for the text.</param>
	/// <param name="y">The y-coordinate of the top-left starting position for the text.</param>
	/// <param name="color">The color tint applied to the text.</param>
	/// <param name="zOrder">The z-order depth for sorting draw order. Higher values are rendered above lower values (default is <c>0</c>).</param>
	public void DrawText(BoxFont font, string text, float x, float y, BoxColor color, int zOrder = 0)
		=> EngineDrawText(font, text, new(x, y), color, zOrder);

	/// <summary>
	/// Draws a polyline connecting a sequence of points, with the specified color and line thickness.
	/// </summary>
	/// <param name="points">The list of points that define the polyline. Must contain at least two points.</param>
	/// <param name="color">The color of the polyline.</param>
	/// <param name="thickness">The thickness of the line segments (default is <c>1f</c>).</param>
	public void DrawBuildPolyline(List<Vect2> points, BoxColor color, float thickness = 1f)
		=> EngineBuildPolyline(points, color, thickness);

	/// <summary>
	/// Draws a filled polygon using the specified list of points and fill color.
	/// </summary>
	/// <param name="points">The list of vertices defining the polygon. Must contain at least three points.</param>
	/// <param name="color">The fill color of the polygon.</param>
	public void DrawPolygonFill(List<Vect2> points, BoxColor color)
		=> EngineBuildPolygonFill(points, color);

	/// <summary>
	/// Draws a filled triangle using the specified three vertices and fill color.
	/// </summary>
	/// <param name="p0">The first vertex of the triangle.</param>
	/// <param name="p1">The second vertex of the triangle.</param>
	/// <param name="p2">The third vertex of the triangle.</param>
	/// <param name="color">The fill color of the triangle.</param>
	public void DrawTriangle(Vect2 p0, Vect2 p1, Vect2 p2, BoxColor color)
		=> EngineBuildTriangle(p0, p1, p2, color);

	#endregion


	#region Engine Batches
	private unsafe void EngineDrawText(BoxFont font, string text, Vect2 position, BoxColor color, int zOrder)
	{
		float lineSpacing = font.GetTextHeight();
		float posX = 0, posY = font is GenericFont ? lineSpacing : 0;

		var texture = font.GetTexture();
		var key = (texture, SFMLPrimitiveType.Triangles, SFMLBlendMode.Alpha, zOrder);

		if (!_groupedVerts.TryGetValue(key, out var verts))
		{
			verts = new List<SFMLVertex>(text.Length * 6); // estimate space
			_groupedVerts[key] = verts;
		}

		fixed (char* ptr = text)
		{
			char* c = ptr;
			char* end = ptr + text.Length;

			while (c < end)
			{
				if (*c == '\r')
				{
					c++;
					continue;
				}

				if (*c == '\n')
				{
					posX = 0;
					posY += lineSpacing;
					c++;
					continue;
				}

				if (!font.Glpyhs.TryGetValue(*c, out var glyph))
				{
					c++;
					continue;
				}

				var dstRect = new Rect2(
					position + new Vect2(posX, posY + glyph.Bounds.Top),
					new Vect2(glyph.TextureRect.Width, glyph.TextureRect.Height)
				);

				// if (!DrawUtils.IsVisible(_screen.Camera, dstRect))
				// 	continue;

				var srcRect = new Rect2(glyph.TextureRect);

				DrawUtils.BuildQuad(
					output: verts,
					dstRect: dstRect,
					srcRect: srcRect,
					color: color
				);

				posX += glyph.Advance + font.Spacing;
				c++;
			}
		}
	}


	private void EngineBatchDraw(Rect2 dstRect,
			Rect2 srcRect,
			SFMLTexture texture,
			BoxColor color,
			SurfaceEffects effects = SurfaceEffects.None,
			Vect2? origin = null,
			float rotation = 0f,
			Vect2? scale = null,
			int zOrder = 0
		)
	{
		if (!DrawUtils.IsVisible(_screen.Camera, dstRect))
			return;
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		// Group key
		var blend = SFMLBlendMode.Alpha; // Or param if needed
		var key = (texture, SFMLPrimitiveType.Triangles, blend, zOrder);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		DrawUtils.BuildQuad(
			output: list,
			dstRect: dstRect,
			srcRect: srcRect,
			color: color,
			effects: effects,
			origin: origin,
			rotation: rotation,
			scale: scale
		);

		_batchIndex++;
	}

	private void EngineBatchDrawLine(Vect2 start, Vect2 end, float thickness, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha; // Can be extended
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Triangles, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		DrawUtils.BuildLine(list, start, end, thickness, color);

		_batchIndex++;
	}

	private void EngineBatchDrawPoint(Vect2 position, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		DrawUtils.BuildPoint(list, position, color);
		_batchIndex++;
	}

	private void EngineBatchDrawRectangleOutline(Rect2 rectangle, float thickness, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.LineStrip, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildRectangleOutline(rectangle, thickness, color);
		list.AddRange(verts);

		_batchIndex++;
	}


	private void EngineBatchDrawRectangleFill(Rect2 rectangle, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Triangles, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildRectangleFill(rectangle, color);
		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBatchDrawCircleOutline(Vect2 center, float radius, int segments, float thickness, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Triangles, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildCircleOutline(center, radius, segments, thickness, color);
		list.AddRange(verts);

		_batchIndex++;
	}


	private void EngineBatchDrawCircleFill(Vect2 position, float radius, int segments, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Triangles, blend, 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildCircleFill(position, radius, segments, color);
		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineDrawRoundedRectangleFill(Rect2 value, float radius, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildRoundedRectangleFill(value, radius, color);

		list.AddRange(verts);

		_batchIndex++;
	}
	private void EngineDrawRoundedRectangleOutline(Rect2 value, float radius, float thickness, int segments, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildRoundedRectangleOutline(value, radius, thickness, segments, color);

		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBuildEllipseOutline(Vect2 center, Vect2 radius, int segments, BoxColor color, Vect2? origin = null,
	float rotation = 0f, Vect2? scale = null, float thickness = 1f)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildEllipseOutline(center, radius, segments, thickness, color, origin, rotation, scale);

		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBuildEllipseFill(Vect2 center, Vect2 radius, int segments, BoxColor color, Vect2? origin = null,
	float rotation = 0f, Vect2? scale = null)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildEllipseFill(center, radius, segments, color, origin, rotation, scale);

		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBuildPolyline(List<Vect2> points, BoxColor color, float thickness = 1f)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildPolyline(points, thickness, color);

		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBuildPolygonFill(List<Vect2> points, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildPolygonFill(points, color);

		list.AddRange(verts);

		_batchIndex++;
	}

	private void EngineBuildTriangle(Vect2 p0, Vect2 p1, Vect2 p2, BoxColor color)
	{
		if (_batchIndex > EngineSettings.Instance.MaxDrawCalls - 1)
			Flush();

		var blend = SFMLBlendMode.Alpha;
		var key = (texture: (SFMLTexture)null, SFMLPrimitiveType.Points, blend, ZOrder: 0);

		if (!_groupedVerts.TryGetValue(key, out var list))
		{
			list = new List<SFMLVertex>();
			_groupedVerts[key] = list;
		}

		var verts = DrawUtils.BuildTriangle(p0, p1, p2, color);

		list.AddRange(verts);

		_batchIndex++;
	}
	#endregion
}
