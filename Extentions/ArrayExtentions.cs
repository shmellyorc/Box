namespace System;

/// <summary>
/// A static class that contains extension methods for <see cref="Array"/>.
/// These methods provide additional functionality to work with arrays in a more convenient way.
/// </summary>
public static class ArrayExtentions
{
    /// <summary>
    /// Adds multiple elements to the end of the array and returns a new array with the added elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The array to add elements to.</param>
    /// <param name="values">The elements to add to the array.</param>
    /// <returns>A new array containing the elements of the original array followed by the added elements.</returns>
    public static T[] AddRange<T>(this T[] array, params T[] values)
    {
        T[] newArray = new T[array.Length + values.Length];
        array.CopyTo(newArray, 0);
        values.CopyTo(newArray, array.Length);
        return newArray;
    }

    /// <summary>
    /// Removes an element at a specified index in the array and returns a new array without the element.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The array to remove the element from.</param>
    /// <param name="index">The index of the element to remove.</param>
    /// <returns>A new array without the element at the specified index.</returns>
    public static T[] RemoveAt<T>(this T[] array, int index)
    {
        if (index < 0 || index >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        return array.Where((val, idx) => idx != index).ToArray();
    }
}
