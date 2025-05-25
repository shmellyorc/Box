namespace Box.Entities.Container;

/// <summary>
/// Represents a grid panel that arranges child entities by column count instead of width.
/// </summary>
public class GridPanel : Panel
{
	private int _columns = 1;
	private int _horizontalSpacing = 4;
	private int _verticalSpacing = 4;
	private bool _autoSize = true;
	private HAlign _hAlign = HAlign.Left;
	private VAlign _vAlign = VAlign.Top;

	/// <summary>
	/// Gets or sets the number of columns in the grid layout.
	/// <para>This defines how many items are placed per row before wrapping to the next row.</para>
	/// </summary>
	public int Columns
	{
		get => _columns;
		set
		{
			if (_columns != value)
			{
				_columns = Math.Max(1, value);
				IsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the spacing in pixels between entities along the horizontal axis.
	/// </summary>
	public int HorizontalSpacing
	{
		get => _horizontalSpacing;
		set { _horizontalSpacing = value; IsDirty = true; }
	}

	/// <summary>
	/// Gets or sets the spacing in pixels between entities along the vertical axis.
	/// </summary>
	public int VerticalSpacing
	{
		get => _verticalSpacing;
		set { _verticalSpacing = value; IsDirty = true; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether the grid box automatically resizes based on its content.
	/// <para>When enabled, the container will expand to fit all visible children.</para>
	/// </summary>
	public bool AutoSize
	{
		get => _autoSize;
		set { _autoSize = value; IsDirty = true; }
	}

	/// <summary>
	/// Gets or sets the horizontal alignment of each entity within its cell.
	/// </summary>
	public HAlign HAlign
	{
		get => _hAlign;
		set { _hAlign = value; IsDirty = true; }
	}

	/// <summary>
	/// Gets or sets the vertical alignment of each entity within its cell.
	/// </summary>
	public VAlign VAlign
	{
		get => _vAlign;
		set { _vAlign = value; IsDirty = true; }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GridPanel"/> class using the specified number of columns and optional child entities.
	/// </summary>
	/// <param name="columns">The number of columns to layout entities in.</param>
	/// <param name="children">The child entities to add to the grid.</param>
	public GridPanel(int columns, params Entity[] children) : base(children)
	{
		_columns = Math.Max(1, columns);
		
		Resize(children);
	}

	/// <summary>
	/// Updates the layout of the grid box when the state is marked as dirty.
	/// <para>This repositions children and resizes the container if <see cref="AutoSize"/> is enabled.</para>
	/// </summary>
	protected override void UpdateDirtyState()
	{
		UpdateEntities(Children.ToArray());

		Resize(Children);

		base.UpdateDirtyState();
	}

	private unsafe void UpdateEntities(Entity[] children)
	{
		var visible = children.Where(x => x.Visible).ToArray();

		fixed (Entity* ptr = visible)
		{
			for (int i = 0; i < visible.Length; i++)
			{
				var entity = ptr + i;
				int col = i % _columns;
				int row = i / _columns;

				float posX = col * (entity->Size.X + _horizontalSpacing);
				float posY = row * (entity->Size.Y + _verticalSpacing);

				posX += AlignmentHelpers.AlignWidth(entity->Size.X, entity->Size.X, _hAlign);
				posY += AlignmentHelpers.AlignHeight(entity->Size.Y, entity->Size.Y, _vAlign);

				entity->Position = new Vect2(posX, posY);
			}
		}
	}

	private void Resize(IEnumerable<Entity> children)
	{
		if (!_autoSize)
			return;

		var visible = children.Where(x => x.Visible).ToArray();

		if (visible.Length == 0)
			return;

		int rows = (int)Math.Ceiling(visible.Length / (float)_columns);
		float cellWidth = visible.Max(x => x.Size.X);
		float cellHeight = visible.Max(x => x.Size.Y);

		float totalWidth = (_columns * cellWidth) + ((_columns - 1) * _horizontalSpacing);
		float totalHeight = (rows * cellHeight) + ((rows - 1) * _verticalSpacing);

		Size = new Vect2(totalWidth, totalHeight);
	}
}
