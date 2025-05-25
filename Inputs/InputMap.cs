using Box.Resources;
using Box.Services.Types;

namespace Box.Inputs;



internal class ControllerMap
{
	// Map SDL’s button codes (like “a”, “b”, “y”) → SFML joystick button index
	public Dictionary<string, uint> ButtonIndex = new();
	// Map SDL’s axis codes (like “leftx+”, “leftx–”) → (SFML axis, direction)
	public Dictionary<string, (SFMLAxis axis, float sign)> AxisIndex = new();
}



/// <summary>
/// Represents a mapping of input controls for the game.
/// </summary>
public class InputMap : GameService
{
	private readonly Dictionary<uint, bool> _gamepadState = new();
	private readonly List<SFMLKeyboard.Key> _keyboardKeys = new();
	private readonly List<SFMLMouse.Button> _mouseKeys = new();
	private readonly List<GamepadButton> _gamepadKeys = new();
	private readonly Dictionary<string, List<IInputState>> _items = new();
	private readonly Dictionary<uint, ControllerMap> _activeMaps = new();
	private readonly Dictionary<string, ControllerMap> _controllerMaps = new();

	/// <summary>
	/// Gets the number of items in the input map.
	/// </summary>
	public int Count => _items.Count;

	/// <summary>
	/// Gets the current active input state (keyboard or gamepad).
	/// </summary>
	public ActiveInputState ActiveState { get; private set; }


	/// <summary>
	/// Gets the global mouse position relative to the monitor.
	/// </summary>
	public Vect2 GlobalMousePosition => new(SFMLMouse.GetPosition()); // rel: to the monitor

	/// <summary>
	/// Gets the mouse position relative to the game window.
	/// </summary>
	public Vect2 MousePosition => new(SFMLMouse.GetPosition(Engine._window)); // rel: to the window

	/// <summary>
	/// Indicates if any gamepad is currently connected.
	/// </summary>
	public bool AnyGamepadConnected => _gamepadState.Values.Any(x => x);

	// var value = Joystick.IsButtonPressed(0, 0); // A button (true/false)
	// var value = Joystick.IsButtonPressed(0, 1); // B Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 2); // X Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 3); // Y Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 4); // Left Bumper (true/false)
	// var value = Joystick.IsButtonPressed(0, 5); // Right Bumper (true/false)
	// var value = Joystick.IsButtonPressed(0, 6); // Back Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 7); // Start Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 8); // LeftThumbStick Button (true/false)
	// var value = Joystick.IsButtonPressed(0, 8); // RightThumbStick Button (true/false)
	private enum ButtonType { AButton, BButton, XButton, YButton, LeftBumper, RightBumper, BackButton, StartButton, LeftThumbstickButton, RightThumbstickButton }

	/// <summary>
	/// Initializes a new instance of the InputMap class.
	/// </summary>>
	public InputMap() { }

	public override void Initialize()
	{
		LoadControllerDbFromResource("gamecontrollerdb.txt");

		SFMLGamepad.Update();

		for (uint i = 0; i < SFMLGamepad.Count; i++)
			_gamepadState.Add(i, SFMLGamepad.IsConnected(i));

		base.Initialize();
	}

	internal void LoadInputs()
	{
		var e = GetService<Engine>();

		// Unbinds previous ones, if any:
		UnloadInputs();

		// Bind new ones:
		e._window.KeyReleased += (_, args) => _keyboardKeys.Remove(args.Code);
		e._window.MouseButtonReleased += (_, args) => _mouseKeys.Remove(args.Button);
		e._window.JoystickButtonReleased += (_, args) => OnJoystickRelease((ButtonType)args.Button);
		e._window.JoystickConnected += (_, args) => OnJoystickConnected(args.JoystickId);
		e._window.JoystickDisconnected += (_, args) => OnJoystickDisconnected(args.JoystickId);
	}

