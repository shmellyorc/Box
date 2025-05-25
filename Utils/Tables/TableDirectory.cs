namespace Box.Utils.Tables;

/// <summary>
/// A utility class that maps enum keys to snake_case string identifiers.
/// </summary>
public class TableDirectory<TEnum> where TEnum : Enum
{
	private readonly Dictionary<TEnum, string> _data = new();

	/// <summary>
	/// Gets the string identifier associated with the specified enum key.
	/// </summary>
	/// <param name="key">The enum key.</param>
	/// <returns>The snake_case string associated with the key.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the key does not exist in the directory.</exception>
	public string this[TEnum key] => _data[key];

	/// <summary>
	/// Attempts to retrieve the string value associated with the given enum key.
	/// </summary>
	/// <param name="key">The enum key to look up.</param>
	/// <param name="value">When this method returns, contains the value associated with the key, if found; otherwise, null.</param>
	/// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
	public bool TryGetValue(TEnum key, out string value)
		=> _data.TryGetValue(key, out value);

	/// <summary>
	/// Adds a new enum key to the directory, generating and assigning a snake_case identifier.
	/// </summary>
	/// <param name="key">The enum key to add.</param>
	protected void Add(TEnum key)
		=> _data[key] = EnumToSnakeCase(key);

	/// <summary>
	/// Converts an enum value to a snake_case string with double underscores around it.
	/// </summary>
	/// <param name="event">The enum value to convert.</param>
	/// <returns>A snake_case string representation of the enum value, wrapped in double underscores.</returns>
	private string EnumToSnakeCase(TEnum @event)
	{
		var name = @event.ToString();

		var snake = string.Concat(
			name.Select((ch, i) =>
				i > 0 && char.IsUpper(ch)
					? "_" + char.ToLower(ch)
					: char.ToLower(ch).ToString()
			)
		);

		return $"__{snake}__";
	}
}
