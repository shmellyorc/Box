namespace Box.Coroutines;

/// <summary>
/// A container for running multiple coroutines in parallel.
/// Provides methods to start, stop, and query coroutines.
/// </summary>
public sealed class Coroutine
{
	/// <summary>
	/// A container for running multiple routines in parallel. Coroutines can be nested.
	/// </summary>
	private List<IEnumerator> running = new List<IEnumerator>();
	private List<float> delays = new List<float>();

	public static Coroutine Instance { get; private set; }

	internal Coroutine()
	{
		Instance ??= this;
	}

	/// <summary>
	/// Run a coroutine.
	/// </summary>
	/// <returns>A handle to the new coroutine.</returns>
	/// <param name="delay">How many seconds to delay before starting.</param>
	/// <param name="routine">The routine to run.</param>
	public CoroutineHandle RunDelayed(float delay, IEnumerator routine)
	{
		running.Add(routine);
		delays.Add(delay);
		return new CoroutineHandle(this, routine);
	}

	/// <summary>
	/// Run a coroutine.
	/// </summary>
	/// <returns>A handle to the new coroutine.</returns>
	/// <param name="routine">The routine to run.</param>
	public CoroutineHandle Run(IEnumerator routine)
	{
		return RunDelayed(0f, routine);
	}

	/// <summary>
	/// Stop the specified routine.
	/// </summary>
	/// <returns>True if the routine was actually stopped.</returns>
	/// <param name="routine">The routine to stop.</param>
	public bool Stop(IEnumerator routine)
	{
		int i = running.IndexOf(routine);
		if (i < 0)
			return false;
		running[i] = null;
		delays[i] = 0f;
		return true;
	}

	/// <summary>
	/// Stop the specified routine.
	/// </summary>
	/// <returns>True if the routine was actually stopped.</returns>
	/// <param name="routine">The routine to stop.</param>
	public bool Stop(CoroutineHandle routine)
	{
		return routine.Stop();
	}

	/// <summary>
	/// Stop all running routines.
	/// </summary>
	public void StopAll()
	{
		running.Clear();
		delays.Clear();
	}

	/// <summary>
	/// Check if the routine is currently running.
	/// </summary>
	/// <returns>True if the routine is running.</returns>
	/// <param name="routine">The routine to check.</param>
	public bool IsRunning(IEnumerator routine)
	{
		return running.Contains(routine);
	}

	/// <summary>
	/// Check if the routine is currently running.
	/// </summary>
	/// <returns>True if the routine is running.</returns>
	/// <param name="routine">The routine to check.</param>
	public bool IsRunning(CoroutineHandle routine)
	{
		return routine.IsRunning;
	}

	internal void Update()
	{
		if (running.Count == 0)
			return;

		for (int i = 0; i < running.Count; i++)
		{
			if (delays[i] > 0f)
				delays[i] -= Clock.Instance.DeltaTime;
			else if (running[i] == null || !MoveNext(running[i], i))
			{
				running.RemoveAt(i);
				delays.RemoveAt(i--);
			}
		}
	}

	bool MoveNext(IEnumerator routine, int index)
	{
		if (routine.Current is IEnumerator enumerator)
		{
			if (MoveNext(enumerator, index))
				return true;

			delays[index] = 0f;
		}

		bool result = routine.MoveNext();

		if (routine.Current is float fValue)
			delays[index] = fValue;
		else if (routine.Current is double dValue)
			delays[index] = (float)dValue;
		else if (routine.Current is int iValue)
			delays[index] = iValue;

		return result;
	}

	/// <summary>
	/// How many coroutines are currently running.
	/// </summary>
	public int Count
	{
		get { return running.Count; }
	}
}
