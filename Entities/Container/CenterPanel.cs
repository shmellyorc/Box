namespace Box.Entities.Container;

/// <summary>
/// Represents a panel that centers its child element within its own dimensions.
/// </summary>
public class CenterPanel : Panel
{
	private bool _roundedToPixel = true;
	private Vect2 _offset = Vect2.Zero;

	/// <summary>
	/// Gets or sets whether the panel should be rounded to the nearest pixel.
	/// </summary>
	public bool RoundedToPixel
	{
		get => _roundedToPixel;
		set
		{
			var oldValue = _roundedToPixel;
			_roundedToPixel = value;

			if (_roundedToPixel != oldValue)
			{
				IsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the offset of the panel relative to its parent or container.
	/// </summary>
	public Vect2 Offset
	{
		get => _offset;
		set
		{
			var oldValue = _offset;
			_offset = value;

			if (_offset != oldValue)
			{
				IsDirty = true;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the CenterPanel class with the specified child entities.
	/// </summary>
	/// <param name="children">The child entities to be added to the panel.</param>
	public CenterPanel(params Entity[] children) : base(children) { }
	public CenterPanel() : this(Array.Empty<Entity>()) { }


	/// <summary>
	/// Updates the internal state of the panel based on layout changes or content updates.
	/// </summary>
	protected override void Update()
	{
		if (GetService<EngineSettings>().DebugDraw)
			Renderer.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f, BoxColor.AllShades.GrassGreen);

		base.Update();
	}

	/// <summary>
	/// Updates the dirty state of the panel, indicating changes that require rendering or layout updates.
	/// </summary>
	protected override void UpdateDirtyState()
	{
		UpdateEntities(Children.ToArray());

		base.UpdateDirtyState();
	}

	private unsafe void UpdateEntities(Entity[] children)
	{
		if (children.Length == 0)
			return;

		// fixed (Entity* ptr = children)
		// {
		for (int i = 0; i < children.Length; i++)
		{
			// var item = ptr + i;
			var item = children[i];

			item.Position = Vect2.Center(Size, item.Size, _roundedToPixel) + _offset;
		}
		// }
	}
}
