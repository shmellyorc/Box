namespace Box.Utils;

/// <summary>
/// Provides static methods to perform easing functions for interpolating numerical and Vector2 values.
/// Easing functions modify the rate of change of a value over time, allowing smooth transitions
/// between different states in animations or transitions.
/// </summary>
public static class EasingHelpers
{
	#region Public Interface (Enum)
	/// <summary>
	/// Computes an eased interpolation between two values based on the specified easing type over time.
	/// </summary>
	/// <param name="type">The type of easing function to use.</param>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The total time of the easing process.</param>
	/// <returns>The eased interpolation result between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float Ease(EaseType type, float from, float to, float time)
	{
		return type switch
		{
			EaseType.Linear => Linear(from, to, time),
			EaseType.QuadIn => QuadIn(from, to, time),
			EaseType.QuadOut => QuadOut(from, to, time),
			EaseType.QuadInOut => QuadInOut(from, to, time),
			EaseType.QuadOutIn => QuadOutIn(from, to, time),
			EaseType.CubicIn => CubicIn(from, to, time),
			EaseType.CubicOut => CubicOut(from, to, time),
			EaseType.CubicInOut => CubicInOut(from, to, time),
			EaseType.CubicOutIn => CubicOutIn(from, to, time),
			EaseType.QuartIn => QuartIn(from, to, time),
			EaseType.QuartOut => QuartOut(from, to, time),
			EaseType.QuartInOut => QuartInOut(from, to, time),
			EaseType.QuartOutIn => QuartOutIn(from, to, time),
			EaseType.QuintIn => QuintIn(from, to, time),
			EaseType.QuintOut => QuintOut(from, to, time),
			EaseType.QuintInOut => QuintInOut(from, to, time),
			EaseType.QuintOutIn => QuintOutIn(from, to, time),
			EaseType.SineIn => SineIn(from, to, time),
			EaseType.SineOut => SineOut(from, to, time),
			EaseType.SineInOut => SineInOut(from, to, time),
			EaseType.SineOutIn => SineOutIn(from, to, time),
			EaseType.ExpoIn => ExpoIn(from, to, time),
			EaseType.ExpoOut => ExpoOut(from, to, time),
			EaseType.ExpoInOut => ExpoInOut(from, to, time),
			EaseType.ExpoOutIn => ExpoOutIn(from, to, time),
			EaseType.CircIn => CircIn(from, to, time),
			EaseType.CircOut => CircOut(from, to, time),
			EaseType.CircInOut => CircInOut(from, to, time),
			EaseType.CircOutIn => CircOutIn(from, to, time),
			EaseType.ElasticIn => ElasticIn(from, to, time),
			EaseType.ElasticOut => ElasticOut(from, to, time),
			EaseType.ElasticInOut => ElasticInOut(from, to, time),
			EaseType.ElasticOutIn => ElasticOutIn(from, to, time),
			EaseType.BackIn => BackIn(from, to, time),
			EaseType.BackOut => BackOut(from, to, time),
			EaseType.BackInOut => BackInOut(from, to, time),
			EaseType.BackOutIn => BackOutIn(from, to, time),
			EaseType.BounceIn => BounceIn(from, to, time),
			EaseType.BounceOut => BounceOut(from, to, time),
			EaseType.BounceInOut => BounceInOut(from, to, time),
			EaseType.BounceOutIn => BounceOutIn(from, to, time),
			_ => 0.0f,
		};
	}

