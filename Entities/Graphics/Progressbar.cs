namespace Box.Entities.Graphics;

/// <summary>
/// Represents a graphical progress bar entity.
/// </summary>
public class Progressbar : Entity
{
	private Surface _bgSurface, _fgSurface;
	private float _value = 0f;

	/// <summary>
	/// Background color of the progress bar.
	/// </summary>
	public BoxColor BgColor = BoxColor.AllShades.White;

	/// <summary>
	/// Foreground color of the progress bar.
	/// </summary>
	public BoxColor FgColor = BoxColor.AllShades.Blue;

	/// <summary>
	/// Type of the progress bar (direction of progress).
	/// </summary>
	public ProgressBarType Type = ProgressBarType.LeftToRight;


	/// <summary>
	/// Minimum value of the progress bar.
	/// </summary>
	public float Min { get; set; } = 0f;

	/// <summary>
	/// Maximum value of the progress bar.
	/// </summary>
	public float Max { get; set; } = 1f;

	/// <summary>
	/// Current value of the progress bar.
	/// </summary>
	public float Value
	{
		get => _value;
		set
		{
			_value = value;

			UpdateProgres();
		}
	}

	/// <summary>
	/// Constructor for the Progressbar.
	/// </summary>
	public Progressbar() { }

	/// <summary>
	/// Method called when the entity enters its active state.
	/// </summary>
	protected override void OnEnter()
	{
		_bgSurface = new Surface(1, 1, BoxColor.AllShades.White);
		_fgSurface = new Surface(1, 1, BoxColor.AllShades.White);

		base.OnEnter();
	}

	/// <summary>
	/// Update method called every frame to update the entity's state.
	/// </summary>
	protected override void Update()
	{
		if (_bgSurface == null)
			return;
		if (_fgSurface == null)
			return;
		if (Size == Vect2.Zero)
			return;
		if (!Visible)
			return;
		if (FgColor.Alpha == 0)
			return;

		UpdateProgres();

		switch (Type)
		{
			case ProgressBarType.LeftToRight:
				DrawLeftToRight();
				break;
			case ProgressBarType.RightToLeft:
				DrawRightToLeft();
				break;
			case ProgressBarType.TopToBottom:
				DrawTopToBottom();
				break;
			case ProgressBarType.BottomToTop:
				DrawBottmToTop();
				break;
		}

		base.Update();
	}

	private void DrawLeftToRight()
	{
		var width = (_value / MathF.Max(Max - Min, 0)) * Size.X;

		if (!AnyParentOfType<RenderTarget>(out var target))
		{
			var region = new Rect2(Position, new Vect2(width, Size.Y));

			Renderer.Draw(_bgSurface, Bounds, BgColor, Layer);
			Renderer.Draw(_fgSurface, region, FgColor, Layer);
		}
		else
		{
			var bounds = new Rect2(Bounds.Position - target.Position, Bounds.Size);
			var region = new Rect2(Position - target.Position, new Vect2(width, Size.Y));

			target.Draw(_bgSurface, bounds, BgColor, Layer);
			target.Draw(_fgSurface, region, FgColor, Layer);
		}
	}

	private void DrawRightToLeft()
	{
		var width = (_value / MathF.Max(Max - Min, 0)) * Size.X;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			var region = new Rect2(new Vect2((Position.X + Size.X) - width, Position.Y), new Vect2(width, Size.Y));

			Renderer.Draw(_bgSurface, Bounds, BgColor, Layer);
			Renderer.Draw(_fgSurface, region, FgColor, Layer);
		}
		else
		{
			var position = Bounds.Position - target.Position;
			var bounds = new Rect2(new Vect2((position.X + Size.X) - width, position.Y), Bounds.Size);
			var region = new Rect2(Position - target.Position, new Vect2(width, Size.Y));

			target.Draw(_bgSurface, bounds, BgColor, Layer);
			target.Draw(_fgSurface, region, FgColor, Layer);
		}
	}

	private void DrawTopToBottom()
	{
		var height = (_value / MathF.Max(Max - Min, 0)) * Size.Y;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			var region = new Rect2(Position, new Vect2(Size.X, height));

			Renderer.Draw(_bgSurface, Bounds, BgColor, Layer);
			Renderer.Draw(_fgSurface, region, FgColor, Layer);
		}
		else
		{
			var bounds = new Rect2(Bounds.Position - target.Position, Bounds.Size);
			var region = new Rect2(Position - target.Position, new Vect2(Size.X, height));

			target.Draw(_bgSurface, bounds, BgColor, Layer);
			target.Draw(_fgSurface, region, FgColor, Layer);
		}
	}

	private void DrawBottmToTop()
	{
		var height = (_value / MathF.Max(Max - Min, 0)) * Size.Y;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			var region = new Rect2(new Vect2(Position.X, (Position.Y + Size.Y) - height), new Vect2(Size.X, height));

			Renderer.Draw(_bgSurface, Bounds, BgColor, Layer);
			Renderer.Draw(_fgSurface, region, FgColor, Layer);
		}
		else
		{
			var position = Bounds.Position - target.Position;
			var bounds = new Rect2(new Vect2(position.X, (position.Y + Size.Y) - height), Bounds.Size);
			var region = new Rect2(Position - target.Position, new Vect2(Size.X, height));

			target.Draw(_bgSurface, bounds, BgColor, Layer);
			target.Draw(_fgSurface, region, FgColor, Layer);
		}
	}

	private void UpdateProgres()
	{
		if (Min > Max)
			Min = MathF.Max(Min - 1, 0);
		if (Max < Min)
			Max = Min + 1;

		_value = Math.Clamp(_value, Min, Max);
	}
}
