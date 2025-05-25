namespace Box.Inputs.InputStates;

internal readonly struct MouseInputState : IInputState
{
    public readonly Enum Button { get; }
    public readonly bool IsEmpty => Button is null;

    public MouseInputState(MouseButton button) => Button = button;
}
