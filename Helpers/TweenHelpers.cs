namespace Box.Helpers;

/// <summary>
/// Provides methods and properties for managing and executing tween animations.
/// </summary>
public static class TweenHelpers
{
	#region Back
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a back-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BackIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a back-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BackIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a back-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BackOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a back-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BackOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a back-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BackOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a back-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BackOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a back-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BackInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a back-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BackInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BackInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Bounce
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a bounce-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-in interpolation between two Vect2 (Vector2) values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 (Vector2) value.</param>
	/// <param name="max">The ending Vect2 (Vector2) value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 (Vector2) value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-out interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-out interpolation between two Vect2 (Vector2) values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 (Vector2) value.</param>
	/// <param name="max">The ending Vect2 (Vector2) value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 (Vector2) value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-out-in interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-out-in interpolation between two Vect2 (Vector2) values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 (Vector2) value.</param>
	/// <param name="max">The ending Vect2 (Vector2) value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 (Vector2) value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-in-out interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a bounce-in-out interpolation between two Vect2 (Vector2) values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 (Vector2) value.</param>
	/// <param name="max">The ending Vect2 (Vector2) value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 (Vector2) value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator BounceInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.BounceInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Circ
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a circular-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CircIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a circular-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CircIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a circular-out interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CircOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a circular-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CircOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a circular-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CircOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a circular-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CircOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a circular-in-out interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CircInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a circular-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CircInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CircInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Cubic
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a cubic-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a cubic-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a cubic-out interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using a cubic-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a cubic-out-in interpolation between two float values over time.
	/// </summary>
	/// <param name="min">The starting float value.</param>
	/// <param name="max">The ending float value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated float value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a cubic-out-in interpolation between two Vect2 values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 value.</param>
	/// <param name="max">The ending Vect2 value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using a cubic-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs a cubic-in-out interpolation between two Vect2 values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 value.</param>
	/// <param name="max">The ending Vect2 value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator CubicInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.CubicInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Elastic
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an elastic-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an elastic-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an elastic-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an elastic-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an elastic-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an elastic-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an elastic-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Performs an elastic-in-out interpolation between two Vect2 values over time.
	/// </summary>
	/// <param name="min">The starting Vect2 value.</param>
	/// <param name="max">The ending Vect2 value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ElasticInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ElasticInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Expo
	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an exponential-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an exponential-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an exponential-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an exponential-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an exponential-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an exponential-out-in easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a float value from min to max over a specified duration using an exponential-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>>
	public static IEnumerator ExpoInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Interpolates a Vect2 value from min to max over a specified duration using an exponential-in-out easing function.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator ExpoInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.ExpoInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Linear
	/// <summary>
	/// Linearly interpolates a float value from min to max over a specified duration.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator Linear(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.Linear, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Linearly interpolates a Vect2 value from min to max over a specified duration.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator Linear(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.Linear, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Quad
	/// <summary>
	/// Quadratic easing function: accelerating from zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: accelerating from zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: accelerating from zero velocity, then decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: accelerating from zero velocity, then decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: decelerating to zero velocity, then accelerating back to starting velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quadratic easing function: decelerating to zero velocity, then accelerating back to starting velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuadOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuadOutIn, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Quart
	/// <summary>
	/// Quartic easing function: accelerating from zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: accelerating from zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: accelerating from zero velocity, then decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: accelerating from zero velocity, then decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: decelerating to zero velocity, then accelerating back to starting velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quartic easing function: decelerating to zero velocity, then accelerating back to starting velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuartInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuartInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Quint
	/// <summary>
	/// Quintic easing function: accelerating from zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: accelerating from zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: accelerating from zero velocity, then decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: accelerating from zero velocity, then decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: accelerating from zero velocity, then decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Quintic easing function: decelerating to zero velocity, then accelerating back to starting velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator QuintInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.QuintInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Sine
	/// <summary>
	/// Sine easing function: accelerating from zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.SineIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: accelerating from zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.SineIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.SineOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.SineOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: accelerating from zero velocity, then decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineOutIn(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.SineOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineOutIn(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.SineOutIn, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: accelerating from zero velocity, then decelerating to zero velocity.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineInOut(float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		yield return Ease(EaseType.SineInOut, min, max, time, result, onStart, onEnd);
	}

	/// <summary>
	/// Sine easing function: accelerating from zero velocity, then decelerating to zero velocity for Vect2.
	/// </summary>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator SineInOut(Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		yield return Ease(EaseType.SineInOut, min, max, time, result, onStart, onEnd);
	}
	#endregion


	#region Ease
	/// <summary>
	/// Performs an easing interpolation based on the specified easing type.
	/// </summary>
	/// <param name="type">The type of easing function to use.</param>
	/// <param name="min">The starting value.</param>
	/// <param name="max">The ending value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator Ease(EaseType type, float min, float max, float time, Action<float> result,
		Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	{
		float cTime = 0f, tTime = 0f;

		onStart?.Invoke(min, max, time);

		while (cTime < time)
		{
			tTime = Math.Clamp(cTime / time, 0f, 1f);
			float value = EasingHelpers.Ease(type, min, max, tTime);
			result?.Invoke(value);

			cTime += Clock.Instance.DeltaTime;
			yield return null;
		}

		result?.Invoke(max);
		onEnd?.Invoke(min, max, time);
	}
	// public static IEnumerator Ease(EaseType type, float min, float max, float time, Action<float> result,
	//     Action<float, float, float> onStart = null, Action<float, float, float> onEnd = null)
	// {
	//     float cTime = 0f, tTime = 0f, value = 0f;

	//     onStart?.Invoke(min, max, time);

	//     while (true)
	//     {
	//         if (value == max && cTime > time)
	//             break;

	//         value = Easing.Ease(type, min, max, tTime);
	//         result?.Invoke(value);

	//         cTime += Clock.Instance.Delta;
	//         tTime += Clock.Instance.Delta * (1f / time);

	//         yield return null;
	//     }

	//     onEnd?.Invoke(min, max, time);
	// }

	/// <summary>
	/// Performs an easing interpolation based on the specified easing type for Vect2 (Vector2).
	/// </summary>
	/// <param name="type">The type of easing function to use.</param>
	/// <param name="min">The starting Vect2 (Vector2) value.</param>
	/// <param name="max">The ending Vect2 (Vector2) value.</param>
	/// <param name="time">The duration of the interpolation.</param>
	/// <param name="result">An action to apply the interpolated Vect2 (Vector2) value.</param>
	/// <param name="onStart">An optional action to call when the interpolation starts.</param>
	/// <param name="onEnd">An optional action to call when the interpolation ends.</param>
	/// <returns>Returns an IEnumerator to control the interpolation.</returns>
	public static IEnumerator Ease(EaseType type, Vect2 min, Vect2 max, float time, Action<Vect2> result,
		Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	{
		float cTime = 0f, tTime = 0f;
		Vect2 value = Vect2.Zero;

		onStart?.Invoke(min, max, time);

		while (cTime < time)
		{
			tTime = Math.Clamp(cTime / time, 0f, 1f);

			value.X = EasingHelpers.Ease(type, min.X, max.X, tTime);
			value.Y = EasingHelpers.Ease(type, min.Y, max.Y, tTime);

			result?.Invoke(value);

			cTime += Clock.Instance.DeltaTime;
			yield return null;
		}

		result?.Invoke(max);
		onEnd?.Invoke(min, max, time);
	}
	// public static IEnumerator Ease(EaseType type, Vect2 min, Vect2 max, float time, Action<Vect2> result,
	// 	Action<Vect2, Vect2, float> onStart = null, Action<Vect2, Vect2, float> onEnd = null)
	// {
	// 	float cTime = 0f, tTime = 0f;
	// 	Vect2 value = Vect2.Zero;

	// 	onStart?.Invoke(min, max, time);

	// 	while (true)
	// 	{
	// 		if (value == max && cTime > time)
	// 			break;

	// 		value.X = Easing.Ease(type, min.X, max.X, tTime);
	// 		value.Y = Easing.Ease(type, min.Y, max.Y, tTime);

	// 		result?.Invoke(value);

	// 		cTime += Clock.Instance.Delta;
	// 		tTime += Clock.Instance.Delta * (1f / time);

	// 		yield return null;
	// 	}

	// 	onEnd?.Invoke(min, max, time);
	// }
	#endregion
}
