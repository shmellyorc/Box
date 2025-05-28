namespace Box.Entities.Graphics;

internal static class DrawUtils
{

	private const int MaxCachedCircleEntries = 256;
	private const int MaxCachedRectEntries = 256;
	private const int MaxCachedEllipseEntries = 256;
	private const int MaxCachedRoundedRectEntries = 512;
	private const int MaxCachedPolylineEntries = 256;
	private const int MaxCachedPolygonEntries = 256;
	private const int MaxCachedTriangleEntries = 256;
	private const int MaxCachePolyConvexEntries = 256;
	private const int RoundedSegments = 6;
	private const float HalfFOffset = 0.005f;

	private static readonly Dictionary<(float width, float height, float radius), SFMLVertex[]> _roundedRectFillCache = new();
	private static readonly Dictionary<(float width, float height, float radius, float thickness, int segments), SFMLVertex[]> _roundedRectOutlineCache = new();
	private static readonly Dictionary<(int segments, float radius), SFMLVertex[]> _circleFillCache = new();
	private static readonly Dictionary<(int segments, float radius), SFMLVertex[]> _circleOutlineCache = new();
	private static readonly Dictionary<(float width, float height), SFMLVertex[]> _rectFillCache = new();
	private static readonly Dictionary<(float width, float height, float thickness), SFMLVertex[]> _rectOutlineCache = new();
	private static readonly Dictionary<(int segments, float rx, float ry), SFMLVertex[]> _ellipseFillCache = new();
	private static readonly Dictionary<(int segments, float rx, float ry, float thickness), SFMLVertex[]> _ellipseOutlineCache = new();
	private static readonly Dictionary<(int pointCount, float thickness), SFMLVertex[]> _polylineCache = new();
	private static readonly Dictionary<int, SFMLVertex[]> _polygonFillCache = new();
	private static readonly Dictionary<int, SFMLVertex[]> _polygonConvexCache = new();
	private static readonly Dictionary<(Vect2 p0, Vect2 p1, Vect2 p2), SFMLVertex[]> _triangleCache = new();

	public static bool IsVisible(Camera camera, Rect2 bounds)
	{
		var sfCam = camera.ToSFML();
		var view = new Rect2(sfCam.Center, sfCam.Size);
		var size = view.Size;
		var center = view.Center;
		var left = center.X - size.X / 2f;
		var top = center.Y - size.Y / 2f;
		var viewRect = new Rect2(left, top, size.X, size.Y);

		return bounds.Intersects(camera.Area);
	}

	public static void ClearCaches()
	{
		_circleFillCache.Clear();
		_circleOutlineCache.Clear();
		_rectFillCache.Clear();
		_rectOutlineCache.Clear();
		_ellipseFillCache.Clear();
		_ellipseOutlineCache.Clear();
		_roundedRectFillCache.Clear();
		_roundedRectOutlineCache.Clear();
	}

