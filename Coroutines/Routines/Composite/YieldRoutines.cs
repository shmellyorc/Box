using Box.Coroutines.Routines.Conditional;
using Box.Coroutines.Routines.Time;

namespace Box.Coroutines.Routines.Composite;

/// <summary>
/// Provides a set of composite coroutine routines for coordinating and composing multiple
/// <see cref="IEnumerator"/> instances. Includes helpers for parallel execution (e.g., <c>WaitForAll</c>),
/// racing (<c>WaitForAny</c>), sequencing (<c>Sequence</c>), timeouts, conditional waits,
/// and other advanced coroutine workflows.
/// </summary>
public static class YieldRoutines
{
	/// <summary>
	/// Starts each provided coroutine in parallel and waits until every one has finished.
	/// </summary>
	/// <param name="routines">One or more <see cref="IEnumerator"/> routines to run in parallel.</param>
	/// <returns>
	/// An <see cref="IEnumerator"/> you can yield that will resume only when all routines complete.
	/// </returns>
	public static IEnumerator WaitForAll(params IEnumerator[] routines)
	{
		// Fire off each routine
		var handles = new List<CoroutineHandle>(routines.Length);

		foreach (var routine in routines)
			handles.Add(Coroutine.Instance.Run(routine));

		// Wait until none are still running
		yield return new WaitUntil(() => handles.All(h => !h.IsRunning));
	}

	/// <summary>
	/// Starts all provided routines in parallel and waits until at least one has finished.
	/// </summary>
	/// <param name="routines">One or more IEnumerator routines to run in parallel.</param>
	/// <returns>An IEnumerator that resumes when any routine has completed.</returns>
	public static IEnumerator WaitForAny(params IEnumerator[] routines)
	{
		var handles = new List<CoroutineHandle>(routines.Length);

		foreach (var routine in routines)
			handles.Add(Coroutine.Instance.Run(routine));

		yield return new WaitUntil(() => handles.Any(h => !h.IsRunning));
	}

	/// <summary>
	/// Runs each provided routine sequentially, waiting for one to finish before starting the next.
	/// </summary>
	/// <param name="routines">One or more IEnumerator routines to run in sequence.</param>
	/// <returns>An IEnumerator that completes when all routines have run in order.</returns>
	public static IEnumerator Sequence(params IEnumerator[] routines)
	{
		foreach (var routine in routines)
			yield return routine;
	}

	/// <summary>
	/// Runs the specified routine but cancels it if the timeout elapses before completion.
	/// </summary>
	/// <param name="routine">The IEnumerator routine to run.</param>
	/// <param name="timeout">Maximum seconds to wait before canceling.</param>
	/// <returns>An IEnumerator that completes when the routine finishes or is canceled due to timeout.</returns>
	public static IEnumerator WithTimeout(IEnumerator routine, float timeout)
	{
		float elapsed = 0f;

		var handle = Coroutine.Instance.Run(routine);

		while (handle.IsRunning && elapsed < timeout)
		{
			elapsed += Clock.Instance.DeltaTime;
			yield return null;
		}

		if (handle.IsRunning)
			handle.Stop();
	}

	/// <summary>
	/// Starts all provided routines in parallel, waits for any to complete, then cancels all still running.
	/// </summary>
	/// <param name="routines">One or more IEnumerator routines to run in parallel.</param>
	/// <returns>An IEnumerator that resumes when one routine completes and all others are canceled.</returns>
	public static IEnumerator WaitForAnyAndCancel(params IEnumerator[] routines)
	{
		var handles = new List<CoroutineHandle>(routines.Length);

		foreach (var routine in routines)
			handles.Add(Coroutine.Instance.Run(routine));

		// wait for one to finish
		yield return new WaitUntil(() => handles.Any(h => !h.IsRunning));

		// cancel all still running
		foreach (var h in handles.Where(h => h.IsRunning))
			h.Stop();
	}