	internal void UnloadInputs()
	{
		var e = GetService<Engine>();

		e._window.KeyReleased -= (_, args) => _keyboardKeys.Remove(args.Code);
		e._window.MouseButtonReleased -= (_, args) => _mouseKeys.Remove(args.Button);
		e._window.JoystickButtonReleased -= (_, args) => OnJoystickRelease((ButtonType)args.Button);
		e._window.JoystickConnected -= (_, args) => OnJoystickConnected(args.JoystickId);
		e._window.JoystickDisconnected -= (_, args) => OnJoystickDisconnected(args.JoystickId);
	}

	/// <summary>
	/// Finalizes the InputMap instance.
	/// </summary>
	~InputMap() => UnloadInputs();


	#region Actions
	/// <summary>
	/// Adds a new input mapping with a specified name and keys.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <param name="keys">The keys associated with the input mapping.</param>
	public void Add(string name, params Enum[] keys)
	{
		if (!Exists(name))
			_items.Add(name, new List<IInputState>(ProcessInputs(keys)));
		else
			_items[name].AddRange(ProcessInputs(keys));
	}

	/// <summary>
	/// Adds a new input mapping with a specified name and keys.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <param name="keys">The keys associated with the input mapping.</param>
	public void Add(Enum name, params Enum[] keys) => Add(name.ToEnumString(), keys);

	/// <summary>
	/// Checks if an input mapping with the specified name exists.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <returns>True if the input mapping exists, otherwise false.</returns>
	public bool Exists(string name) => _items.ContainsKey(name);

	/// <summary>
	/// Checks if an input mapping with the specified name exists.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <returns>True if the input mapping exists, otherwise false.</returns>
	public bool Exists(Enum name) => Exists(name.ToEnumString());

	/// <summary>
	/// Removes an input mapping with the specified name.
	/// </summary>
	/// <param name="name">The name of the input mapping to remove.</param>
	/// <returns>True if the input mapping was successfully removed, otherwise false.</returns>
	public bool Remove(string name)
	{
		if (!Exists(name))
			return false;

		return _items.Remove(name);
	}

	/// <summary>
	/// Removes an input mapping with the specified name.
	/// </summary>
	/// <param name="name">The name of the input mapping to remove.</param>
	/// <returns>True if the input mapping was successfully removed, otherwise false.</returns>
	public bool Remove(Enum name) => Remove(name.ToEnumString());

	/// <summary>
	/// Removes a specific key from an input mapping with the specified name.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <param name="key">The key to remove from the input mapping.</param>
	/// <returns>True if the key was successfully removed, otherwise false.</returns>
	public bool Remove(string name, Enum key)
	{
		if (!Exists(name))
			return false;

		List<IInputState> values = _items[name]
			.FindAll(x => x.Button == key);

		if (!values.IsEmpty())
		{
			bool[] check = new bool[values.Count];
			bool[] result = new bool[values.Count];

			for (int i = 0; i < values.Count; i++)
			{
				check[i] = true;
				result[i] = _items[name].Remove(values[i]);
			}

			if (_items[name].Count == 0)
				_items.Remove(name);

			return result.SequenceEqual(result);
		}

		return false;
	}

	/// <summary>
	/// Removes a specific key from an input mapping with the specified name.
	/// </summary>
	/// <param name="name">The name of the input mapping.</param>
	/// <param name="key">The key to remove from the input mapping.</param>
	/// <returns>True if the key was successfully removed, otherwise false.</returns>
	public bool Remove(Enum name, Enum key) => Remove(name.ToEnumString(), key);

	/// <summary>
	/// Clears all items from the input map.
	/// </summary>
	public void Clear() => _items.Clear();