	public static void BuildQuad(
		List<SFMLVertex> output,
		Rect2 dstRect,
		Rect2 srcRect,
		BoxColor color,
		SurfaceEffects effects = SurfaceEffects.None,
		Vect2? origin = null,
		float rotation = 0f,
		Vect2? scale = null)
	{

		output.EnsureCapacity(output.Count + 6);
		var originOffset = origin ?? Vect2.Zero;
		var scaleFactor = scale ?? Vect2.One;

		var topLeft = (new Vect2(0, 0) - originOffset) * scaleFactor;
		var topRight = (new Vect2(dstRect.Width, 0) - originOffset) * scaleFactor;
		var bottomLeft = (new Vect2(0, dstRect.Height) - originOffset) * scaleFactor;
		var bottomRight = (new Vect2(dstRect.Width, dstRect.Height) - originOffset) * scaleFactor;

		if (rotation != 0f)
		{
			topLeft = topLeft.Rotate(rotation);
			topRight = topRight.Rotate(rotation);
			bottomLeft = bottomLeft.Rotate(rotation);
			bottomRight = bottomRight.Rotate(rotation);
		}

		topLeft += dstRect.Position;
		topRight += dstRect.Position;
		bottomLeft += dstRect.Position;
		bottomRight += dstRect.Position;

		var ho = EngineSettings.Instance.UseTextureHalfOffset ? HalfFOffset : 0f;
		var uvLeft = srcRect.X + ho;
		var uvTop = srcRect.Y + ho;
		var uvRight = srcRect.X + srcRect.Width - ho;
		var uvBottom = srcRect.Y + srcRect.Height - ho;

		if ((effects & SurfaceEffects.FlipHorizontally) != 0)
			(uvLeft, uvRight) = (uvRight, uvLeft);

		if ((effects & SurfaceEffects.FlipVErtically) != 0)
			(uvTop, uvBottom) = (uvBottom, uvTop);

		var col = color.ToSFML();
		output.Add(new SFMLVertex(topLeft.ToSFML_F(), col, new SFMLVectF(uvLeft, uvTop)));
		output.Add(new SFMLVertex(topRight.ToSFML_F(), col, new SFMLVectF(uvRight, uvTop)));
		output.Add(new SFMLVertex(bottomLeft.ToSFML_F(), col, new SFMLVectF(uvLeft, uvBottom)));
		output.Add(new SFMLVertex(topRight.ToSFML_F(), col, new SFMLVectF(uvRight, uvTop)));
		output.Add(new SFMLVertex(bottomRight.ToSFML_F(), col, new SFMLVectF(uvRight, uvBottom)));
		output.Add(new SFMLVertex(bottomLeft.ToSFML_F(), col, new SFMLVectF(uvLeft, uvBottom)));
	}

	public static void BuildPoint(List<SFMLVertex> output, Vect2 position, BoxColor color)
	{
		output.Add(new SFMLVertex(position.ToSFML_F(), color.ToSFML()));
	}

	public static void BuildLine(List<SFMLVertex> output, Vect2 start, Vect2 end, float thickness, BoxColor color)
	{
		var direction = (end - start).Normalized();
		var perpendicular = new Vect2(-direction.Y, direction.X) * (thickness * 0.5f);
		var v0 = start + perpendicular;
		var v1 = end + perpendicular;
		var v2 = end - perpendicular;
		var v3 = start - perpendicular;
		output.Add(new SFMLVertex(v0.ToSFML_F(), color.ToSFML()));
		output.Add(new SFMLVertex(v1.ToSFML_F(), color.ToSFML()));
		output.Add(new SFMLVertex(v2.ToSFML_F(), color.ToSFML()));
		output.Add(new SFMLVertex(v2.ToSFML_F(), color.ToSFML()));
		output.Add(new SFMLVertex(v3.ToSFML_F(), color.ToSFML()));
		output.Add(new SFMLVertex(v0.ToSFML_F(), color.ToSFML()));
	}

	public static SFMLVertex[] BuildRectangleOutline(Rect2 rect, float thickness, BoxColor color)
	{
		if (_rectOutlineCache.Count > MaxCachedRectEntries)
			_rectOutlineCache.Clear();
		var key = (rect.Width, rect.Height, thickness);
		if (!_rectOutlineCache.TryGetValue(key, out var verts))
		{
			verts = new SFMLVertex[]
			{
				new(new(0, 0), BoxColor.AllShades.White.ToSFML()),
				new(new(rect.Width, 0), BoxColor.AllShades.White.ToSFML()),
				new(new(rect.Width, rect.Height), BoxColor.AllShades.White.ToSFML()),
				new(new(0, rect.Height), BoxColor.AllShades.White.ToSFML()),
				new(new(0, 0), BoxColor.AllShades.White.ToSFML())
			};
			_rectOutlineCache[key] = verts;
		}
		return TranslateVertices(verts, new Vect2(rect.Left, rect.Top), color);
	}

	public static SFMLVertex[] BuildRectangleFill(Rect2 rect, BoxColor color)
	{
		if (_rectFillCache.Count > MaxCachedRectEntries)
			_rectFillCache.Clear();
		var key = (rect.Width, rect.Height);
		if (!_rectFillCache.TryGetValue(key, out var verts))
		{
			verts = BuildQuadVertices(0, 0, rect.Width, rect.Height, BoxColor.AllShades.White);
			_rectFillCache[key] = verts;
		}
		return TranslateVertices(verts, new Vect2(rect.Left, rect.Top), color);
	}

