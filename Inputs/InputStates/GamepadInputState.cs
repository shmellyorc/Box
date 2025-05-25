namespace Box.Inputs.InputStates;

internal readonly struct GamepadInputState : IInputState
{
    public readonly Enum Button { get; }
    public readonly bool IsEmpty => Button is null;

    public GamepadInputState(GamepadButton button) => Button = button;
}
