namespace Box.Inputs;

/// <summary>
/// Represents a default implementation of an input map.
/// </summary>
public class DefaultInputMap : InputMap
{
    /// <summary>
    /// Initializes the default input map.
    /// </summary>
	public DefaultInputMap()
	{
		Add(DefaultInputs.MoveLeft, KeyboardButton.A, KeyboardButton.Left, GamepadButton.DpadLeft);
        Add(DefaultInputs.MoveRight, KeyboardButton.D, KeyboardButton.Right, GamepadButton.DpadRight);
        Add(DefaultInputs.MoveUp, KeyboardButton.W, KeyboardButton.Up, GamepadButton.DpadUp);
        Add(DefaultInputs.MoveDown, KeyboardButton.S, KeyboardButton.Down, GamepadButton.DpadDown);
        Add(DefaultInputs.Accept, KeyboardButton.E, GamepadButton.A);
        Add(DefaultInputs.Cancel, KeyboardButton.F, GamepadButton.B);
        Add(DefaultInputs.Other, KeyboardButton.Q, GamepadButton.Y);
	}
}
