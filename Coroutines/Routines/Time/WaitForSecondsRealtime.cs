namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Waits for the specified number of real-time seconds, unaffected by frame-rate or time-scaling.
/// </summary>
public sealed class WaitForSecondsRealtime : IEnumerator
{
	private double _targetTime;
	private Clock _svc;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitForSecondsRealtime"/> class.
	/// </summary>
	/// <param name="seconds">Number of real-time seconds to wait before resuming the coroutine.</param>
	public WaitForSecondsRealtime(float seconds)
	{
		_svc = Engine.GetService<Clock>();
		_targetTime = _svc.RealTime + (double)seconds;
	}

	/// <summary>
	/// Advances the wait; continues waiting while the real-time has not yet reached the target.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (current real time &lt; target time); otherwise <c>false</c> to resume the 
	/// coroutine.
	/// </returns>
	public bool MoveNext() => _svc.RealTime < _targetTime;

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