	public static SFMLVertex[] BuildCircleOutline(Vect2 center, float radius, int segments, float thickness, BoxColor color)
	{
		if (_circleOutlineCache.Count > MaxCachedCircleEntries)
			_circleOutlineCache.Clear();
		var key = (segments, radius);
		if (!_circleOutlineCache.TryGetValue(key, out var cached))
		{
			var verts = new List<SFMLVertex>();
			float step = MathF.Tau / segments;
			for (int i = 0; i < segments; i++)
			{
				float a0 = i * step;
				float a1 = (i + 1) * step;
				var in0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (radius - thickness * 0.5f);
				var in1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (radius - thickness * 0.5f);
				var out0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (radius + thickness * 0.5f);
				var out1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (radius + thickness * 0.5f);
				verts.Add(new SFMLVertex(in0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(out0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(out1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(out1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(in1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(in0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			}
			cached = verts.ToArray();
			_circleOutlineCache[key] = cached;
		}
		return TranslateVertices(cached, center, color);
	}

	public static SFMLVertex[] BuildCircleFill(Vect2 center, float radius, int segments, BoxColor color, Vect2? origin = null,
	float rotation = 0f, Vect2? scale = null)
	{
		if (_circleFillCache.Count > MaxCachedCircleEntries)
			_circleFillCache.Clear();
		var key = (segments, radius);
		if (!_circleFillCache.TryGetValue(key, out var cached))
		{
			var verts = new List<SFMLVertex>();
			float step = MathF.Tau / segments;
			for (int i = 0; i < segments; i++)
			{
				float a0 = i * step;
				float a1 = (i + 1) * step;
				var p1 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * radius;
				var p2 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * radius;
				verts.Add(new SFMLVertex(new Vect2(0, 0).ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p2.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			}
			cached = verts.ToArray();
			_circleFillCache[key] = cached;
		}
		return TransformVertices(cached, center, color, origin, rotation, scale);
	}

	public static SFMLVertex[] BuildEllipseFill(
		Vect2 center,
		Vect2 radii,
		int segments,
		BoxColor color,
		Vect2? origin = null,
		float rotation = 0f,
		Vect2? scale = null)
	{
		if (_ellipseFillCache.Count > MaxCachedEllipseEntries)
			_ellipseFillCache.Clear();

		var key = (segments, radii.X, radii.Y);
		if (!_ellipseFillCache.TryGetValue(key, out var cached))
		{
			var verts = new List<SFMLVertex>();
			float angleStep = MathF.Tau / segments;

			for (int i = 0; i < segments; i++)
			{
				float a0 = i * angleStep;
				float a1 = (i + 1) * angleStep;

				var p1 = new Vect2(MathF.Cos(a0), MathF.Sin(a0));
				var p2 = new Vect2(MathF.Cos(a1), MathF.Sin(a1));

				verts.Add(new SFMLVertex(new Vect2(0, 0).ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p2.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			}

			cached = verts.ToArray();
			_ellipseFillCache[key] = cached;
		}

		return TransformVertices(cached, center, color, origin, rotation, scale: radii * (scale ?? Vect2.One));
	}

	public static SFMLVertex[] BuildEllipseOutline(
		Vect2 center,
		Vect2 radii,
		int segments,
		float thickness,
		BoxColor color,
		Vect2? origin = null,
		float rotation = 0f,
		Vect2? scale = null)
	{
		if (_ellipseOutlineCache.Count > MaxCachedEllipseEntries)
			_ellipseOutlineCache.Clear();

		var key = (segments, radii.X, radii.Y, thickness);
		if (_ellipseOutlineCache.TryGetValue(key, out var cached))
			return TransformVertices(cached, center, color, origin, rotation, scale);

		var verts = new List<SFMLVertex>();
		float angleStep = MathF.Tau / segments;
		var thicknessOffset = thickness * 0.5f;

		for (int i = 0; i < segments; i++)
		{
			float a0 = i * angleStep;
			float a1 = (i + 1) * angleStep;

			var inner0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (1f - thicknessOffset);
			var inner1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (1f - thicknessOffset);
			var outer0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (1f + thicknessOffset);
			var outer1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (1f + thicknessOffset);

			verts.Add(new SFMLVertex(inner0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			verts.Add(new SFMLVertex(outer0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			verts.Add(new SFMLVertex(outer1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			verts.Add(new SFMLVertex(outer1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			verts.Add(new SFMLVertex(inner1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			verts.Add(new SFMLVertex(inner0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
		}

		cached = verts.ToArray();
		_ellipseOutlineCache[key] = cached;
		return TransformVertices(cached, center, color, origin, rotation, scale: radii * (scale ?? Vect2.One));
	}

	public static SFMLVertex[] BuildRoundedRectangleFill(Rect2 rect, float radius, Color color)
	{
		if (_roundedRectFillCache.Count > MaxCachedRoundedRectEntries)
			_polygonConvexCache.Clear();
		if (_roundedRectFillCache.TryGetValue((rect.Width, rect.Height, radius), out var cached))
			return TransformVertices(cached, rect.Position, color);

		var verts = new List<SFMLVertex>();
		float w = rect.Width, h = rect.Height, r = radius;

		// Clamp radius to half width/height
		r = MathF.Min(MathF.Min(w * 0.5f, h * 0.5f), r);

		// Center rect (0,0) — apply transform later
		Vect2[] corners =
		{
			new(+w - r, +r),     // Top-right
			new(+w - r, +h - r), // Bottom-right
			new(+r, +h - r),     // Bottom-left
			new(+r, +r),         // Top-left
		};

		float step = MathF.PI * 0.5f / RoundedSegments;
		float[] angles = { -MathF.PI * 0.5f, 0, MathF.PI * 0.5f, MathF.PI };

		// Triangulate center rect
		verts.AddRange(BuildQuadVertices(r, r, w - r * 2, h - r * 2, Color.AllShades.White));

		// Four edges (without corners)
		verts.AddRange(BuildQuadVertices(r, 0, w - r * 2, r, Color.AllShades.White)); // Top
		verts.AddRange(BuildQuadVertices(r, h - r, w - r * 2, r, Color.AllShades.White)); // Bottom
		verts.AddRange(BuildQuadVertices(0, r, r, h - r * 2, Color.AllShades.White)); // Left
		verts.AddRange(BuildQuadVertices(w - r, r, r, h - r * 2, Color.AllShades.White)); // Right

		// Rounded corners
		for (int corner = 0; corner < 4; corner++)
		{
			var center = corners[corner];
			float angle = angles[corner];

			for (int i = 0; i < RoundedSegments; i++)
			{
				float a0 = angle + step * i;
				float a1 = angle + step * (i + 1);

				var p0 = center;
				var p1 = center + new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * r;
				var p2 = center + new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * r;

				verts.Add(new SFMLVertex(p0.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p1.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
				verts.Add(new SFMLVertex(p2.ToSFML_F(), BoxColor.AllShades.White.ToSFML()));
			}
		}

		var result = verts.ToArray();
		_roundedRectFillCache[(w, h, r)] = result;
		return TransformVertices(result, rect.Position, color);
	}

	public static SFMLVertex[] BuildRoundedRectangleOutline(Rect2 rect, float radius, float thickness, int segments = 6, Color? overrideColor = null)
	{
		if (_roundedRectOutlineCache.Count > MaxCachedRoundedRectEntries)
			_roundedRectOutlineCache.Clear();
		if (segments < RoundedSegments)
			segments = RoundedSegments;

		float w = rect.Width, h = rect.Height, r = radius;
		r = MathF.Min(MathF.Min(w * 0.5f, h * 0.5f), r);
		var key = (w, h, r, thickness, segments);

		if (_roundedRectOutlineCache.TryGetValue(key, out var cached))
			return TranslateVertices(cached, new Vect2(rect.Left, rect.Top), overrideColor ?? BoxColor.AllShades.White);

		var verts = new List<SFMLVertex>();
		float angleStep = MathF.PI * 0.5f / segments;
		var white = BoxColor.AllShades.White.ToSFML();

		void AddCorner(float cx, float cy, float startAngle)
		{
			for (int i = 0; i < segments; i++)
			{
				float a0 = startAngle + i * angleStep;
				float a1 = startAngle + (i + 1) * angleStep;

				var inner0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (r - thickness * 0.5f);
				var inner1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (r - thickness * 0.5f);
				var outer0 = new Vect2(MathF.Cos(a0), MathF.Sin(a0)) * (r + thickness * 0.5f);
				var outer1 = new Vect2(MathF.Cos(a1), MathF.Sin(a1)) * (r + thickness * 0.5f);

				verts.Add(new SFMLVertex((new Vect2(cx, cy) + inner0).ToSFML_F(), white));
				verts.Add(new SFMLVertex((new Vect2(cx, cy) + outer0).ToSFML_F(), white));
				verts.Add(new SFMLVertex((new Vect2(cx, cy) + outer1).ToSFML_F(), white));
				verts.Add(new SFMLVertex((new Vect2(cx, cy) + outer1).ToSFML_F(), white));
				verts.Add(new SFMLVertex((new Vect2(cx, cy) + inner1).ToSFML_F(), white));
				verts.Add(new SFMLVertex((new Vect2(cx, cy) + inner0).ToSFML_F(), white));
			}
		}

		AddCorner(r, r, MathF.PI);             // Top-left
		AddCorner(w - r, r, -MathF.PI * 0.5f);  // Top-right
		AddCorner(w - r, h - r, 0);             // Bottom-right
		AddCorner(r, h - r, MathF.PI * 0.5f);   // Bottom-left

		cached = verts.ToArray();
		_roundedRectOutlineCache[key] = cached;
		return TranslateVertices(cached, new Vect2(rect.Left, rect.Top), overrideColor ?? BoxColor.AllShades.White);
	}

	public static SFMLVertex[] BuildPolyline(IReadOnlyList<Vect2> points, float thickness, BoxColor color)
	{
		if (points.Count < 2)
			return Array.Empty<SFMLVertex>();

		var key = (points.Count, thickness);
		if (_polylineCache.Count > MaxCachedPolylineEntries)
			_polylineCache.Clear();

		if (_polylineCache.TryGetValue(key, out var cached))
			return TransformVertices(cached, Vect2.Zero, color);

		var white = BoxColor.AllShades.White.ToSFML();
		var verts = new List<SFMLVertex>(points.Count * 6);

		for (int i = 0; i < points.Count - 1; i++)
		{
			var start = points[i];
			var end = points[i + 1];
			var dir = (end - start).Normalized();
			var perp = new Vect2(-dir.Y, dir.X) * (thickness * 0.5f);

			var v0 = start + perp;
			var v1 = end + perp;
			var v2 = end - perp;
			var v3 = start - perp;

			verts.Add(new SFMLVertex(v0.ToSFML_F(), white));
			verts.Add(new SFMLVertex(v1.ToSFML_F(), white));
			verts.Add(new SFMLVertex(v2.ToSFML_F(), white));
			verts.Add(new SFMLVertex(v2.ToSFML_F(), white));
			verts.Add(new SFMLVertex(v3.ToSFML_F(), white));
			verts.Add(new SFMLVertex(v0.ToSFML_F(), white));
		}

		cached = verts.ToArray();
		_polylineCache[key] = cached;
		return TransformVertices(cached, Vect2.Zero, color);
	}

	public static SFMLVertex[] BuildPolygonFill(IReadOnlyList<Vect2> points, BoxColor color)
	{
		if (points.Count < 3)
			return System.Array.Empty<SFMLVertex>();

		var key = (points.Count);
		if (_polygonFillCache.Count > MaxCachedPolygonEntries)
			_polygonFillCache.Clear();

		if (_polygonFillCache.TryGetValue(key, out var cached))
			return TransformVertices(cached, Vect2.Zero, color);

		var white = BoxColor.AllShades.White.ToSFML();
		var verts = new List<SFMLVertex>((points.Count - 2) * 3);
		var p0 = points[0];
		for (int i = 1; i < points.Count - 1; i++)
		{
			verts.Add(new SFMLVertex(p0.ToSFML_F(), white));
			verts.Add(new SFMLVertex(points[i].ToSFML_F(), white));
			verts.Add(new SFMLVertex(points[i + 1].ToSFML_F(), white));
		}

		cached = verts.ToArray();
		_polygonFillCache[key] = cached;
		return TransformVertices(cached, Vect2.Zero, color);
	}

	public static SFMLVertex[] BuildTriangle(Vect2 p0, Vect2 p1, Vect2 p2, BoxColor color)
	{
		if (_triangleCache.Count > MaxCachedTriangleEntries)
			_triangleCache.Clear();

		var key = (p0, p1, p2);
		if (_triangleCache.TryGetValue(key, out var cached))
			return TransformVertices(cached, Vect2.Zero, color);

		var white = BoxColor.AllShades.White.ToSFML();
		cached = new[]
		{
			new SFMLVertex(p0.ToSFML_F(), white),
			new SFMLVertex(p1.ToSFML_F(), white),
			new SFMLVertex(p2.ToSFML_F(), white)
		};
		_triangleCache[key] = cached;
		return TransformVertices(cached, Vect2.Zero, color);
	}

	public static SFMLVertex[] BuildConvexPolygon(IReadOnlyList<Vect2> points, BoxColor color)
	{
		if (points.Count < 3)
			return System.Array.Empty<SFMLVertex>();

		if (_polygonConvexCache.Count > MaxCachePolyConvexEntries)
			_polygonConvexCache.Clear();

		var key = (points.Count);
		if (_polygonFillCache.TryGetValue(key, out var cached))
			return TransformVertices(cached, Vect2.Zero, color);

		var white = Color.AllShades.White.ToSFML();
		var verts = new List<SFMLVertex>((points.Count - 2) * 3);
		var p0 = points[0];
		for (int i = 1; i < points.Count - 1; i++)
		{
			verts.Add(new SFMLVertex(p0.ToSFML_F(), white));
			verts.Add(new SFMLVertex(points[i].ToSFML_F(), white));
			verts.Add(new SFMLVertex(points[i + 1].ToSFML_F(), white));
		}

		cached = verts.ToArray();
		_polygonConvexCache[key] = cached;
		return TransformVertices(cached, Vect2.Zero, color);
	}



	private static SFMLVertex[] TransformVertices(SFMLVertex[] source, Vect2 translation, BoxColor color)
	{
		var result = new SFMLVertex[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			var v = source[i];
			result[i] = new SFMLVertex(v.Position + translation.ToSFML_F(), color.ToSFML());
		}
		return result;
	}

	private static SFMLVertex[] BuildQuadVertices(float x, float y, float width, float height, BoxColor color)
	{
		var c = color.ToSFML();
		return new SFMLVertex[]
		{
			new(new(x, y), c),
			new(new(x + width, y), c),
			new(new(x + width, y + height), c),
			new(new(x, y + height), c),
		};
	}

	private static SFMLVertex[] TranslateVertices(SFMLVertex[] source, Vect2 offset, Color color)
	{
		var result = new SFMLVertex[source.Length];
		var c = color.ToSFML();
		for (int i = 0; i < source.Length; i++)
			result[i] = new SFMLVertex(source[i].Position + offset.ToSFML_F(), c);
		return result;
	}

	private static SFMLVertex[] TransformVertices(SFMLVertex[] source, Vect2 translation, BoxColor color,
	Vect2? origin = null, float rotation = 0f, Vect2? scale = null)
	{
		var result = new SFMLVertex[source.Length];
		var cos = MathF.Cos(rotation);
		var sin = MathF.Sin(rotation);
		var sc = scale ?? Vect2.One;
		var ox = origin?.X ?? 0f;
		var oy = origin?.Y ?? 0f;
		var c = color.ToSFML();
		for (int i = 0; i < source.Length; i++)
		{
			var raw = source[i].Position;
			var x = raw.X - ox;
			var y = raw.Y - oy;
			x *= sc.X;
			y *= sc.Y;
			var rx = x * cos - y * sin;
			var ry = x * sin + y * cos;
			var fx = rx + ox + translation.X;
			var fy = ry + oy + translation.Y;
			result[i] = new SFMLVertex(new(fx, fy), c);
		}
		return result;
	}
}
