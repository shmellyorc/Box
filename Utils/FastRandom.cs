using System.Runtime.CompilerServices;

using Box.Services.Types;

namespace Box.Utils;

/// <summary>
/// A very fast, non-cryptographic pseudo-random number generator based on xoroshiro128+.
/// </summary>
public sealed class FastRandom : GameService
{
	private ulong _s0, _s1;

	/// <summary>
	/// Initializes a new instance of <see cref="FastRandom"/> with a seed drawn from
	/// the system cryptographic RNG.
	/// </summary>
	public FastRandom()
	{
		var seedBytes = new byte[16];
		System.Security.Cryptography.RandomNumberGenerator.Fill(seedBytes);
		_s0 = BitConverter.ToUInt64(seedBytes, 0);
		_s1 = BitConverter.ToUInt64(seedBytes, 8);
	}

	/// <summary>
	/// Initializes a new instance of <see cref="FastRandom"/> using the specified seed.
	/// </summary>
	/// <param name="seed">An arbitrary 64-bit value to seed the generator.</param>
	public FastRandom(ulong seed)
	{
		_s0 = SplitMix64(ref seed);
		_s1 = SplitMix64(ref seed);
	}

	/// <summary>
	/// Reseeds this instance of <see cref="FastRandom"/> with the given value.
	/// </summary>
	/// <param name="seed">An arbitrary 64-bit value to reseed the generator.</param>
	public void SetSeed(ulong seed)
	{
		_s0 = SplitMix64(ref seed);
		_s1 = SplitMix64(ref seed);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong SplitMix64(ref ulong x)
	{
		x += 0x9E3779B97F4A7C15UL;
		ulong z = x;
		z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
		z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
		return z ^ (z >> 31);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ulong NextRaw()
	{
		ulong s0 = _s0;
		ulong s1 = _s1;
		ulong result = s0 + s1;

		s1 ^= s0;
		_s0 = RotateLeft(s0, 55) ^ s1 ^ (s1 << 14); // a, b
		_s1 = RotateLeft(s1, 36);                     // c

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));

	/// <summary>
	/// Returns a non-negative random integer.
	/// </summary>
	/// <returns>An <see cref="int"/> between 0 (inclusive) and <see cref="int.MaxValue"/> (inclusive).</returns>
	public int NextInt() => (int)(NextRaw() >> 33);

	/// <summary>
	/// Returns a non-negative random integer less than <paramref name="max"/>.
	/// </summary>
	/// <param name="max">The exclusive upper bound. Must be &gt; 0.</param>
	/// <returns>An <see cref="int"/> in the range [0, <paramref name="max"/>).</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="max"/> is not greater than zero.
	/// </exception>
	public int NextInt(int max)
	{
		if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), "max must be greater than zero.");
		return (int)((NextRaw() >> 33) % (uint)max);
	}

	/// <summary>
	/// Returns a random integer in the range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>An <see cref="int"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public int NextInt(int min, int max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return min + NextInt(max - min);
	}

	/// <summary>
	/// Returns a non-negative random 64-bit signed integer.
	/// </summary>
	/// <returns>A <see cref="long"/> between 0 (inclusive) and <see cref="long.MaxValue"/> (inclusive).</returns>
	public long NextLong() => (long)(NextRaw() >> 1);

	/// <summary>
	/// Returns a non-negative random 64-bit signed integer less than <paramref name="max"/>.
	/// </summary>
	/// <param name="max">The exclusive upper bound. Must be &gt; 0.</param>
	/// <returns>A <see cref="long"/> in the range [0, <paramref name="max"/>).</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="max"/> is not greater than zero.
	/// </exception>
	public long NextLong(long max)
	{
		if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), "max must be greater than zero.");
		return (long)(NextRaw() % (ulong)max);
	}

	/// <summary>
	/// Returns a random 64-bit signed integer in the range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>A <see cref="long"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public long NextLong(long min, long max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return min + NextLong(max - min);
	}

	/// <summary>
	/// Returns a random double-precision floating-point number in [0.0, 1.0).
	/// </summary>
	/// <returns>A <see cref="double"/> in the range [0.0, 1.0).</returns>
	public double NextDouble() => (NextRaw() >> 11) * (1.0 / (1UL << 53));

	/// <summary>
	/// Returns a random double-precision floating-point number in [0.0, <paramref name="max"/>).
	/// </summary>
	/// <param name="max">The exclusive upper bound. Must be &gt; 0.</param>
	/// <returns>A <see cref="double"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="max"/> is not greater than zero.
	/// </exception>
	public double NextDouble(double max)
	{
		if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), "max must be greater than zero.");
		return NextDouble() * max;
	}

	/// <summary>
	/// Returns a random double-precision floating-point number in the range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>A <see cref="double"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public double NextDouble(double min, double max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return min + NextDouble() * (max - min);
	}

	/// <summary>
	/// Returns a random single-precision floating-point number in [0.0f, 1.0f).
	/// </summary>
	/// <returns>A <see cref="float"/> in the range [0.0f, 1.0f).</returns>
	public float NextFloat() => (float)((NextRaw() >> 40) * (1.0 / (1UL << 24)));

	/// <summary>
	/// Returns a random single-precision floating-point number in [0.0f, <paramref name="max"/>).
	/// </summary>
	/// <param name="max">The exclusive upper bound. Must be &gt; 0.</param>
	/// <returns>A <see cref="float"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="max"/> is not greater than zero.
	/// </exception>
	public float NextFloat(float max)
	{
		if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), "max must be greater than zero.");
		return NextFloat() * max;
	}

	/// <summary>
	/// Returns a random single-precision floating-point number in the range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>A <see cref="float"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public float NextFloat(float min, float max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return min + NextFloat() * (max - min);
	}

	/// <summary>
	/// Returns a random boolean value.
	/// </summary>
	/// <returns><c>true</c> or <c>false</c>, each with approximately 50% probability.</returns>
	public bool NextBool() => (NextRaw() & 1) == 1;

	/// <summary>
	/// Returns a random integer in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The inclusive upper bound. Must be greater than or equal to <paramref name="min"/>.</param>
	/// <returns>An <see cref="int"/> in the specified inclusive range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is greater than <paramref name="max"/>.
	/// </exception>
	public int Range(int min, int max)
	{
		if (min > max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than or equal to max.");
		return NextInt(min, max + 1);
	}

	/// <summary>
	/// Returns a random long in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The inclusive upper bound. Must be greater than or equal to <paramref name="min"/>.</param>
	/// <returns>A <see cref="long"/> in the specified inclusive range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is greater than <paramref name="max"/>.
	/// </exception>
	public long Range(long min, long max)
	{
		if (min > max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than or equal to max.");
		return NextLong(min, max + 1);
	}

	/// <summary>
	/// Returns a random double in the half-open range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>A <see cref="double"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public double Range(double min, double max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return NextDouble() * (max - min) + min;
	}

	/// <summary>
	/// Returns a random float in the half-open range [<paramref name="min"/>, <paramref name="max"/>).
	/// </summary>
	/// <param name="min">The inclusive lower bound.</param>
	/// <param name="max">The exclusive upper bound. Must be greater than <paramref name="min"/>.</param>
	/// <returns>A <see cref="float"/> in the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="min"/> is not less than <paramref name="max"/>.
	/// </exception>
	public float Range(float min, float max)
	{
		if (min >= max) throw new ArgumentOutOfRangeException(nameof(min), "min must be less than max.");
		return NextFloat() * (max - min) + min;
	}
}
