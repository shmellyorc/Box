using System.Text.RegularExpressions;

namespace System;

/// <summary>
/// Extensions for string manipulation and validation.
/// </summary>
public static partial class StringExtentions
{
    /// <summary>
    /// Converts the enum value to a string representation in the format "EnumTypeName.EnumValue".
    /// </summary>
    /// <param name="text">The enum value to convert.</param>
    /// <returns>A string representation of the enum value.</returns>
    public static string ToEnumString(this Enum text) => $"{text.GetType().Name}.{text}";

    /// <summary>
    /// Checks whether the string is null or empty.
    /// </summary>
    /// <param name="text">The string to check.</param>
    /// <returns>True if the string is null or empty; otherwise, false.</returns>
    public static bool IsEmpty(this string text) => string.IsNullOrWhiteSpace(text);

    /// <summary>
    /// Converts the string to title case using the current culture.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>The string converted to title case.</returns>
    public static string ToTitleCase(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null or empty.");

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>
    /// Truncates the string to the specified maximum length.
    /// </summary>
    /// <param name="text">The string to truncate.</param>
    /// <param name="length">The maximum length of the truncated string.</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(this string text, int length)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null or empty.");

        if (string.IsNullOrEmpty(text) || text.Length <= length)
            return text;

        return string.Concat(text.AsSpan(0, length), "...");
    }

    /// <summary>
    /// Counts the number of times a specific character appears in the given string.
    /// </summary>
    /// <param name="text">The input string to search.</param>
    /// <param name="character">The character to count occurrences of.</param>
    /// <returns>The number of times the specified character appears in the string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
    public static int CountOccurrences(this string text, char character)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null or empty.");

        return text.Count(c => c == character);
    }

    /// <summary>
    /// Encodes the input string to Base64.
    /// </summary>
    /// <param name="text">The input string to encode.</param>
    /// <returns>The Base64 encoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null or empty.</exception>
    public static string Encode(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null or empty.");

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }

    /// <summary>
    /// Decodes a Base64 encoded string to its original form.
    /// </summary>
    /// <param name="text">The Base64 encoded string to decode.</param>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null or empty.</exception>
    public static string Decode(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Encoded string cannot be null or empty.");

        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }

    /// <summary>
    /// Compares two strings for equality ignoring case.
    /// </summary>
    /// <param name="a">The first string to compare.</param>
    /// <param name="b">The second string to compare.</param>
    /// <returns>True if the strings are equal ignoring case; otherwise, false.</returns>
    public static bool EqualsIgnoreCase(this string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Converts the first character of the string to uppercase.
    /// </summary>
    /// <param name="text">The string to modify.</param>
    /// <returns>The string with the first character in uppercase.</returns>
    public static string Firstcase(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return $"{text[0].ToString().ToUpper()}{text[1..]}";
    }

    /// <summary>
    /// Removes numeric digits from the string.
    /// </summary>
    /// <param name="text">The input string.</param>
    /// <returns>The string with numeric digits removed.</returns>
    public static string RemoveNumbers(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        var sb = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            if (char.IsDigit(c))
                continue;

            sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the string to uppercase.
    /// </summary>
    /// <param name="text">The input string.</param>
    /// <returns>The string converted to uppercase.</returns>
    public static string Uppercase(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return text.ToUpper();
    }

    /// <summary>
    /// Converts the string to lowercase.
    /// </summary>
    /// <param name="text">The input string.</param>
    /// <returns>The string converted to lowercase.</returns>
    public static string Lowercase(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return text.ToLower();
    }

    /// <summary>
    /// Count the occurrences of a substring within a string.
    /// </summary>
    /// <param name="text">the string to input</param>
    /// <param name="substring">the word you are trying to search.</param>
    /// <returns>Returns zero if couldnt be found or returns the amount of words based on keyword</returns>
    public static int CountOccurrences(this string text, string substring)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        int count = 0, index = 0;

        while ((index = text.IndexOf(substring, index)) != -1)
        {
            index += substring.Length;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Checks if the string represents a numeric value.
    /// </summary>
    /// <param name="text">The string to check.</param>
    /// <returns>True if the string is numeric; otherwise, false.</returns>
    public static bool IsNumeric(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return long.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _);
    }

    /// <summary>
    /// Converts the given string to snake_case formatting.
    /// </summary>
    /// <param name="text">The input string to convert.</param>
    /// <returns>A new string in snake_case format.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
    public static string ToSnakeCase(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return Regex.Replace(text, @"([a-z])([A-Z])", "$1_$2").ToLower();
    }

    /// <summary>
    /// Removes all whitespace characters from the given string.
    /// </summary>
    /// <param name="text">The input string from which whitespace will be removed.</param>
    /// <returns>A new string with all whitespace characters removed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
    public static string RemoveWhitespace(this string text)
    {
        if (text.IsEmpty())
            throw new ArgumentNullException(nameof(text), "Input string cannot be null.");

        return string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
    }

    /// <summary>
    /// Computes the hash value of the string using a hashing algorithm.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The hash value of the string.</returns>
    public static string ToHash(this string value)
    {
        if (value.IsEmpty())
            return value;

        try
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = MD5.HashData(bytes);

            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);

        }
        catch (EncoderFallbackException ex)
        {
            // Handle encoding issues
            throw new ArgumentException("Error encoding string for hashing.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error computing MD5 hash.", ex);
        }
    }
}