	/// <summary>
	/// Waits until the value returned by <paramref name="getter"/> is non-null.
	/// </summary>
	/// <typeparam name="T">
	/// The reference type being checked.  Must be a class so it can be null.
	/// </typeparam>
	/// <param name="getter">
	/// A function that returns the current instance of <typeparamref name="T"/> to test for null.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerator"/> that will continue yielding until <paramref name="getter"/> returns a non-null value.
	/// </returns>
	public static IEnumerator WaitUntilNotNull<T>(Func<T> getter) where T : class
	{
		yield return new WaitUntil(() => getter() != null);
	}

	public static IEnumerator WaitUntilNotNullThan<T>(Func<T> getter, Action<T> action) where T : class
	{
		yield return new WaitUntil(() => getter() != null);

		action?.Invoke(getter());
	}

	/// <summary>
	/// Waits until the value returned by <paramref name="getter"/> is non-null,
	/// or until the specified timeout (in seconds) elapses.
	/// </summary>
	/// <typeparam name="T">
	/// The reference type being checked for null.
	/// </typeparam>
	/// <param name="getter">
	/// A function that returns the current instance of <typeparamref name="T"/> to test.
	/// </param>
	/// <param name="timeoutSeconds">
	/// Maximum number of seconds to wait before giving up.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerator"/> that will resume when <paramref name="getter"/>
	/// returns a non-null value or when <paramref name="timeoutSeconds"/> has elapsed.
	/// </returns>
	public static IEnumerator WaitUntilNotNull<T>(Func<T> getter, float timeoutSeconds) where T : class
	{
		float elapsed = 0f;

		while (getter() == null && elapsed < timeoutSeconds)
		{
			elapsed += Clock.Instance.DeltaTime;

			yield return null;
		}
	}

	/// <summary>
	/// Starts the given animation (if it exists and isn’t looped) and waits
	/// until playback completes.
	/// </summary>
	/// <param name="sprite">
	/// The sprite on which to play the animation. If <c>null</c>, the coroutine ends immediately.
	/// </param>
	/// <param name="animationName">
	/// The name of the animation to play. If it doesn’t exist or is a looped animation, the coroutine ends immediately.
	/// </param>
	/// <param name="resetAnimation">
	/// If <c>true</c>, the animation’s progress is reset before playback.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerator"/> you can <c>yield return</c> that will resume
	/// when the animation is no longer playing (or immediately if it couldn’t be started).
	/// </returns>
	public static IEnumerator WaitForAnimation(AnimatedSprite sprite, string animationName, bool resetAnimation = true)
	{
		// sanity checks
		if (sprite == null) yield break;
		if (!sprite.Exists(animationName)) yield break;
		if (sprite.Get(animationName).Looped) yield break;

		// start playback
		sprite.EnginePlay(animationName, resetAnimation);

		// small safety: wait one frame for IsPlaying to go true
		yield return new WaitForNextFrame();

		// now wait until it stops
		yield return new WaitUntil(() => !sprite.IsPlaying);
	}

	/// <summary>
	/// Starts the given animation (if it exists and isn’t looped) and waits
	/// until playback completes.
	/// </summary>
	/// <param name="sprite">
	/// The sprite on which to play the animation. If <c>null</c>, the coroutine ends immediately.
	/// </param>
	/// <param name="animationName">
	/// The name of the animation to play. If it doesn’t exist or is a looped animation, the coroutine ends 
	/// immediately.
	/// </param>
	/// <param name="resetAnimation">
	/// If <c>true</c>, the animation’s progress is reset before playback.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerator"/> you can <c>yield return</c> that will resume
	/// when the animation is no longer playing (or immediately if it couldn’t be started).
	/// </returns>
	public static IEnumerator WaitForAnimation(AnimatedSprite sprite, Enum animationName, bool resetAnimation = true)
		=> WaitForAnimation(sprite, animationName.ToEnumString(), resetAnimation);
}
