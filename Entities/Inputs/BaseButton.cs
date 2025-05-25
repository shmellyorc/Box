namespace Box.Entities.Inputs;

/// <summary>
/// A basic button that contains no texture but useful to detect mouse enter, mouse exit, pressed, and released events.
/// </summary>
public class BaseButton : Entity
{
    private enum BaseButtonState { None, Normal, Down, Disabled }

    private bool _pressed, _click, _initialized, _hover;
    private bool _enabled = true;
    private BaseButtonState _state;

    /// <summary>
    /// Represents the click state triggered on press or release events.
    /// </summary>
    public BaseButtonClickState ClickState = BaseButtonClickState.Released;

    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    public Action<BaseButton> Click;

    /// <summary>
    /// Gets or sets a value indicating whether the button is enabled.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;

            if (_initialized)
            {
                if (_enabled)
                    SetState(BaseButtonState.Normal, OnNormal);
                else
                    SetState(BaseButtonState.Disabled, OnDisabled);
            }
        }
    }

    /// <summary>
    /// Called when the button is released or normal button state.
    /// </summary>
    protected virtual void OnNormal() { }

    /// <summary>
    /// Called when the button is pressed.
    /// </summary>
    protected virtual void OnDown() { }

    /// <summary>
    /// Called when the button is disabled.
    /// </summary>
    protected virtual void OnDisabled() { }

    /// <summary>
    /// Called when the mouse pointer hovers over the button.
    /// </summary>
    protected virtual void OnHoverEnter() { }

    /// <summary>
    /// Called when the mouse pointer exits the button's area.
    /// </summary>
    protected virtual void OnHoverExit() { }

    /// <summary>
    /// Called when the button is clicked.
    /// </summary>
    protected virtual void OnClick() => Click?.Invoke(this);

    /// <summary>
    /// Called when the entity is added to the screen.
    /// </summary>
    protected override void OnEnter()
    {
        if (_enabled)
            SetState(BaseButtonState.Normal, OnNormal);
        else
            SetState(BaseButtonState.Disabled, OnDisabled);

        _initialized = true;

        base.OnEnter();
    }

    /// <summary>
    /// Updates the button's state.
    /// </summary>
    protected override void Update()
    {
        if (!_enabled)
            return;

        var position = Vect2.Transform(Input.MousePosition, Camera);

        if (Bounds.Contains(position))
        {
            if (!_hover)
            {
                OnHoverEnter();
                _hover = true;
            }

            if (Input.IsMousePressed(MouseButton.Left))
            {
                if (!_pressed)
                {
                    SetState(BaseButtonState.Down, OnDown);

                    if (ClickState == BaseButtonClickState.Pressed && !_click)
                    {
                        OnClick();
                    }

                    _pressed = true;
                    _click = true;
                }
            }
            else if (Input.IsMouseReleased(MouseButton.Left))
            {
                if (_pressed)
                {
                    SetState(BaseButtonState.Normal, OnNormal);

                    if (ClickState == BaseButtonClickState.Released && _click)
                    {
                        OnClick();
                    }

                    _click = false;
                    _pressed = false;
                    _hover = false;
                }
            }
        }
        else
        {
            if (_hover)
            {
                OnHoverExit();

                _hover = false;
            }

            if (Input.IsMouseReleased(MouseButton.Left))
            {
                if (_pressed)
                {
                    SetState(BaseButtonState.Normal, OnNormal);

                    if (ClickState == BaseButtonClickState.Released && _click)
                    {
                        OnClick();
                    }

                    _click = false;
                    _pressed = false;
                }
            }
        }

        base.Update();
    }

    private void SetState(BaseButtonState state, Action execute)
    {
        if (_state == state)
            return;

        execute.Invoke();

        _state = state;
    }
}