	/// <summary>
	/// Checks if the specified action name is in a "up" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionReleased(string name)
	{
		if (!Exists(name))
			return false;
		if (_items[name].Count == 0)
			return false;
		if (!Engine.IsActive)
			return false;

		foreach (var item in _items[name])
		{
			if (item is KeyboardInputState keyboard)
			{
				var value = IsKeyReleased((KeyboardButton)keyboard.Button);

				if (value)
					return true;
			}

			if (item is MouseInputState mouse)
			{
				var value = IsMouseReleased((MouseButton)mouse.Button);

				if (value)
					return true;
			}

			if (item is GamepadInputState gamepad)
			{
				var value = IsGamepadReleased((GamepadButton)gamepad.Button);

				if (value)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the specified action name is in a "up" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionReleased(Enum name) => IsActionReleased(name.ToEnumString());

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when an action is released.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action was released, 1 if the "right" action was released, 
	/// and 0 if neither or both were released.
	/// </returns>
	public float GetActionAxisReleased(string left, string right)
	{
		if (!Exists(left) || !Exists(right))
			return 0;

		bool leftDown = IsActionReleased(left);
		bool rightDown = IsActionReleased(right);

		if (leftDown && rightDown)
			return 0; // Neutral if both are pressed

		if (leftDown)
			return -1;

		if (rightDown)
			return 1;

		return 0;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when an action is released.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action was released, 1 if the "right" action was released, 
	/// and 0 if neither or both were released.
	/// </returns>
	public float GetActionAxisReleased(Enum left, Enum right) => GetActionAxisReleased(left.ToEnumString(), right.ToEnumString());


	/// <summary>
	/// Checks if the specified action name is in a "down" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionPressed(string name)
	{
		if (!Exists(name))
			return false;
		if (_items[name].Count == 0)
			return false;
		if (!Engine.IsActive)
			return false;

		foreach (var item in _items[name])
		{
			if (item is KeyboardInputState keyboard)
			{
				var value = IsKeyPressed((KeyboardButton)keyboard.Button);

				if (value)
					return true;
			}

			if (item is MouseInputState mouse)
			{
				var value = IsMousePressed((MouseButton)mouse.Button);

				if (value)
					return true;
			}

			if (item is GamepadInputState gamepad)
			{
				var value = IsGamepadPressed((GamepadButton)gamepad.Button);

				if (value)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the specified action name is in a "down" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionPressed(Enum name) => IsActionPressed(name.ToEnumString());

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) based on the state of two action inputs.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action is pressed, 1 if the "right" action is pressed, 
	/// and 0 if neither or both are pressed.
	/// </returns>
	public float GetActionAxisPressed(string left, string right)
	{
		if (!Exists(left) || !Exists(right))
			return 0;

		bool leftDown = IsActionPressed(left);
		bool rightDown = IsActionPressed(right);

		if (leftDown && rightDown)
			return 0; // Neutral if both are pressed

		if (leftDown)
			return -1;

		if (rightDown)
			return 1;

		return 0;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) based on the state of two action inputs.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action is pressed, 1 if the "right" action is pressed, 
	/// and 0 if neither or both are pressed.
	/// </returns>
	public float GetActionAxisPressed(Enum left, Enum right) => GetActionAxisPressed(left.ToEnumString(), right.ToEnumString());


	/// <summary>
	/// Checks if the specified action name is in a "single key pressed" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionJustPressed(string name)
	{
		if (!Exists(name))
			return false;
		if (_items[name].Count == 0)
			return false;
		if (!Engine.IsActive)
			return false;

		foreach (var item in _items[name])
		{
			if (item is KeyboardInputState keyboard)
			{
				var value = IsKeyJustPressed((KeyboardButton)keyboard.Button);

				if (value)
					return true;
			}

			if (item is MouseInputState mouse)
			{
				var value = IsMouseJustPressed((MouseButton)mouse.Button);

				if (value)
					return true;
			}

			if (item is GamepadInputState gamepad)
			{
				var value = IsGamepadJustPressed((GamepadButton)gamepad.Button);

				if (value)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the specified action name is in a "single key pressed" state, indicating the gamepad, keyboard, or mouse button is pressed.
	/// </summary>
	/// <param name="name">The name of the action to check.</param>
	/// <returns>True if the action is in a "down" state, false otherwise.</returns>
	public bool IsActionJustPressed(Enum name) => IsActionJustPressed(name.ToEnumString());

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when an action is pressed for the first time.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action was just pressed, 1 if the "right" action was just pressed, 
	/// and 0 if neither or both were pressed simultaneously.
	/// </returns>
	public float GetActionAxisJustPressed(string left, string right)
	{
		if (!Exists(left) || !Exists(right))
			return 0;

		bool leftDown = IsActionJustPressed(left);
		bool rightDown = IsActionJustPressed(right);

		if (leftDown && rightDown)
			return 0; // Neutral if both are pressed

		if (leftDown)
			return -1;

		if (rightDown)
			return 1;

		return 0;
	}


	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when an action is pressed for the first time.
	/// </summary>
	/// <param name="left">The action name for the negative direction (e.g., "left").</param>
	/// <param name="right">The action name for the positive direction (e.g., "right").</param>
	/// <returns>
	/// Returns -1 if the "left" action was just pressed, 1 if the "right" action was just pressed, 
	/// and 0 if neither or both were pressed simultaneously.
	/// </returns>
	public float GetActionAxisJustPressed(Enum left, Enum right) => GetActionAxisJustPressed(left.ToEnumString(), right.ToEnumString());
	#endregion


	#region Keyboard
	/// <summary>
	/// Checks if the specified keyboard button is currently pressed down.
	/// </summary>
	/// <param name="button">The keyboard button to check.</param>
	/// <returns>True if the keyboard button is down, false otherwise.</returns>
	public bool IsKeyPressed(KeyboardButton button)
	{
		if (!Engine.IsActive)
			return false;

		var result = SFMLKeyboard.IsKeyPressed((SFMLKeyboard.Key)button);

		if (result)
			ActiveState = ActiveInputState.Keyboard;

		return result;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) while a keyboard key is being held down.
	/// </summary>
	/// <param name="left">The keyboard button for the negative direction (e.g., move left).</param>
	/// <param name="right">The keyboard button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" key is currently being held down, 1 if the "right" key is being held down, 
	/// and 0 if neither or both keys are held at the same time.
	/// </returns>
	public float GetKeyAxisPressed(KeyboardButton left, KeyboardButton right)
	{
		// Check if the keys were just released
		bool leftPressed = IsKeyPressed(left);
		bool rightPressed = IsKeyPressed(right);

		if (leftPressed && rightPressed)
			return 0;

		if (leftPressed)
			return -1;

		if (rightPressed)
			return 1;

		return 0;
	}

	/// <summary>
	/// Checks if the specified keyboard button is currently released (not pressed down).
	/// </summary>
	/// <param name="button">The keyboard button to check.</param>
	/// <returns>True if the keyboard button is up, false otherwise.</returns>
	public bool IsKeyReleased(KeyboardButton button)
	{
		if (!Engine.IsActive)
			return false;

		return !IsKeyPressed(button);
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when a keyboard key is released.
	/// </summary>
	/// <param name="left">The keyboard button for the negative direction (e.g., move left).</param>
	/// <param name="right">The keyboard button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" key was just released, 1 if the "right" key was just released, 
	/// and 0 if neither or both keys were released simultaneously.
	/// </returns>
	public float GetKeyAxisReleased(KeyboardButton left, KeyboardButton right)
	{
		// Check if the keys were just released
		bool leftReleased = IsKeyReleased(left);
		bool rightReleased = IsKeyReleased(right);

		if (leftReleased && rightReleased)
			return 0; // Neutral if both are released at the same time

		if (leftReleased)
			return -1;

		if (rightReleased)
			return 1;

		return 0;
	}

	/// <summary>
	/// Checks if the specified keyboard button was pressed in the current frame.
	/// </summary>
	/// <param name="button">The keyboard button to check.</param>
	/// <returns>True if the keyboard button was pressed in the current frame, false otherwise.</returns>
	public bool IsKeyJustPressed(KeyboardButton button)
	{
		if (!Engine.IsActive)
			return false;
		if (IsKeyReleased(button))
			return false;

		var result = _keyboardKeys.Any(x => x == (SFMLKeyboard.Key)button);

		if (!result)
		{
			_keyboardKeys.Add((SFMLKeyboard.Key)button);

			ActiveState = ActiveInputState.Keyboard;
		}

		return !result;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when a keyboard key is just pressed.
	/// </summary>
	/// <param name="left">The keyboard button for the negative direction (e.g., move left).</param>
	/// <param name="right">The keyboard button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" key was just pressed, 1 if the "right" key was just pressed, 
	/// and 0 if neither or both keys were pressed simultaneously.
	/// </returns>
	public float GetKeyAxisJustPressed(KeyboardButton left, KeyboardButton right)
	{
		// Check if the keys were just released
		bool leftJustPressed = IsKeyJustPressed(left);
		bool rightJustPressed = IsKeyJustPressed(right);

		if (leftJustPressed && rightJustPressed)
			return 0;

		if (leftJustPressed)
			return -1;

		if (rightJustPressed)
			return 1;

		return 0;
	}
	#endregion


	#region Mouse
	/// <summary>
	/// Checks if the specified mouse button is currently pressed down.
	/// </summary>
	/// <param name="button">The mouse button to check.</param>
	/// <returns>True if the mouse button is down, false otherwise.</returns>
	public bool IsMousePressed(MouseButton button)
	{
		if (!Engine.IsActive)
			return false;

		return SFMLMouse.IsButtonPressed((SFMLMouse.Button)button);
	}

	/// <summary>
	/// Checks if the specified mouse button is currently released (not pressed down).
	/// </summary>
	/// <param name="button">The mouse button to check.</param>
	/// <returns>True if the mouse button is up, false otherwise.</returns>
	public bool IsMouseReleased(MouseButton button)
	{
		if (!Engine.IsActive)
			return false;

		return !IsMousePressed(button);
	}

	/// <summary>
	/// Checks if the specified mouse button was pressed in the current frame.
	/// </summary>
	/// <param name="button">The mouse button to check.</param>
	/// <returns>True if the mouse button was pressed in the current frame, false otherwise.</returns>
	public bool IsMouseJustPressed(MouseButton button)
	{
		if (!Engine.IsActive)
			return false;
		if (IsMouseReleased(button))
			return false;

		var result = _mouseKeys.Any(x => x == (SFMLMouse.Button)button);

		if (!result)
			_mouseKeys.Add((SFMLMouse.Button)button);

		return !result;
	}
	#endregion


	#region Gamepad
	/// <summary>
	/// Checks if the specified gamepad button is currently released (not pressed down).
	/// </summary>
	/// <param name="button">The gamepad button to check.</param>
	/// <returns>True if the gamepad button is up, false otherwise.</returns>
	public bool IsGamepadReleased(GamepadButton button) => !IsGamepadPressed(button);

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when a gamepad button is released.
	/// </summary>
	/// <param name="left">The gamepad button for the negative direction (e.g., move left).</param>
	/// <param name="right">The gamepad button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" button was just released, 1 if the "right" button was just released, 
	/// and 0 if neither or both buttons were released simultaneously.
	/// </returns>
	public float GetGamepadAxisReleased(GamepadButton left, GamepadButton right)
	{
		// Check if the keys were just released
		bool leftReleased = IsGamepadReleased(left);
		bool rightReleased = IsGamepadReleased(right);

		if (leftReleased && rightReleased)
			return 0;

		if (leftReleased)
			return -1;

		if (rightReleased)
			return 1;

		return 0;
	}

	/// <summary>
	/// Checks if the specified gamepad button was pressed in the current frame.
	/// </summary>
	/// <param name="button">The gamepad button to check.</param>
	/// <returns>True if the gamepad button was pressed in the current frame, false otherwise.</returns>
	public bool IsGamepadJustPressed(GamepadButton button)
	{
		if (!Engine.IsActive)
			return false;
		if (IsGamepadReleased(button))
			return false;

		var result = _gamepadKeys.Any(x => x == button);

		if (!result)
		{
			ActiveState = ActiveInputState.Gamepad;

			_gamepadKeys.Add(button);
		}

		return !result;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) when a gamepad button is just pressed.
	/// </summary>
	/// <param name="left">The gamepad button for the negative direction (e.g., move left).</param>
	/// <param name="right">The gamepad button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" button was just pressed, 1 if the "right" button was just pressed, 
	/// and 0 if neither or both buttons were pressed simultaneously.
	/// </returns>
	public float GetGamepadAxisJustPressed(GamepadButton left, GamepadButton right)
	{
		// Check if the keys were just released
		bool leftJustPressed = IsGamepadJustPressed(left);
		bool rightJustPressed = IsGamepadJustPressed(right);

		if (leftJustPressed && rightJustPressed)
			return 0;

		if (leftJustPressed)
			return -1;

		if (rightJustPressed)
			return 1;

		return 0;
	}

	/// <summary>
	/// Checks if the specified gamepad button is currently pressed down.
	/// </summary>
	/// <param name="button">The gamepad button to check.</param>
	/// <returns>True if the gamepad button is down, false otherwise.</returns>
	public bool IsGamepadPressed(GamepadButton button)
	{
		if (!Engine.IsActive)
			return false;
		if (!AnyGamepadConnected)
			return false;

		SFMLGamepad.Update();

		var settings = GetService<EngineSettings>();
		var deadzoneThreshold = settings.GamepadDeadzone * 100f;

		foreach (var joy in _gamepadState)
		{
			var joyId = joy.Key;
			if (!joy.Value) continue; // Joystick not connected

			// look up the per-device mapping
			_activeMaps.TryGetValue(joyId, out var map);

			// 1) try button map
			var keyName = button.ToString().ToLowerInvariant(); // e.g. "a", "b", "dpadup"
			if (map?.ButtonIndex.TryGetValue(keyName, out var btnIdx) == true)
			{
				if (SFMLGamepad.IsButtonPressed(joyId, btnIdx))
				{
					ActiveState = ActiveInputState.Gamepad;
					return true;
				}
			}
			// 2) or try axis map
			else if (map?.AxisIndex.TryGetValue(keyName, out var axInfo) == true)
			{
				var pos = SFMLGamepad.GetAxisPosition(joyId, axInfo.axis) * axInfo.sign;
				if (pos > (settings.GamepadDeadzone * 100f))
				{
					ActiveState = ActiveInputState.Gamepad;
					return true;
				}
			}
			else
			{
				// fallback to your old literal mapping:
				bool value = button switch
				{
					GamepadButton.LeftThumbstickLeft or GamepadButton.LeftThumbstickRight or GamepadButton.LeftThumbstickUp or GamepadButton.LeftThumbstickDown or
					GamepadButton.RightThumbstickLeft or GamepadButton.RightThumbstickRight or GamepadButton.RightThumbstickUp or GamepadButton.RightThumbstickDown or
					GamepadButton.LeftTrigger or GamepadButton.RightTrigger => GetGamepadForce(button) > 0f,

					GamepadButton.DpadLeft => (SFMLGamepad.GetAxisPosition(0, SFMLAxis.PovX) * -1f) > deadzoneThreshold, // Thumbstick (left-right) (-100 or 100)
					GamepadButton.DpadRight => SFMLGamepad.GetAxisPosition(0, SFMLAxis.PovX) > deadzoneThreshold, // Thumbstick (left-right) (-100 or 100)
					GamepadButton.DpadUp => SFMLGamepad.GetAxisPosition(0, SFMLAxis.PovY) > deadzoneThreshold, // Thumbstick (up-down) (100 or -100)
					GamepadButton.DpadDown => (SFMLGamepad.GetAxisPosition(0, SFMLAxis.PovY) * -1f) > deadzoneThreshold, // Thumbstick (up-down) (100 or -100)
					GamepadButton.A => SFMLGamepad.IsButtonPressed(0, 0), // A button (true/false),
					GamepadButton.B => SFMLGamepad.IsButtonPressed(0, 1), // B Button (true/false)
					GamepadButton.X => SFMLGamepad.IsButtonPressed(0, 2), // X Button (true/false)
					GamepadButton.Y => SFMLGamepad.IsButtonPressed(0, 3), // Y Button (true/false),
					GamepadButton.LeftBumper => SFMLGamepad.IsButtonPressed(0, 4), // Left Bumper (true/false)
					GamepadButton.RightBumper => SFMLGamepad.IsButtonPressed(0, 5), // Right Bumper (true/false)
					GamepadButton.Back => SFMLGamepad.IsButtonPressed(0, 6), // Back Button (true/false)
					GamepadButton.Start => SFMLGamepad.IsButtonPressed(0, 7), // Start Button (true/false)
					GamepadButton.LeftThumbstickButton => SFMLGamepad.IsButtonPressed(0, 8), // Left Thumbstick Button (true/false)
					GamepadButton.RightThumbstickButton => SFMLGamepad.IsButtonPressed(0, 9), // Right Thumbstic Button (true/false)
					_ => false
				};

				if (value)
				{
					ActiveState = ActiveInputState.Gamepad;

					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Gets an axis value (-1, 0, or 1) while a gamepad button is being held down.
	/// </summary>
	/// <param name="left">The gamepad button for the negative direction (e.g., move left).</param>
	/// <param name="right">The gamepad button for the positive direction (e.g., move right).</param>
	/// <returns>
	/// Returns -1 if the "left" button is currently being held down, 1 if the "right" button is being held down, 
	/// and 0 if neither or both buttons are held at the same time.
	/// </returns>
	public float GetGamepadAxisPressed(GamepadButton left, GamepadButton right)
	{
		// Check if the keys were just released
		bool leftPressed = IsGamepadPressed(left);
		bool rightPressed = IsGamepadPressed(right);

		if (leftPressed && rightPressed)
			return 0;

		if (leftPressed)
			return -1;

		if (rightPressed)
			return 1;

		return 0;
	}

	/// <summary>
	/// Gets the thumbstick or trigger force press from 0.0 to 1.0. The more you press into the trigger, the higher the force increases.
	/// </summary>
	/// <param name="button">The gamepad button to check the force for.</param>
	/// <returns>The force of the gamepad button press, ranging from 0.0 (not pressed) to 1.0 (fully pressed).</returns>
	public float GetGamepadForce(GamepadButton button)
	{
		if (!Engine.IsActive)
			return 0f;
		if (!AnyGamepadConnected)
			return 0f;
		if (button == GamepadButton.None)
			return 0f;

		SFMLGamepad.Update();

		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.X); // Left Thumbstick (left-right) (-100 to 100) *
		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.Y); // Left Thumbstick (up-down) (-100 to 100) *
		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.U); // Right Thumbstick (left-right) (-100 to 100)
		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.V); // Right Thumbstick (up-down) (-100 to 100)
		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.Z); // Left Trigger (0 to 100), Right Trigger (0 to -100)

		// var value = Joystick.IsButtonPressed(0, 0); // A button (true/false)
		// var value = Joystick.IsButtonPressed(0, 1); // B Button (true/false)
		// var value = Joystick.IsButtonPressed(0, 2); // X Button (true/false)
		// var value = Joystick.IsButtonPressed(0, 3); // Y Button (true/false)
		// var value = Joystick.IsButtonPressed(0, 6); // Back Button (true/false)
		// var value = Joystick.IsButtonPressed(0, 7); // Start Button (true/false)
		// var value = Joystick.IsButtonPressed(0, 4); // Left Bumper (true/false)
		// var value = Joystick.IsButtonPressed(0, 5); // Right Bumper (true/false)

		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.PovX); // Thumbstick (left-right) (-100 or 100)
		// var value = Joystick.GetAxisPosition(0, Joystick.Axis.PovY); // Thumbstick (up-down) (100 or -100)

		foreach (var joy in _gamepadState)
		{
			if (!joy.Value) // Joystick not connected
				continue;

			var value = button switch
			{
				GamepadButton.LeftThumbstickLeft =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.X) * -1f, 0f, 100f) / 100f, 0f, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.LeftThumbstickRight =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.X), 0f, 100f) / 100f, 0f, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.LeftThumbstickUp =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.Y) * -1f, 0f, 100f) / 100f, 0f, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.LeftThumbstickDown =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.Y), 0f, 100f) / 100f, 0f, GetService<EngineSettings>().GamepadDeadzone),

				GamepadButton.RightThumbstickLeft =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.U) * -1f, 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.RightThumbstickRight =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.U), 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.RightThumbstickUp =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.V) * -1f, 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.RightThumbstickDown =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.V), 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),

				GamepadButton.LeftTrigger =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.Z), 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),
				GamepadButton.RightTrigger =>
					Deadzone(Math.Clamp(SFMLGamepad.GetAxisPosition(joy.Key, SFMLGamepad.Axis.Z) * -1f, 0f, 100f) / 100f, 0, GetService<EngineSettings>().GamepadDeadzone),

				_ => 0f
			};

			if (value != 0f)
				return value;
		}

		return 0f;
	}
	#endregion


	#region Private Methods
	private void OnJoystickConnected(uint joystickId)
	{
		SFMLGamepad.Update();
		_gamepadState[joystickId] = SFMLGamepad.IsConnected(joystickId);

		var id = SFML.Window.Joystick.GetIdentification(joystickId);
		if (_controllerMaps.TryGetValue(id.Name, out var map))
			_activeMaps[joystickId] = map;
	}

	private void OnJoystickDisconnected(uint joystickId)
	{
		SFMLGamepad.Update();
		_gamepadState[joystickId] = false;
		_activeMaps.Remove(joystickId);
	}

	private void OnJoystickRelease(ButtonType button)
	{
		switch (button)
		{
			case ButtonType.AButton:
				_gamepadKeys.Remove(GamepadButton.A);
				break;
			case ButtonType.BButton:
				_gamepadKeys.Remove(GamepadButton.B);
				break;
			case ButtonType.XButton:
				_gamepadKeys.Remove(GamepadButton.X);
				break;
			case ButtonType.YButton:
				_gamepadKeys.Remove(GamepadButton.Y);
				break;
			case ButtonType.LeftBumper:
				_gamepadKeys.Remove(GamepadButton.LeftBumper);
				break;
			case ButtonType.RightBumper:
				_gamepadKeys.Remove(GamepadButton.RightBumper);
				break;
			case ButtonType.BackButton:
				_gamepadKeys.Remove(GamepadButton.Back);
				break;
			case ButtonType.StartButton:
				_gamepadKeys.Remove(GamepadButton.Start);
				break;
			case ButtonType.LeftThumbstickButton:
				_gamepadKeys.Remove(GamepadButton.LeftThumbstickButton);
				break;
			case ButtonType.RightThumbstickButton:
				_gamepadKeys.Remove(GamepadButton.RightThumbstickButton);
				break;
		}
	}

	private float Deadzone(float input, float min, float max)
	{
		if (input < min || input > max)
			return input;

		return 0f;
	}

	private List<IInputState> ProcessInputs(Enum[] items)
	{
		List<IInputState> result = new();

		foreach (var item in items)
		{
			if (item is KeyboardButton keyboard)
				result.Add(new KeyboardInputState(keyboard));

			if (item is MouseButton mouse)
				result.Add(new MouseInputState(mouse));

			if (item is GamepadButton gamepad)
				result.Add(new GamepadInputState(gamepad));
		}

		return result;
	}

	private void LoadControllerDbFromResource(string resourceName)
	{
		var bytes = ResourceLoader.GetResourceBytes(resourceName);
		var allText = Encoding.UTF8.GetString(bytes);
		var lines = allText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines)
		{
			if (line.StartsWith("#")) continue;
			var parts = line.Split(',');
			if (parts.Length < 3) continue;

			var deviceName = parts[1].Trim();
			var map = new ControllerMap();

			for (int i = 2; i < parts.Length; i++)
			{
				var kv = parts[i].Split(':');
				if (kv.Length != 2) continue;
				var sdlKey = kv[0].Trim().ToLowerInvariant();
				var binding = kv[1].Trim();

				// buttons
				if (binding.StartsWith("b") && uint.TryParse(binding[1..], out var btn))
					map.ButtonIndex[sdlKey] = btn;

				// axes (+/- suffix on sdlKey)
				else if (binding.StartsWith("a") && uint.TryParse(binding[1..], out var ax)
					  && Enum.TryParse<SFMLAxis>($"Axis{ax}", out var sfAxis))
				{
					float sign = 1;
					if (sdlKey.EndsWith("+")) { sdlKey = sdlKey[..^1]; sign = 1; }
					if (sdlKey.EndsWith("-")) { sdlKey = sdlKey[..^1]; sign = -1; }
					map.AxisIndex[sdlKey] = (sfAxis, sign);
				}
			}

			_controllerMaps[deviceName] = map;
		}
	}
	#endregion
}
