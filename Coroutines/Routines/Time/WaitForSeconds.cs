namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Suspends the coroutine for the specified duration (in seconds).
/// </summary>
public class WaitForSeconds : IEnumerator
{
	private float _remaining;
	private Clock _clock;

	/// <summary>
	/// Creates a new <see cref="WaitForSeconds"/> that will wait for the given time.
	/// </summary>
	/// <param name="seconds">How many seconds to pause the coroutine.</param>
	public WaitForSeconds(float seconds) => _remaining = seconds;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Advances the timer by subtracting <see cref="Clock.DeltaTime"/> 
	/// from the remaining time.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (remaining &gt; 0); otherwise <c>false</c> 
	/// to indicate the wait is over.
	/// </returns>
	public bool MoveNext()
	{
		_remaining -= Clock.Instance.DeltaTime;

		return _remaining > 0f;
	}

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
