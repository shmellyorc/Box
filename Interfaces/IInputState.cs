namespace Box.Interfaces;

internal interface IInputState
{
    bool IsEmpty { get; }
    Enum Button { get; }
}