	/// <summary>
	/// Computes an eased interpolation between two Vect2 values based on the specified easing type over time.
	/// </summary>
	/// <param name="type">The type of easing function to use.</param>
	/// <param name="from">The starting Vect2 value.</param>
	/// <param name="to">The target Vect2 value.</param>
	/// <param name="time">The total time of the easing process.</param>
	/// <returns>The eased interpolation result between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 Ease(EaseType type, Vect2 from, Vect2 to, float time)
	{
		return type switch
		{
			EaseType.Linear => Linear(from, to, time),
			EaseType.QuadIn => QuadIn(from, to, time),
			EaseType.QuadOut => QuadOut(from, to, time),
			EaseType.QuadInOut => QuadInOut(from, to, time),
			EaseType.QuadOutIn => QuadOutIn(from, to, time),
			EaseType.CubicIn => CubicIn(from, to, time),
			EaseType.CubicOut => CubicOut(from, to, time),
			EaseType.CubicInOut => CubicInOut(from, to, time),
			EaseType.CubicOutIn => CubicOutIn(from, to, time),
			EaseType.QuartIn => QuartIn(from, to, time),
			EaseType.QuartOut => QuartOut(from, to, time),
			EaseType.QuartInOut => QuartInOut(from, to, time),
			EaseType.QuartOutIn => QuartOutIn(from, to, time),
			EaseType.QuintIn => QuintIn(from, to, time),
			EaseType.QuintOut => QuintOut(from, to, time),
			EaseType.QuintInOut => QuintInOut(from, to, time),
			EaseType.QuintOutIn => QuintOutIn(from, to, time),
			EaseType.SineIn => SineIn(from, to, time),
			EaseType.SineOut => SineOut(from, to, time),
			EaseType.SineInOut => SineInOut(from, to, time),
			EaseType.SineOutIn => SineOutIn(from, to, time),
			EaseType.ExpoIn => ExpoIn(from, to, time),
			EaseType.ExpoOut => ExpoOut(from, to, time),
			EaseType.ExpoInOut => ExpoInOut(from, to, time),
			EaseType.ExpoOutIn => ExpoOutIn(from, to, time),
			EaseType.CircIn => CircIn(from, to, time),
			EaseType.CircOut => CircOut(from, to, time),
			EaseType.CircInOut => CircInOut(from, to, time),
			EaseType.CircOutIn => CircOutIn(from, to, time),
			EaseType.ElasticIn => ElasticIn(from, to, time),
			EaseType.ElasticOut => ElasticOut(from, to, time),
			EaseType.ElasticInOut => ElasticInOut(from, to, time),
			EaseType.ElasticOutIn => ElasticOutIn(from, to, time),
			EaseType.BackIn => BackIn(from, to, time),
			EaseType.BackOut => BackOut(from, to, time),
			EaseType.BackInOut => BackInOut(from, to, time),
			EaseType.BackOutIn => BackOutIn(from, to, time),
			EaseType.BounceIn => BounceIn(from, to, time),
			EaseType.BounceOut => BounceOut(from, to, time),
			EaseType.BounceInOut => BounceInOut(from, to, time),
			EaseType.BounceOutIn => BounceOutIn(from, to, time),
			_ => Vect2.Zero,
		};
	}

	#endregion

	#region Public Interface (Functions)
	/// <summary>
	/// Linearly interpolates between two float values.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The interpolated value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float Linear(float from, float to, float time)
	{
		return In(Linear, time, from, to - from);
	}

