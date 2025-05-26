namespace Box.Entities.Graphics;

/// <summary>
/// Represents a rendering target that inherits from the Entity class.
/// </summary>
public class RenderTarget : Entity
{
	// How many instructions and/or items it should process at once for the flush.
	private List<Entity> _children = new();
	private SFMLRenderTexture _target;
	private readonly Dictionary<(SFMLTexture, SFMLPrimitiveType, SFMLBlendMode, int zOrder), List<SFMLVertex>> _groupedVerts = new();
	private int _batchIndex, _count; //_batchIndex
	private Vect2 _offset;
	private SFMLView _view;
	private bool _initialized = false;
	private Surface _surface;

	/// <summary>
	/// Gets a value indicating whether rendering is currently active.
	/// </summary>
	public bool IsRendering { get; private set; }

	/// <summary>
	/// Gets or sets a value indicating whether smoothing is enabled.
	/// </summary>
	public bool Smoothing
	{
		get => _target.Smooth;
		set => _target.Smooth = value;
	}

	/// <summary>
	/// Gets or sets the offset for rendering.
	/// </summary>
	public Vect2 Offset
	{
		get => _offset;
		set
		{
			// var oldValue = _offset;
			_offset = value;

			// if (_offset != oldValue)
			// {
			IEnumerator WaitForView()
			{
				while (_view == null)
					yield return null;

				_view.Center =
					new SFMLVector(Size.X / 2 + _offset.X, Size.Y / 2 + _offset.Y);
			}

			StartRoutine(WaitForView());
			// }
		}
	}

	/// <summary>
	/// Gets or sets the color of the render target.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Initializes a new instance of the RenderTarget class with the specified children.
	/// </summary>
	/// <param name="children">The child entities to associate with this render target.</param>
	public RenderTarget(params Entity[] children)
	{
		_children = new List<Entity>(children);
	}

	/// <summary>
	/// Called when the entity enters the scene.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when a parent entity already contains a render target.</exception>
	protected override void OnEnter()
	{
		if (_children.IsEmpty())
			return;
		if (AnyParentOfType<BoxRenderTarget>(out _))
			throw new Exception("A parent contains a render target. Only one render target can be used with this group of entities.");

		_target = new SFMLRenderTexture((uint)Size.X, (uint)Size.Y);
		_view = new SFMLView(new SFMLFloatRect(0, 0, Size.X, Size.Y));
		_surface = new Surface(_target.Texture);

		_target.SetView(_view);

		unsafe
		{
			fixed (Entity* ptr = _children.ToArray())
			{
				for (int i = 0; i < _children.Count; i++)
				{
					base.AddChild(*(ptr + i));
				}
			}
		}

		_children.Clear();
		_initialized = true;

		base.OnEnter();
	}

	#region Entity (Override)
	/// <summary>
	/// Adds a single child entity to this rendering target.
	/// </summary>
	/// <param name="entity">The entity to add as a child.</param>
	public new void AddChild(Entity entity) => AddChild(entities: entity);

	/// <summary>
	/// Adds multiple child entities to this rendering target.
	/// </summary>
	/// <param name="entities">The entities to add as children.</param>
	public new void AddChild(params Entity[] entities)
	{
		if (!_initialized && Size > Vect2.Zero)
		{
			_target?.Dispose();
			_target = new SFMLRenderTexture((uint)Size.X, (uint)Size.Y);
			_view = new SFMLView(new SFMLFloatRect(0, 0, Size.X, Size.Y));
			_surface = new Surface(_target.Texture);

			_target.SetView(_view);

			_initialized = true;
		}

		base.AddChild(entities);
	}

	// /// <summary>
	// /// Adds multiple child entities of a specific type to this rendering target.
	// /// </summary>
	// /// <typeparam name="T">The type of the child entities.</typeparam>
	// /// <param name="entity">The entities to add as children.</param>
	// /// <returns>The current instance of the rendering target.</returns>
	public new T AddChild<T>(params Entity[] entity) where T : RenderTarget
	{
		AddChild(entity);

		return (T)this;
	}
	#endregion

	/// <summary>
	/// Updates the state of the entity.
	/// </summary>
	protected override void Update()
	{
		if (IsExiting)
			return;
		if (!_initialized)
			return;
		if (_target == null)
			return;
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;
		if (_batchIndex == 0)
			return;

		IsRendering = true;

		_target.Clear(BoxColor.ShadesOfTransparency.Transparent.ToSFML());
		_target.SetView(_view);

		Flush();

		_target.Display();

		Renderer.Draw(_surface, Position, Color);

		IsRendering = false;

		if (GetService<EngineSettings>().DebugDraw)
			Renderer.DrawRectangleOutline(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, 1f, BoxColor.AllShades.Teal);
	}



	private void Flush()
	{
		if (_groupedVerts.Count == 0)
			return;

		var sorted = _groupedVerts.OrderBy(pair => pair.Key.zOrder);

		foreach (var (key, verts) in sorted)
		{
			if (verts.Count == 0)
				continue;

			var (texture, primitive, blend, _) = key;

			using var buffer = new SFMLVertexBuffer((uint)verts.Count, primitive, SFMLVertexBuffer.UsageSpecifier.Stream);
			if (!buffer.Update(verts.ToArray()))
				continue;

			var state = new SFMLRenderStates
			{
				Texture = texture,
				BlendMode = blend,
				Transform = SFMLTransform.Identity,
			};

			_target.Draw(buffer, state);
		}

		_groupedVerts.Clear();
		_batchIndex = 0;
	}

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

				// if (!DrawUtils.IsVisible(Screen.Camera, dstRect))
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
		if (!DrawUtils.IsVisible(Screen.Camera, dstRect))
			return;
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
		if (_batchIndex > GetService<EngineSettings>().MaxDrawCalls - 1)
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
