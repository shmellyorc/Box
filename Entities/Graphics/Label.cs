namespace Box.Entities.Graphics;

/// <summary>
/// Represents a graphical label control for displaying text with specified alignment.
/// </summary>
public class Label : Entity
{
	private readonly BoxFont _font;
	private readonly List<string> _text = new();
	private string _displayText = string.Empty;
	private bool _dirtyFlag; // Don't change to _isDirty. That is used internally.
	private int _visibleLength = -1;

	/// <summary>
	/// Vertical alignment of the label within its container.
	/// </summary>
	public VAlign VAlign = VAlign.Top;

	/// <summary>
	/// Horizontal alignment of the label within its container.
	/// </summary>
	public HAlign HAlign = HAlign.Left;

	/// <summary>
	/// Color of the label text.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Offset position of the label relative to its default position.
	/// </summary>
	public Vect2 Offset = Vect2.Zero;

	/// <summary>
	/// Gets the length of the displayed text in the label.
	/// </summary>
	public int Length => _displayText.Length;

	/// <summary>
	/// Determines whether shadow effect is applied to the label text.
	/// </summary>
	public bool Shadow = false;

	/// <summary>
	/// Color of the shadow applied to the label text.
	/// </summary>
	public BoxColor ShadowColor = BoxColor.ShadesOfTransparency.Black25Transparent;

	/// <summary>
	/// Offset of the shadow relative to the text's position.
	/// </summary>
	public Vect2 ShadowOffset = Vect2.One;

	/// <summary>
	/// Gets or sets the visible length of the displayed text in the label.
	/// </summary>
	public int VisibleLength
	{
		get => _visibleLength;
		set
		{
			var oldValue = _visibleLength;
			_visibleLength = Math.Clamp(value, -1, _displayText.Length);

			if (_visibleLength != oldValue)
				_dirtyFlag = true;
		}
	}

	/// <summary>
	/// Gets or sets the size of the label, represented as a 2D vector.
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
				_dirtyFlag = true;

				foreach (var item in GetParents<Entity>())
				{
					if (item is Panel panel)
						panel.IsDirty = true;
					else
						item._isDirty = true;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the text content displayed by the label.
	/// </summary>
	public string Text
	{
		get => _displayText;
		set
		{
			var oldText = _displayText;

			_displayText = value.IsEmpty() ? string.Empty : value;

			if (_displayText != oldText)
			{
				_dirtyFlag = true;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the Label class with the specified font.
	/// </summary>
	/// <param name="font">The font used for displaying text in the label.</param>
	public Label(BoxFont font) => _font = font;

	/// <summary>
	/// Updates the label's internal state and appearance.
	/// </summary>
	protected unsafe override void Update()
	{
		if (_dirtyFlag)
		{
			if (_text.Count > 0)
				_text.Clear();

			_visibleLength = Math.Clamp(_visibleLength, -1, _displayText.Length);

			var formatText = _font.FormatText(_visibleLength == -1
				? _displayText
				: string.Join(string.Empty, _displayText.Take(_visibleLength)), Size.X
			);

			var split = formatText
				.Split(new[] { '\r', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			fixed (string* ptr = split)
			{
				for (var i = 0; i < split.Length; i++)
				{
					var item = ptr + i;

					_text.Add(item->Trim());
				}
			}

			_dirtyFlag = false;
		}

		if (Size == Vect2.Zero)
			return;
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;
		if (_text.Count == 0)
			return;

		var lineOffset = 0f;

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			fixed (string* ptr = _text.ToArray())
			{
				for (var i = 0; i < _text.Count; i++)
				{
					var item = ptr + i;

					if (Shadow && ShadowColor.Alpha > 0 && ShadowOffset != Vect2.Zero)
					{
						Renderer.DrawText(_font, *item,
							new Vect2(Position.X + Offset.X + ShadowOffset.X, Position.Y + lineOffset + Offset.Y + ShadowOffset.Y) + SetAlignment(*item), ShadowColor, Layer);
					}

					Renderer.DrawText(_font, *item, new Vect2(Position.X + Offset.X, Position.Y + lineOffset + Offset.Y) + SetAlignment(*item), Color, Layer);

					lineOffset += _font.GetTextHeight();
				}
			}
		}
		else
		{
			fixed (string* ptr = _text.ToArray())
			{
				for (var i = 0; i < _text.Count; i++)
				{
					var item = ptr + i;

					if (Shadow && ShadowColor.Alpha > 0 && ShadowOffset != Vect2.Zero)
					{
						target.DrawText(_font, *item,
							new Vect2(Position.X - target.Position.X + Offset.X + ShadowOffset.X, Position.Y - target.Position.Y + lineOffset + Offset.Y + ShadowOffset.Y) + SetAlignment(*item), ShadowColor, Layer);
					}

					target.DrawText(_font, *item,
						new Vect2(Position.X - target.Position.X + Offset.X, Position.Y - target.Position.Y + lineOffset + Offset.Y) + SetAlignment(*item), Color, Layer);

					lineOffset += _font.GetTextHeight();
				}
			}
		}

		if (GetService<EngineSettings>().DebugDraw)
			Renderer.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f, BoxColor.AllShades.Yellow);
	}

	private Vect2 SetAlignment(string line)
	{
		var result = Vect2.Zero;
		// var lineGap = _text.Count == 0 ? 0 : Math.Max(_text.Count, 0) * _font.LineSpacing;
		var maxX = line.Length == 0 ? 0 : _font.MeasureWidth(line);
		var maxY = _text.Count == 0 ? 0 : _text.Sum(x => _font.MeasureHeight(x)) - _font.LineSpacing;

		if (maxX == 0 && maxY == 0)
			return result;

		result.X = AlignmentHelpers.AlignWidth(Size.X, maxX, HAlign);
		result.Y = AlignmentHelpers.AlignHeight(Size.Y, maxY, VAlign);

		return result;
	}
}