	/// <summary>
	/// Linearly interpolates between two float values.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The interpolated value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 Linear(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			Linear(from.X, to.X, time),
			Linear(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases in using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased-in value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float QuadIn(float from, float to, float time)
	{
		return In(Quad, time, from, to - from);
	}

	/// <summary>
	/// Eases in using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased-in value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 QuadIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuadIn(from.X, to.X, time),
			QuadIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases out using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased-out value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float QuadOut(float from, float to, float time)
	{
		return Out(Quad, time, from, to - from);
	}

	/// <summary>
	/// Eases out using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased-out value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 QuadOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuadOut(from.X, to.X, time),
			QuadOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases in and out using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float QuadInOut(float from, float to, float time)
	{
		return InOut(Quad, time, from, to - from);
	}

	/// <summary>
	/// Eases in and out using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 QuadInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuadInOut(from.X, to.X, time),
			QuadInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases out and in using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float QuadOutIn(float from, float to, float time)
	{
		return OutIn(Quad, time, from, to - from);
	}

	/// <summary>
	/// Eases out and in using a quadratic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 QuadOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuadOutIn(from.X, to.X, time),
			QuadOutIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases in using a cubic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static float CubicIn(float from, float to, float time)
	{
		return In(Cubic, time, from, to - from);
	}

	/// <summary>
	/// Eases in using a cubic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 CubicIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CubicIn(from.X, to.X, time),
			CubicIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Eases out using a cubic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>

	public static float CubicOut(float from, float to, float time)
	{
		return Out(Cubic, time, from, to - from);
	}

	/// <summary>
	/// Eases out using a cubic function.
	/// </summary>
	/// <param name="from">The starting value.</param>
	/// <param name="to">The target value.</param>
	/// <param name="time">The interpolation parameter. Should be between 0 and 1.</param>
	/// <returns>The eased value between <paramref name="from"/> and <paramref name="to"/> at the given <paramref name="time"/>.</returns>
	public static Vect2 CubicOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CubicOut(from.X, to.X, time),
			CubicOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs cubic easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CubicInOut(float from, float to, float time)
	{
		return InOut(Cubic, time, from, to - from);
	}

	/// <summary>
	/// Performs cubic easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CubicInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CubicInOut(from.X, to.X, time),
			CubicInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs cubic easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CubicOutIn(float from, float to, float time)
	{
		return OutIn(Cubic, time, from, to - from);
	}

	/// <summary>
	/// Performs cubic easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CubicOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CubicOutIn(from.X, to.X, time),
			CubicOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs quartic easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuartIn(float from, float to, float time)
	{
		return In(Quart, time, from, to - from);
	}

	/// <summary>
	/// Performs quartic easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuartIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuartIn(from.X, to.X, time),
			QuartIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs quartic easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuartOut(float from, float to, float time)
	{
		return Out(Quart, time, from, to - from);
	}

	/// <summary>
	/// Performs quartic easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuartOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuartOut(from.X, to.X, time),
			QuartOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs quartic easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuartInOut(float from, float to, float time)
	{
		return InOut(Quart, time, from, to - from);
	}

	/// <summary>
	/// Performs quartic easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuartInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuartInOut(from.X, to.X, time),
			QuartInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs quartic easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuartOutIn(float from, float to, float time)
	{
		return OutIn(Quart, time, from, to - from);
	}

	/// <summary>
	/// Performs quartic easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuartOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuartOutIn(from.X, to.X, time),
			QuartOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs quintic easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuintIn(float from, float to, float time)
	{
		return In(Quint, time, from, to - from);
	}

	/// <summary>
	/// Performs quintic easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuintIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuintIn(from.X, to.X, time),
			QuintIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs quintic easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuintOut(float from, float to, float time)
	{
		return Out(Quint, time, from, to - from);
	}

	/// <summary>
	/// Performs quintic easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuintOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuintOut(from.X, to.X, time),
			QuintOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs quintic easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuintInOut(float from, float to, float time)
	{
		return InOut(Quint, time, from, to - from);
	}

	/// <summary>
	/// Performs quintic easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuintInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuintInOut(from.X, to.X, time),
			QuintInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs quintic easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float QuintOutIn(float from, float to, float time)
	{
		return OutIn(Quint, time, from, to - from);
	}

	/// <summary>
	/// Performs quintic easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 QuintOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			QuintOutIn(from.X, to.X, time),
			QuintOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs sine easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float SineIn(float from, float to, float time)
	{
		return In(Sine, time, from, to - from);
	}

	/// <summary>
	/// Performs sine easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 SineIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			SineIn(from.X, to.X, time),
			SineIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs sine easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float SineOut(float from, float to, float time)
	{
		return Out(Sine, time, from, to - from);
	}

	/// <summary>
	/// Performs sine easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 SineOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			SineOut(from.X, to.X, time),
			SineOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs sine easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float SineInOut(float from, float to, float time)
	{
		return InOut(Sine, time, from, to - from);
	}

	/// <summary>
	/// Performs sine easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 SineInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			SineInOut(from.X, to.X, time),
			SineInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs sine easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float SineOutIn(float from, float to, float time)
	{
		return OutIn(Sine, time, from, to - from);
	}

	/// <summary>
	/// Performs sine easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 SineOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			SineOutIn(from.X, to.X, time),
			SineOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs exponential easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ExpoIn(float from, float to, float time)
	{
		return In(Expo, time, from, to - from);
	}

	/// <summary>
	/// Performs exponential easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ExpoIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ExpoIn(from.X, to.X, time),
			ExpoIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs exponential easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ExpoOut(float from, float to, float time)
	{
		return Out(Expo, time, from, to - from);
	}

	/// <summary>
	/// Performs exponential easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ExpoOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ExpoOut(from.X, to.X, time),
			ExpoOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs exponential easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ExpoInOut(float from, float to, float time)
	{
		return InOut(Expo, time, from, to - from);
	}

	/// <summary>
	/// Performs exponential easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ExpoInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ExpoInOut(from.X, to.X, time),
			ExpoInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs exponential easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ExpoOutIn(float from, float to, float time)
	{
		return OutIn(Expo, time, from, to - from);
	}

	/// <summary>
	/// Performs exponential easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ExpoOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ExpoOutIn(from.X, to.X, time),
			ExpoOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs circular easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CircIn(float from, float to, float time)
	{
		return In(Circ, time, from, to - from);
	}

	/// <summary>
	/// Performs circular easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CircIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CircIn(from.X, to.X, time),
			CircIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs circular easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CircOut(float from, float to, float time)
	{
		return Out(Circ, time, from, to - from);
	}

	/// <summary>
	/// Performs circular easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CircOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CircOut(from.X, to.X, time),
			CircOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs circular easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CircInOut(float from, float to, float time)
	{
		return InOut(Circ, time, from, to - from);
	}

	/// <summary>
	/// Performs circular easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CircInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CircInOut(from.X, to.X, time),
			CircInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs circular easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float CircOutIn(float from, float to, float time)
	{
		return OutIn(Circ, time, from, to - from);
	}

	/// <summary>
	/// Performs circular easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 CircOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			CircOutIn(from.X, to.X, time),
			CircOutIn(from.Y, to.Y, time)
		);
	}




	/// <summary>
	/// Performs elastic easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ElasticIn(float from, float to, float time)
	{
		return In(Elastic, time, from, to - from);
	}

	/// <summary>
	/// Performs elastic easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ElasticIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ElasticIn(from.X, to.X, time),
			ElasticIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs elastic easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ElasticOut(float from, float to, float time)
	{
		return Out(Elastic, time, from, to - from);
	}

	/// <summary>
	/// Performs elastic easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ElasticOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ElasticOut(from.X, to.X, time),
			ElasticOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs elastic easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ElasticInOut(float from, float to, float time)
	{
		return InOut(Elastic, time, from, to - from);
	}

	/// <summary>
	/// Performs elastic easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ElasticInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ElasticInOut(from.X, to.X, time),
			ElasticInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs elastic easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float ElasticOutIn(float from, float to, float time)
	{
		return OutIn(Elastic, time, from, to - from);
	}

	/// <summary>
	/// Performs elastic easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 ElasticOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			ElasticOutIn(from.X, to.X, time),
			ElasticOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs back easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BackIn(float from, float to, float time)
	{
		return In(Back, time, from, to - from);
	}

	/// <summary>
	/// Performs back easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BackIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BackIn(from.X, to.X, time),
			BackIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs back easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BackOut(float from, float to, float time)
	{
		return Out(Back, time, from, to - from);
	}

	/// <summary>
	/// Performs back easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BackOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BackOut(from.X, to.X, time),
			BackOut(from.Y, to.Y, time)
		);
	}




	/// <summary>
	/// Performs back easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BackInOut(float from, float to, float time)
	{
		return InOut(Back, time, from, to - from);
	}

	/// <summary>
	/// Performs back easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BackInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BackInOut(from.X, to.X, time),
			BackInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs back easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BackOutIn(float from, float to, float time)
	{
		return OutIn(Back, time, from, to - from);
	}

	/// <summary>
	/// Performs back easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BackOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BackOutIn(from.X, to.X, time),
			BackOutIn(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs bounce easing in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BounceIn(float from, float to, float time)
	{
		return In(Bounce, time, from, to - from);
	}

	/// <summary>
	/// Performs bounce easing in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BounceIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BounceIn(from.X, to.X, time),
			BounceIn(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs bounce easing out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BounceOut(float from, float to, float time)
	{
		return Out(Bounce, time, from, to - from);
	}

	/// <summary>
	/// Performs bounce easing out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BounceOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BounceOut(from.X, to.X, time),
			BounceOut(from.Y, to.Y, time)
		);
	}



	/// <summary>
	/// Performs bounce easing in-out interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BounceInOut(float from, float to, float time)
	{
		return InOut(Bounce, time, from, to - from);
	}

	/// <summary>
	/// Performs bounce easing in-out interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BounceInOut(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BounceInOut(from.X, to.X, time),
			BounceInOut(from.Y, to.Y, time)
		);
	}

	/// <summary>
	/// Performs bounce easing out-in interpolation between two float values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated float value.</returns>
	public static float BounceOutIn(float from, float to, float time)
	{
		return OutIn(Bounce, time, from, to - from);
	}

	/// <summary>
	/// Performs bounce easing out-in interpolation between two Vect2 values.
	/// </summary>
	/// <param name="from">Starting value.</param>
	/// <param name="to">Target value.</param>
	/// <param name="time">Interpolation time (normalized, typically 0 to 1).</param>
	/// <returns>Interpolated Vect2 value.</returns>
	public static Vect2 BounceOutIn(Vect2 from, Vect2 to, float time)
	{
		return new Vect2(
			BounceOutIn(from.X, to.X, time),
			BounceOutIn(from.Y, to.Y, time)
		);
	}
	#endregion

	#region Ease Types
	private static float In(Func<float, float, float> ease_f, float time, float b, float c, float d = 1)
	{
		if (time >= d)
			return b + c;
		if (time <= 0)
			return b;

		return c * ease_f(time, d) + b;
	}

	private static float Out(Func<float, float, float> ease_f, float time, float b, float c, float d = 1)
	{
		if (time >= d)
			return b + c;
		if (time <= 0)
			return b;

		return b + c - c * ease_f(d - time, d);
	}

	private static float InOut(Func<float, float, float> ease_f, float time, float b, float c, float d = 1)
	{
		if (time >= d)
			return b + c;
		if (time <= 0)
			return b;

		if (time < d / 2)
			return In(ease_f, time * 2, b, c / 2, d);

		return Out(ease_f, time * 2 - d, b + c / 2, c / 2, d);
	}

	private static float OutIn(Func<float, float, float> ease_f, float time, float b, float c, float d = 1)
	{
		if (time >= d)
			return b + c;
		if (time <= 0)
			return b;

		if (time < d / 2)
			return Out(ease_f, time * 2, b, c / 2, d);

		return In(ease_f, time * 2 - d, b + c / 2, c / 2, d);
	}

	#endregion

	#region Equations

	private static float Linear(float time, float d = 1)
	{
		return time / d;
	}

	private static float Quad(float time, float d = 1)
	{
		return (time /= d) * time;
	}

	private static float Cubic(float time, float d = 1)
	{
		return (time /= d) * time * time;
	}

	private static float Quart(float time, float d = 1)
	{
		return (time /= d) * time * time * time;
	}

	private static float Quint(float time, float d = 1)
	{
		return (time /= d) * time * time * time * time;
	}

	private static float Sine(float time, float d = 1)
	{
		return 1 - (float)Math.Cos(time / d * (Math.PI / 2));
	}

	private static float Expo(float time, float d = 1)
	{
		return (float)Math.Pow(2, 10 * (time / d - 1));
	}

	private static float Circ(float time, float d = 1)
	{
		return -((float)Math.Sqrt(1 - (time /= d) * time) - 1);
	}

	private static float Elastic(float time, float d = 1)
	{
		time /= d;
		float p = d * .3f;
		float s = p / 4;
		return -((float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * d - s) * (2 * (float)Math.PI) / p));
	}

	private static float Back(float time, float d = 1)
	{
		return (time /= d) * time * ((1.70158f + 1) * time - 1.70158f);
	}

	private static float Bounce(float time, float d = 1)
	{
		time = d - time;
		if ((time /= d) < 1 / 2.75f)
			return 1 - 7.5625f * time * time;
		else if (time < 2 / 2.75f)
			return 1 - (7.5625f * (time -= 1.5f / 2.75f) * time + .75f);
		else if (time < 2.5f / 2.75f)
			return 1 - (7.5625f * (time -= 2.25f / 2.75f) * time + .9375f);
		else
			return 1 - (7.5625f * (time -= 2.625f / 2.75f) * time + .984375f);
	}

	#endregion
}
