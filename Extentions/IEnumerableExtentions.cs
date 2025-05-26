using Box;

namespace System;

/// <summary>
/// Provides extension methods for operations on IEnumerable collections.
/// </summary>
public static class IEnumerableExtentions
{
	/// <summary>
	/// Checks if the IEnumerable collection is empty.
	/// </summary>
	/// <typeparam name="T">Type of elements in the collection.</typeparam>
	/// <param name="source">IEnumerable collection to check.</param>
	/// <returns>True if the collection is empty, false otherwise.</returns>
	public static bool IsEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

	/// <summary>
	/// Searches for the specified value and returns the zero-based index of the first occurrence within the IEnumerable collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="source">The IEnumerable collection to search.</param>
	/// <param name="value">The value to locate in the collection.</param>
	/// <returns>The zero-based index of the first occurrence of value within the entire collection, if found; otherwise, -1.</returns>
	public static int IndexOf<T>(this IEnumerable<T> source, T value) => source.IndexOf(value, null);

	/// <summary>
	/// Finds the index of the first occurrence of a value in the IEnumerable collection using a specified comparer.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="source">The IEnumerable collection to search.</param>
	/// <param name="value">The value to locate in the collection.</param>
	/// <param name="comparer">The equality comparer to use for comparing elements.</param>
	/// <returns>The zero-based index of the first occurrence of the value within the entire collection, if found; otherwise, -1.</returns>
	/// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
	public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
	{
		ArgumentNullException.ThrowIfNull(source);

		int index = 0;

		comparer ??= EqualityComparer<T>.Default; // Use default comparer if none provided

		foreach (var item in source)
		{
			if (comparer.Equals(item, value))
				return index;
			index++;
		}

		return -1;
	}

	/// <summary>
	/// Shuffles the elements in the IEnumerable collection using the Fisher-Yates (Knuth) shuffle algorithm.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="items">The IEnumerable collection to shuffle.</param>
	/// <returns>A shuffled IEnumerable collection of elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when items is null.</exception>
	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
	{
		ArgumentNullException.ThrowIfNull(items);

		return ShuffleIterator(items);

		static IEnumerable<T> ShuffleIterator(IEnumerable<T> source)
		{
			var list = source.ToList();
			var n = list.Count;

			while (n > 1)
			{
				n--;
				int k = Engine.GetService<FastRandom>().NextInt(n + 1);
				T value = list[k];

				list[k] = list[n];
				list[n] = value;
			}

			foreach (T element in list)
				yield return element;
		}
	}

	/// <summary>
	/// Returns the element with the minimum value based on the specified key selector.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by the key selector function.</typeparam>
	/// <param name="source">The sequence to return the minimum element from.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <returns>The element with the minimum value.</returns>
	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		=> source.OrderBy(keySelector).FirstOrDefault();

	/// <summary>
	/// Returns the element with the maximum value based on the specified key selector.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by the key selector function.</typeparam>
	/// <param name="source">The sequence to return the maximum element from.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <returns>The element with the maximum value.</returns>
	public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		=> source.OrderByDescending(keySelector).FirstOrDefault();

	/// <summary>
	/// Performs the specified action on each element of the IEnumerable collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="source">The IEnumerable collection to iterate over.</param>
	/// <param name="action">The action to perform on each element.</param>
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T element in source)
			action(element);
	}

	/// <summary>
	/// Splits the IEnumerable collection into chunks of the specified size.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="source">The IEnumerable collection to split.</param>
	/// <param name="chunkSize">The size of each chunk.</param>
	/// <returns>An IEnumerable of IEnumerable collections representing the chunks.</returns>
	/// <exception cref="ArgumentException">Thrown when chunkSize is less than or equal to zero.</exception>
	public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
	{
		if (chunkSize <= 0)
			throw new ArgumentException("Chunk size must be greater than zero.");

		using var enumerator = source.GetEnumerator();

		while (enumerator.MoveNext())
			yield return YieldChunkElements(enumerator, chunkSize - 1);

		IEnumerable<T> YieldChunkElements(IEnumerator<T> enumerator, int remaining)
		{
			yield return enumerator.Current;

			while (remaining-- > 0 && enumerator.MoveNext())
				yield return enumerator.Current;
		}
	}
}
