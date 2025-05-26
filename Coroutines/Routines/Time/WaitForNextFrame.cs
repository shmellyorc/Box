namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Suspends the coroutine for exactly one Update() frame.
/// </summary>
public class WaitForNextFrame : IEnumerator
{
	private bool _hasWaited = false;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Advances the wait by one frame.
	/// </summary>
	/// <returns>
	/// <c>true</c> on the first call (skips one frame), or <c>false</c>  
	/// on subsequent calls to resume the coroutine.
	/// </returns>
	public bool MoveNext()
	{
		if (!_hasWaited)
		{
			_hasWaited = true;
			return true;    // skip this frame
		}
		return false;       // then done
	}

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
