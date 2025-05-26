namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Waits until the specified predicate returns true or until a maximum number of frames have elapsed, 
/// whichever comes first.
/// </summary>
public sealed class WaitUntilOrFrames : IEnumerator
{
	private readonly Func<bool> _predicate;
	private int _maxFrames;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitUntilOrFrames"/> class.
	/// </summary>
	/// <param name="pred">The predicate function to evaluate each frame.</param>
	/// <param name="maxFrames">The maximum number of frames to wait.</param>
	public WaitUntilOrFrames(Func<bool> pred, int maxFrames)
	{
		_predicate = pred;
		_maxFrames = maxFrames;
	}

	/// <summary>
	/// Advances the wait by one frame and evaluates the predicate.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (frames remaining > 0 and predicate has not returned <c>true</c>); 
	/// <c>false</c> to resume the coroutine.
	/// </returns>
	public bool MoveNext() => (_maxFrames-- > 0) && !_predicate();

	/// <summary>
	/// Reset is not supported for this instruction and does nothing.
	/// </summary>
	public void Reset() { }
}
