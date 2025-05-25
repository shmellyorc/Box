namespace System;

/// <summary>
/// A static class that contains extension methods for <see cref="List{T}"/>.
/// These methods provide additional functionality to work with lists in a more convenient way.
/// </summary>
public static class ListExtentions
{
    /// <summary>
    /// Removes all null elements from a list.
    /// </summary>
    /// <param name="list">The list to clean.</param>
    /// <returns>The list without null elements.</returns>
    public static List<T> RemoveNulls<T>(this List<T> list) where T : class
    {
        list.RemoveAll(item => item == null);
        return list;
    }

    /// <summary>
    /// Flattens a list of lists into a single list.
    /// </summary>
    /// <param name="list">The list of lists to flatten.</param>
    /// <returns>A single list containing all elements from the inner lists.</returns>
    public static List<T> Flatten<T>(this List<List<T>> list)
    {
        return list.SelectMany(innerList => innerList).ToList();
    }

    /// <summary>
    /// Removes the first element from the list.
    /// </summary>
    /// <param name="list">The list to remove from.</param>
    /// <returns>The removed element, or default value if the list is empty.</returns>
    public static T RemoveFirst<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            T first = list[0];
            list.RemoveAt(0);
            return first;
        }
        return default(T);
    }

    /// <summary>
    /// Removes the last element from the list.
    /// </summary>
    /// <param name="list">The list to remove from.</param>
    /// <returns>The removed element, or default value if the list is empty.</returns>
    public static T RemoveLast<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            T last = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return last;
        }
        return default(T);
    }

    /// <summary>
    /// Counts how many times a specified element occurs in the list.
    /// </summary>
    /// <param name="list">The list to search in.</param>
    /// <param name="element">The element to count.</param>
    /// <returns>The number of occurrences of the element.</returns>
    public static int CountOccurrences<T>(this List<T> list, T element)
    {
        return list.Count(x => EqualityComparer<T>.Default.Equals(x, element));
    }

    /// <summary>
    /// Replaces the first occurrence of a specified element in the list with another element.
    /// </summary>
    /// <param name="list">The list to modify.</param>
    /// <param name="oldElement">The element to replace.</param>
    /// <param name="newElement">The element to replace with.</param>
    /// <returns>True if the element was replaced, otherwise false.</returns>
    public static bool Replace<T>(this List<T> list, T oldElement, T newElement)
    {
        int index = list.IndexOf(oldElement);
        if (index >= 0)
        {
            list[index] = newElement;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds an element to the end of the list (Push operation).
    /// </summary>
    /// <param name="list">The list to which the element will be added.</param>
    /// <param name="item">The item to add to the list.</param>
    public static void Push<T>(this List<T> list, T item)
    {
        list.Add(item);
    }

    /// <summary>
    /// Removes and returns the last element of the list (Pop operation).
    /// </summary>
    /// <param name="list">The list to remove the element from.</param>
    /// <returns>The last element of the list, or default value if the list is empty.</returns>
    public static T Pop<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default;
        else
        {
            T item = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return item;
        }
    }
}
