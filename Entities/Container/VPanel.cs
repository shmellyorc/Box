namespace Box.Entities.Container;

/// <summary>
/// Represents a panel that aligns entities vertically within its layout.
/// </summary>
public class VPanel : Panel
{
    private int _spacing;
    private HAlign _hAlign = HAlign.Left;
    private VAlign _vAlign = VAlign.Top;
    private bool _autoSize = true;

    /// <summary>
    /// Gets or sets a value indicating whether the panel adjusts its size based on its content.
    /// </summary>
    public bool AutoSize
    {
        get => _autoSize;
        set
        {
            var oldValue = _autoSize;
            _autoSize = value;

            if (_autoSize != oldValue)
                IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the vertical alignment of the panel within its layout space.
    /// </summary>
    public VAlign VAlign
    {
        get => _vAlign;
        set
        {
            var oldValue = _vAlign;
            _vAlign = value;

            if (_vAlign != oldValue)
                IsDirty = true;
        }
    }


    /// <summary>
    /// Gets or sets the horizontal alignment of the panel within its layout space.
    /// </summary>
    public HAlign HAlign
    {
        get => _hAlign;
        set
        {
            var oldValue = _hAlign;
            _hAlign = value;

            if (_hAlign != oldValue)
                IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the horizontal alignment of the panel within its layout space.
    /// </summary>
    public int Spacing
    {
        get => _spacing;
        set
        {
            var oldValue = _spacing;
            _spacing = value;

            if (_spacing != oldValue)
                IsDirty = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of the VPanel class with the specified spacing and child entities.
    /// </summary>
    /// <param name="spacing">The spacing between child entities.</param>
    /// <param name="children">The child entities to be added to the panel.</param>
    public VPanel(int spacing, params Entity[] children) : base(children)
    {
        _spacing = spacing;

        Resize(children);
    }

    /// <summary>
    /// Initializes a new instance of the VPanel class with the default spacing and specified child entities.
    /// </summary>
    /// <param name="children">The child entities to be added to the panel.</param>
    public VPanel(params Entity[] children) : this(4, children) { }

    /// <summary>
    /// Updates the internal state of the panel based on changes to its layout or content.
    /// </summary>
    protected override void UpdateDirtyState()
    {
        UpdateEntities(Children.ToArray());
        Resize(Children);

        base.UpdateDirtyState();
    }

    private unsafe void UpdateEntities(Entity[] children)
    {
        // var offsetY = 0f;
        var items = children.Where(x => x.Visible).ToArray();
        var height = items.Sum(x => x.Size.Y + _spacing) - _spacing;

        // switch (_vAlign)
        // {
        //     case VAlign.Top:
        //         offsetY = 0f;
        //         break;
        //     case VAlign.Center:
        //         offsetY = Vect2.Center(Size.Y, height, false);
        //         break;
        //     case VAlign.Bottom:
        //         offsetY = Size.Y - height;
        //         break;
        // }
        var offsetY = AlignmentHelpers.AlignHeight(Size.Y, height, _vAlign);

        fixed (Entity* ptr = children)
        {
            for (int i = 0; i < children.Length; i++)
            {
                var item = ptr + i;

                // switch (_hAlign)
                // {
                //     case HAlign.Left:
                //         item->Position = new Vect2(0, offsetY);
                //         break;
                //     case HAlign.Center:
                //         item->Position = new Vect2(Vect2.Center(Size.X, item->Size.X, false), offsetY);
                //         break;
                //     case HAlign.Right:
                //         item->Position = new Vect2(Size.X - item->Size.X, offsetY);
                //         break;
                // }
                item->Position = new Vect2(AlignmentHelpers.AlignWidth(Size.X, item->Size.X, _hAlign), offsetY);

                if (*item != Children.Last())
                    offsetY += item->Size.Y + _spacing;
                else
                    offsetY += item->Size.Y;
            }
        }
    }

    private void Resize(IEnumerable<Entity> children)
    {
        if (!_autoSize)
            return;
        if (!children.Any())
            return;

        var entities = children
            .Where(x => x.Visible);

        if (!entities.Any())
            return;

        var width = entities.Max(x => x.Size.X);
        var height = entities.Sum(x => x.Size.Y) + (Math.Max(entities.Count() - 1, 0) * _spacing);

        Size = new(width, height);
    }
}
