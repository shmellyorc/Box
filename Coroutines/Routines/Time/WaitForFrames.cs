namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Suspends the coroutine for a specified number of frames.
/// </summary>
public sealed class WaitForFrames : IEnumerator
{
	private int _remaining;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitForFrames"/> class.
	/// </summary>
	/// <param name="frameCount">
	/// The number of frames to wait before resuming the coroutine.
	/// </param>
	public WaitForFrames(int frameCount) => _remaining = frameCount;

	/// <summary>
	/// Advances the frame counter by one.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (remaining frames &gt; 0); otherwise <c>false</c> to resume the coroutine.
	/// </returns>
	public bool MoveNext()
	{
		if (_remaining-- > 0) return true;
		return false;
	}

	/// <summary>
	/// Reset is not supported for this instruction and will throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
