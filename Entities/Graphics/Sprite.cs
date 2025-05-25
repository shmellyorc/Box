using Box.Graphics.Batch;

namespace Box.Entities.Graphics;

/// <summary>
/// Represents a graphical sprite entity.
/// </summary>
public class Sprite : Entity
{
	private readonly Rect2 _source;
	private readonly Surface _surface;

	/// <summary>
	/// Represents the color of the sprite.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Represents the surface effects applied to the sprite.
	/// </summary>
	public SurfaceEffects Effects = SurfaceEffects.None;

	/// <summary>
	/// Represents the vertical alignment of the sprite within its container.
	/// </summary>
	public VAlign VAlign = VAlign.Top;

	/// <summary>
	/// Represents the horizontal alignment of the sprite within its container.
	/// </summary>
	public HAlign HAlign = HAlign.Left;

	/// <summary>
	/// Represents the size of the sprite.
	/// </summary>
	public new Vect2 Size
	{
		get => base.Size;
		set
		{
			var oldValue = base.Size;
			base.Size = value;

			if (base.Size != oldValue)
			{
				_isDirty = true;

				foreach (var parent in GetParents<Entity>())
					parent._isDirty = true;
			}
		}
	}


	/// <summary>
	/// Represents a sprite rendered from a surface with a specified source rectangle.
	/// </summary>
	/// <param name="surface">The surface to render as a sprite.</param>
	/// <param name="source">The source rectangle defining the area of the surface to render.</param>
	public Sprite(Surface surface, Rect2 source)
	{
		_surface = surface;
		_source = source;

		Size = _source.Size;
	}

	/// <summary>
	/// Initializes a sprite using the given surface. The size of the sprite may stretch or tile the surface.
	/// </summary>
	/// <param name="surface">The surface to use for initializing the sprite.</param>
	/// <remarks>
	/// Note: Using this constructor without providing a size may result in the entity having no size. 
	/// This constructor is primarily used for tiling sprites or background sprites.
	/// </remarks>
	public Sprite(Surface surface) : this(surface, surface.Bounds) { }

	/// <summary>
	/// Initializes a sprite using a specific region defined by the sprite sheet.
	/// </summary>
	/// <param name="surface">The surface to render as a sprite.</param>
	/// <param name="sheet">The sprite sheet containing the region definitions.</param>
	/// <param name="name">The name of the region within the sprite sheet.</param>
	public Sprite(Surface surface, Spritesheet sheet, string name) : this(surface, sheet.GetBounds(name)) { }

	/// <summary>
	/// Initializes a sprite using a specific cell index from a grid of cells within the surface.
	/// </summary>
	/// <param name="surface">The surface to render as a sprite.</param>
	/// <param name="cellSize">The size of each cell in the grid.</param>
	/// <param name="index">The index of the cell to use as the source.</param>
	public Sprite(Surface surface, Vect2 cellSize, int index) : this(surface, surface.GetSource(cellSize, index)) { }

	/// <summary>
	/// Updates the state or behavior of the object during each frame or update cycle.
	/// </summary>
	protected override void Update()
	{
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			if (!_source.IsEmpty)
				Renderer.Draw(_surface, Position + Alignment(), _source, Effects, Color, Layer);
			else
				Renderer.Draw(_surface, Position + Alignment(), Bounds > _surface.Bounds ? Bounds : _surface.Bounds, Effects, Color, Layer);
		}
		else
		{
			if (!_source.IsEmpty)
				target.Draw(_surface, Position - target.Position + Alignment(), _source, Effects, Color, Layer);
			else
				target.Draw(_surface, Position - target.Position + Alignment(), Bounds > _surface.Bounds ? Bounds : _surface.Bounds, Effects, Color, Layer);
		}

		if (GetService<EngineSettings>().DebugDraw)
			Renderer.Instance.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f, BoxColor.AllShades.Purple);
	}

	private object _target = null;

	private Vect2 Alignment()
	{
		var rect = _source.IsEmpty ? _surface.Bounds : _source;
		var result = Vect2.Zero;

		result.X = AlignmentHelpers.AlignWidth(Size.X, rect.Width, HAlign);
		result.Y = AlignmentHelpers.AlignHeight(Size.Y, rect.Height, VAlign);

		return result;
	}
}
