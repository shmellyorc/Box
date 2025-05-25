namespace Box.Inputs.InputStates;

internal readonly struct KeyboardInputState : IInputState
{
    public readonly Enum Button { get; }
    public readonly bool IsEmpty => Button is null;

    public KeyboardInputState(KeyboardButton button) => Button = button;
}
