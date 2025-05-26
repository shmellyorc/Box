namespace Box.Coroutines;

/// <summary>
/// Represents a single coroutine instance and its associated delay before execution.
/// </summary>
public struct CoroutineJob
{
	/// <summary>
	/// The coroutine routine to execute (as an IEnumerator).
	/// </summary>
	public IEnumerator Routine { get; set; }

	/// <summary>
	/// The remaining delay in seconds before the coroutine runs or continues.
	/// </summary>
	public float Delay { get; set; }

	public object Owner { get; }

	internal CoroutineJob(object owner, IEnumerator routine, float delay)
	{
		Owner = owner;
		Routine = routine;
		Delay = delay;
	}
}
