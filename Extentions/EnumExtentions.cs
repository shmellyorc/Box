using Box;

namespace System;

/// <summary>
/// Extension methods for enums related to font measurement operations.
/// </summary>
public static class EnumExtentions
{
	/// <summary>
	/// Checks if a flag enum value contains a specific flag.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <param name="value">The enum value to check.</param>
	/// <param name="flag">The flag to check for.</param>
	/// <returns>True if the enum value contains the flag, false otherwise.</returns>
	public static bool HasFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
	{
		if (!typeof(TEnum).IsEnum)
			throw new ArgumentException("TEnum must be an enumerated type");

		var valueInt = Convert.ToInt32(value);
		var flagInt = Convert.ToInt32(flag);

		return (valueInt & flagInt) == flagInt;
	}

	/// <summary>
	/// Gets the description attribute of an enum value.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <param name="value">The enum value.</param>
	/// <returns>The description attribute value if present, otherwise the enum value name.</returns>
	public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
	{
		if (!typeof(TEnum).IsEnum)
			throw new ArgumentException("TEnum must be an enumerated type");

		string name = Enum.GetName(typeof(TEnum), value);

		if (name == null)
			return string.Empty;

		var fieldInfo = typeof(TEnum).GetField(name);

		if (fieldInfo == null)
			return name;

		var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

		return attribute?.Description ?? name;
	}

	/// <summary>
	/// Adds a flag to the enum value.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <param name="value">The current enum value.</param>
	/// <param name="flag">The flag to add.</param>
	/// <returns>The resulting enum value with the flag added.</returns>
	public static TEnum AddFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
	{
		if (!typeof(TEnum).IsEnum)
			throw new ArgumentException("TEnum must be an enumerated type");

		var valueInt = Convert.ToInt32(value);
		var flagInt = Convert.ToInt32(flag);

		valueInt |= flagInt;

		return (TEnum)Enum.ToObject(typeof(TEnum), valueInt);
	}

	/// <summary>
	/// Removes a flag from the enum value.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <param name="value">The current enum value.</param>
	/// <param name="flag">The flag to remove.</param>
	/// <returns>The resulting enum value with the flag removed.</returns>
	public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
	{
		if (!typeof(TEnum).IsEnum)
			throw new ArgumentException("TEnum must be an enumerated type");

		var valueInt = Convert.ToInt32(value);
		var flagInt = Convert.ToInt32(flag);

		valueInt &= ~flagInt;

		return (TEnum)Enum.ToObject(typeof(TEnum), valueInt);
	}

	/// <summary>
	/// Measures the dimensions (width and height) of a specified text using the font associated with the enum value.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to measure.</param>
	/// <returns>The dimensions (width and height) of the rendered text.</returns>
	public static Vect2 Measure(this Enum value, string text)
		=> Assets.Instance.Get<BoxFont>(value).Measure(text);

	/// <summary>
	/// Measures the width of a specified text using the font associated with the enum value.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to measure.</param>
	/// <returns>The width of the rendered text.</returns>
	public static float MeasureWidth(this Enum value, string text)
		=> Assets.Instance.Get<BoxFont>(value).MeasureWidth(text);

	/// <summary>
	/// Measures the height of a specified text using the font associated with the enum value.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to measure.</param>
	/// <returns>The height of the rendered text.</returns>
	public static float MeasureHeight(this Enum value, string text)
		=> Assets.Instance.Get<BoxFont>(value).MeasureHeight(text);

	/// <summary>
	/// Formats a text string to fit within a specified width using the font associated with the enum value.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to format.</param>
	/// <param name="width">The width constraint.</param>
	/// <returns>The formatted text string.</returns>
	public static string FormatText(this Enum value, string text, int width)
		=> Assets.GetFont(value).FormatText(text, width);

	/// <summary>
	/// Formats a text string to fit within a specified width using the font associated with the enum value
	/// and returns the dimensions (width and height) of the formatted text.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to format.</param>
	/// <param name="width">The width constraint.</param>
	/// <returns>The dimensions (width and height) of the formatted text.</returns>
	public static Vect2 FormatTextAndMeasure(this Enum value, string text, int width)
		=> Assets.GetFont(value).FormatTextAndMeasure(text, width);

	/// <summary>
	/// Formats a text string to fit within a specified width using the font associated with the enum value
	/// and returns the dimensions (width and height) of the formatted text.
	/// </summary>
	/// <param name="value">The enum value representing the font.</param>
	/// <param name="text">The text to format.</param>
	/// <param name="width">The width constraint.</param>
	/// <returns>The dimensions (width and height) of the formatted text.</returns>
	public static Vect2 FormatTextAndMeasure(this Enum value, string text, float width)
		=> Assets.GetFont(value).FormatTextAndMeasure(text, (int)width);
}
