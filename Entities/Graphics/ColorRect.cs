using Box.Graphics.Batch;

namespace Box.Entities.Graphics;

/// <summary>
/// Represents a rectangular entity with color attributes.
/// </summary>
public class ColorRect : Entity
{
	private Surface _surface;

	/// <summary>
	/// The color of the rectangle.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Initializes a new instance of the <see cref="ColorRect"/> class with default color (white).
	/// </summary>
	public ColorRect() => Size = Renderer.Instance.Size;

	/// <summary>
	/// Called when the entity enters the active state.
	/// </summary>
	protected override void OnEnter()
	{
		_surface = new Surface(1, 1);

		base.OnEnter();
	}

	/// <summary>
	/// Updates the entity's state in each frame.
	/// </summary>
	protected override void Update()
	{
		if (Size == Vect2.Zero)
			return;
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			BE.Renderer.Draw(_surface,
				new Rect2(Position.X, Position.Y, Size.X, Size.Y), Color, Layer);
		}
		else
		{
			target.Draw(_surface,
				new Rect2(Position.X - target.Position.X, Position.Y - target.Position.Y, Size.X, Size.Y), Color, Layer);
		}

		if (EngineSettings.Instance.DebugDraw)
			BE.Renderer.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f, BoxColor.AllShades.Orange);
	}
}
