using Box;

namespace System;

/// <summary>
/// Contains extension methods for generating random values and making random choices using a specified random generator.
/// </summary>
public static class RandomExtentions
{
	/// <summary>
	/// Returns a randomly chosen element from the provided choices array using the specified random generator.
	/// </summary>
	/// <typeparam name="T">The type of elements in the choices array.</typeparam>
	/// <param name="random">The random generator instance.</param>
	/// <param name="choices">The array of choices.</param>
	/// <returns>The randomly chosen element.</returns>
	public static T Choice<T>(this FastRandom random, params T[] choices)
	{
		if (choices.IsEmpty())
			return default;

		return choices[random.NextInt() % choices.Length];
	}

	/// <summary>
	/// Generates and returns a random color using the specified random generator.
	/// </summary>
	/// <param name="random">The random generator instance.</param>
	/// <returns>A randomly generated color.</returns>
	public static BoxColor RandomColor(this FastRandom random)
		=> new Color(random.Range(0, 255), random.Range(0, 255), random.Range(0, 255));

	/// <summary>
	/// Generates and returns a random 2D vector within the range [-1, 1] for both X and Y coordinates using the specified random generator.
	/// </summary>
	/// <param name="random">The random generator instance.</param>
	/// <returns>A randomly generated 2D vector.</returns>
	public static Vect2 RandomVector(this FastRandom random) => new(random.Range(-1, 1), random.Range(-1, 1));

	/// <summary>
	/// Generates and returns a random direction vector (Up, Right, Down, Left) using the specified random generator.
	/// </summary>
	/// <param name="random">The random generator instance.</param>
	/// <returns>A randomly chosen direction vector.</returns>
	public static Vect2 RandomDirection(this FastRandom random)
		=> random.Choice(Vect2.Up, Vect2.Right, Vect2.Down, Vect2.Left);

	/// <summary>
	/// Generates a random 2D vector with components within the specified minimum and maximum bounds.
	/// </summary>
	/// <param name="random">The Rand instance used to generate the random values.</param>
	/// <param name="minimum">The minimum value for each component of the vector.</param>
	/// <param name="maximum">The maximum value for each component of the vector.</param>
	/// <returns>A randomly generated vector whose components are between <paramref name="minimum"/> and <paramref name="maximum"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="minimum"/> is greater than <paramref name="maximum"/>.</exception>
	public static Vect2 RandomVector(this FastRandom random, float minimum, float maximum)
	{
		if (minimum > maximum)
			throw new ArgumentException("Minimum value must be less than or equal to maximum value.");

		return new(random.Range(minimum, maximum), random.Range(minimum, maximum));
	}
}
