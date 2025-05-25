namespace Box.Entities.Container;

/// <summary>
/// Represents a panel that aligns entities horizontally within its layout.
/// </summary>
public class HPanel : Panel
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
    /// Initializes a new instance of the HPanel class with the specified spacing and child entities.
    /// </summary>
    /// <param name="spacing">The spacing between child entities.</param>
    /// <param name="children">The child entities to be added to the panel.</param>
    public HPanel(int spacing, params Entity[] children) : base(children)
    {
        _spacing = spacing;

        Resize(children);
    }


    /// <summary>
    /// Initializes a new instance of the HPanel class with the default spacing and specified child entities.
    /// </summary>
    /// <param name="children">The child entities to be added to the panel.</param>
    public HPanel(params Entity[] children) : this(4, children) { }


    /// <summary>
    /// Updates the internal state of the panel based on changes to its layout or content.
    /// </summary>
    protected override void UpdateDirtyState()
    {
        UpdateEntities(Children.ToArray());
        Resize(Children);

        base.UpdateDirtyState();
    }

    private unsafe void UpdateEntities(IEnumerable<Entity> children)
    {
        // var offsetX = 0f;
        var items = children.Where(x => x.Visible).ToArray();
        var width = items.Sum(x => x.Size.X + _spacing) - _spacing;

        // switch (_hAlign)
        // {
        //     case HAlign.Left:
        //         offsetX = 0;
        //         break;
        //     case HAlign.Center:
        //         offsetX = Vect2.Center(Size.X, width, false);
        //         break;
        //     case HAlign.Right:
        //         offsetX = Size.X - width;
        //         break;
        // }

        var offsetX = AlignmentHelpers.AlignWidth(Size.X, width, _hAlign);

        fixed (Entity* ptr = items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = ptr + i;

                // switch (_vAlign)
                // {
                //     case VAlign.Top:
                //         item->Position = new Vect2(offsetX, 0);
                //         break;
                //     case VAlign.Center:
                //         item->Position = new Vect2(offsetX, Vect2.Center(Size.Y, item->Size.Y, false));
                //         break;
                //     case VAlign.Bottom:
                //         item->Position = new Vect2(offsetX, Size.Y - item->Size.Y);
                //         break;
                // }
                item->Position = new Vect2(offsetX, AlignmentHelpers.AlignHeight(Size.Y, item->Size.Y, _vAlign));

                if (*item != items.Last())
                    offsetX += item->Size.X + _spacing;
                else
                    offsetX += item->Size.X;
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

        var width = entities.Sum(x => x.Size.X) + (Math.Max(entities.Count() - 1, 0) * _spacing);
        var height = entities.Max(x => x.Size.Y);

        Size = new(width, height);
    }
}
