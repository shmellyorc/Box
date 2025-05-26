using Box.Coroutines.Routines;
using Box.Coroutines.Routines.Time;
using Box.Services.Types;

namespace Box.Coroutines;

/// <summary>
/// A container for running multiple coroutines in parallel.
/// Provides methods to start, stop, and query coroutines.
/// </summary>
public sealed class Coroutine : UpdatableService
{
	private readonly List<CoroutineJob> _jobs = new();

	/// <summary>
	/// Gets the number of currently active coroutines.
	/// </summary>
	public int Count => _jobs.Count;


	/// <summary>
	/// Starts a coroutine immediately.
	/// </summary>
	/// <param name="routine">The IEnumerator routine to run.</param>
	/// <returns>A handle to control the started coroutine.</returns>
	public CoroutineHandle Run(IEnumerator routine) => EngineRun(null, routine);
	internal CoroutineHandle EngineRun(object owner, IEnumerator routine) =>
		EngineRunDelayed(owner, 0f, routine);

	/// <summary>
	/// Starts a coroutine after an initial delay.
	/// </summary>
	/// <param name="delay">Seconds to wait before starting the coroutine.</param>
	/// <param name="routine">The IEnumerator routine to run.</param>
	/// <returns>A handle to control the started coroutine.</returns>
	public CoroutineHandle RunDelayed(float delay, IEnumerator routine)
		=> EngineRunDelayed(null, delay, routine);
	internal CoroutineHandle EngineRunDelayed(object owner, float delay, IEnumerator routine)
	{
		_jobs.Add(new CoroutineJob(owner, routine, delay));

		return new CoroutineHandle(routine);
	}

	/// <summary>
	/// Stops the specified coroutine if it is running.
	/// </summary>
	/// <param name="routine">The IEnumerator routine to stop.</param>
	/// <returns><c>true</c> if the coroutine was found and stopped; otherwise, <c>false</c>.</returns>
	public bool Stop(IEnumerator routine)
	{
		for (int i = 0; i < _jobs.Count; i++)
		{
			if (_jobs[i].Routine == routine)
			{
				_jobs.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Stops the specified coroutine via its handle.
	/// </summary>
	/// <param name="handle">The handle of the coroutine to stop.</param>
	/// <returns><c>true</c> if the coroutine was stopped; otherwise, <c>false</c>.</returns>
	public bool Stop(CoroutineHandle handle) => handle.Stop();

	/// <summary>
	/// Determines whether the specified IEnumerator routine is currently running.
	/// </summary>
	/// <param name="routine">The IEnumerator routine to query.</param>
	/// <returns><c>true</c> if the routine is active; otherwise, <c>false</c>.</returns>
	public bool IsRunning(IEnumerator routine) =>
		_jobs.Exists(j => j.Routine == routine);

	/// <summary>
	/// Determines whether the specified coroutine handle is currently running.
	/// </summary>
	/// <param name="handle">The handle to query.</param>
	/// <returns><c>true</c> if the handle's coroutine is active; otherwise, <c>false</c>.</returns>
	public bool IsRunning(CoroutineHandle handle) => handle.IsRunning;

	internal void StopAll(object owner = null)
	{
		if (owner == null)
		{
			_jobs.Clear();
			return;
		}

		_jobs.RemoveAll(job => ReferenceEquals(job.Owner, owner));
	}

	/// <inheritdoc/>
	public override void Update()
	{
		if (_jobs.Count == 0) return;

		for (int i = _jobs.Count - 1; i >= 0; i--)
		{
			var job = _jobs[i];
			if (job.Delay > 0f)
			{
				job.Delay -= Clock.DeltaTime;
				continue;
			}

			if (!MoveNext(ref job))
				_jobs.RemoveAt(i);
		}

		base.Update();
	}

	// Internal: advances a single job one step, returns false if complete.
	private bool MoveNext(ref CoroutineJob job)
	{
		var routine = job.Routine;

		if (routine.Current is IEnumerator nested)
		{
			var nestedJob = new CoroutineJob(job.Owner, nested, 0f);

			if (MoveNext(ref nestedJob))
			{
				job.Routine = Wrap(nestedJob.Routine, routine);
				job.Delay = nestedJob.Delay;

				return true;
			}
		}

		if (!routine.MoveNext()) return false;

		switch (routine.Current)
		{
			case float f:
				job.Routine = Wrap(new WaitForSeconds(f), routine);
				job.Delay = 0f;
				return true;
			case double d:
				job.Routine = Wrap(new WaitForSeconds((float)d), routine);
				job.Delay = 0f;
				return true;
			case int n:
				job.Routine = Wrap(new WaitForSeconds(n), routine);
				job.Delay = 0f;
				return true;
			default:
				break; // do nothing...
		}

		if (routine.Current is IEnumerator anyEnum)
		{
			job.Routine = Wrap(anyEnum, routine);
			job.Delay = 0f;

			return true;
		}

		job.Delay = 0f;

		return true;
	}

	// Internal: helper to resume parent after first completes.
	private static IEnumerator Wrap(IEnumerator first, IEnumerator parent)
	{
		yield return first;
		yield return parent;
	}
}
