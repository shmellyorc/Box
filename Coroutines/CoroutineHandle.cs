namespace Box.Coroutines;

/// <summary>
/// Represents a handle to a running coroutine, allowing control and status queries.
/// </summary>
public struct CoroutineHandle
{
	/// <summary>
	/// Reference to the routine's runner.
	/// </summary>
	public Coroutine Runner;

	/// <summary>
	/// Reference to the routine's enumerator.
	/// </summary>
	public IEnumerator Enumerator;

	/// <summary>
	/// Construct a coroutine. Never call this manually, only use return values from Coroutines.Run().
	/// </summary>
	/// <param name="runner">The routine's runner.</param>
	/// <param name="enumerator">The routine's enumerator.</param>
	public CoroutineHandle(Coroutine runner, IEnumerator enumerator)
	{
		Runner = runner;
		Enumerator = enumerator;
	}

	/// <summary>
	/// Stop this coroutine if it is running.
	/// </summary>
	/// <returns>True if the coroutine was stopped.</returns>
	public bool Stop()
	{
		return IsRunning && Runner.Stop(Enumerator);
	}

	/// <summary>
	/// A routine to wait until this coroutine has finished running.
	/// </summary>
	/// <returns>The wait enumerator.</returns>
	public IEnumerator Wait()
	{
		if (Enumerator != null)
			while (Runner.IsRunning(Enumerator))
				yield return null;
	}

	/// <summary>
	/// True if the enumerator is currently running.
	/// </summary>
	public bool IsRunning
	{
		get { return Enumerator != null && Runner.IsRunning(Enumerator); }
	}
}
