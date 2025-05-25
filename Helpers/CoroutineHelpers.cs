namespace Box.Helpers;

/// <summary>
/// Helper class for managing coroutines and providing coroutine-related utilities.
/// </summary>
public static class CoroutineHelper
{
	/// <summary>
	/// Executes multiple coroutines one after another in sequence, waiting for each to complete before starting the next.
	/// </summary>
	/// <param name="routines">An array of coroutine routines to execute sequentially. Each routine must be a valid <see cref="IEnumerator"/>.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that completes after all routines have run to completion, in order.</returns>
	/// <remarks>
	/// This method runs each coroutine in the order provided, waiting for one to finish before starting the next.
	/// It is ideal for chaining dependent animations, scripted events, or staged operations.
	///
	/// If <paramref name="routines"/> is empty, the method exits immediately without yielding.
	/// </remarks>
	/// <example>
	/// Example usage:
	/// <code>
	/// yield return Coroutine.WaitSequentially(
	///     FadeOut(panel),
	///     Delay(0.5f),
	///     SlideIn(menu)
	/// );
	/// // Execution continues only after all three routines complete, in order.
	/// </code>
	/// </example>
	public static IEnumerator WaitSequentially(params IEnumerator[] routines)
	{
		if (routines.Length == 0)
			yield break;

		foreach (var routine in routines)
		{
			yield return routine;
		}
	}

	/// <summary>
	/// Executes multiple coroutines in parallel and waits until all of them have completed.
	/// </summary>
	/// <param name="routines">An array of coroutine routines to run in parallel. Each routine must be a valid <see cref="IEnumerator"/>.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that completes when all provided routines have finished execution.</returns>
	/// <remarks>
	/// This method starts all coroutines simultaneously and yields control until every routine has completed. It is ideal for running
	/// multiple animations, transitions, or operations in parallel where continuation should only happen after all have finished.
	///
	/// If <paramref name="routines"/> is null or empty, the method exits immediately without yielding.
	/// </remarks>
	/// <example>
	/// Example usage:
	/// <code>
	/// yield return Coroutine.WaitAll(
	///     FadeOut(panel),
	///     SlideIn(menu),
	///     Delay(1.5f)
	/// );
	/// // Continue only after all three have completed
	/// </code>
	/// </example>
	public static IEnumerator WaitAll(params IEnumerator[] routines)
	{
		if (routines == null || routines.Length == 0)
			yield break;

		var runners = new List<CoroutineHandle>();

		foreach (var routine in routines)
			runners.Add(Engine.GetService<Coroutine>().Run(routine)); // assumes you have a coroutine system that returns handles

		while (runners.Any(r => r.IsRunning))
			yield return null;
	}

	/// <summary>
	/// Delays coroutine execution for the specified number of real-time seconds.
	/// </summary>
	/// <param name="seconds">The duration in seconds to wait before resuming execution. Must be a non-negative number.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that waits for the specified time duration.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="seconds"/> is less than zero or not a valid number.</exception>
	/// <remarks>
	/// This method uses frame delta time and is ideal for time-based transitions or delays in gameplay, UI, or animations.
	/// </remarks>
	public static IEnumerator Delay(float seconds)
	{
		if (seconds < 0f || float.IsNaN(seconds))
			throw new ArgumentOutOfRangeException(nameof(seconds), "Delay duration must be a non-negative, valid number.");

		float elapsed = 0f;

		while ((elapsed += Engine.GetService<Clock>().DeltaTime) < seconds)
			yield return null;
	}

	/// <summary>
	/// Delays coroutine execution for the specified number of rendered frames.
	/// </summary>
	/// <param name="frames">The number of frames to wait before resuming execution. Must be non-negative.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that waits for the specified number of frames.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frames"/> is less than zero.</exception>
	/// <remarks>
	/// This method is frame-accurate and does not depend on time-based delta values. It is ideal for precise control
	/// in animations, transitions, and frame-specific logic in rendering pipelines.
	/// </remarks>
	public static IEnumerator WaitFrames(int frames)
	{
		if (frames < 0)
			throw new ArgumentOutOfRangeException(nameof(frames), "WaitFrames must be given a non-negative number of frames.");

		while (frames-- > 0)
			yield return null;
	}

	/// <summary>
	/// Waits until the specified condition becomes true.
	/// </summary>
	/// <param name="condition">A delegate representing the condition to evaluate each frame.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that yields control until the condition evaluates to true.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> is null.</exception>
	/// <remarks>
	/// This method is useful for waiting on deferred state changes, such as waiting for a screen or entity to be initialized.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return Coroutine.WaitFor(() => screen != null);
	/// // Continue only after screen is assigned
	/// </code>
	/// </example>
	public static IEnumerator WaitWhile(Func<bool> condition)
	{
		if (condition == null)
			throw new ArgumentNullException(nameof(condition));

		while (condition.Invoke())
		{
			yield return null;
		}
	}

	/// <summary>
	/// Waits for a reference object to become non-null, with an optional timeout.
	/// </summary>
	/// <typeparam name="T">The expected type of the object being waited on.</typeparam>
	/// <param name="obj">A delegate that returns the object to monitor.</param>
	/// <param name="duration">The maximum duration in seconds to wait before timing out. Defaults to 1 second.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that yields control until the object is assigned or the timeout is reached.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
	/// <remarks>
	/// This is useful when waiting for an asynchronously initialized object (such as a screen, entity, or resource)
	/// to be assigned or become available. If the object is still null after the timeout, the coroutine will continue regardless.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return Coroutine.WaitForObject&lt;MyScreen&gt;(() => currentScreen);
	/// if (currentScreen != null)
	///     currentScreen.Show();
	/// </code>
	/// </example>
	public static IEnumerator WaitForObject<T>(Func<object> obj, float duration = 1f)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		float timeout = 0f;

		while (obj() is null && timeout <= duration)
		{
			yield return null;

			timeout += Engine.GetService<Clock>().DeltaTime;
		}
	}

	/// <summary>
	/// Generates an enumerable collection of idle frame.
	/// </summary>
	/// <returns>An IEnumerable representing the idle frame.</returns>
	public static IEnumerator IdleFrame()
	{
		yield return null;
	}

	/// <summary>
	/// Waits for a reference object to become non-null and optionally invokes an action when the object is available.
	/// </summary>
	/// <typeparam name="T">The expected type of the object being waited on.</typeparam>
	/// <param name="obj">A delegate that returns the object to monitor.</param>
	/// <param name="action">An optional callback to invoke when the object becomes available.</param>
	/// <param name="duration">The maximum duration in seconds to wait before timing out. Defaults to 1 second.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that yields control until the object is assigned or the timeout is reached.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
	/// <remarks>
	/// This is useful for asynchronously waiting on an object (such as a screen, entity, or resource) to be created or initialized.
	/// If the object remains null after the timeout, the action will not be invoked.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return Coroutine.WaitForObject&lt;Screen&gt;(
	///     () => screenManager.Current,
	///     screen => screen.Show(),
	///     2f
	/// );
	/// </code>
	/// </example>
	public static IEnumerator WaitForObject<T>(Func<object> obj, Action<T> action = null, float duration = 1f)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		float timeout = 0f;
		bool canExecute = true;

		while (obj() is null)
		{
			yield return null;

			timeout += Engine.GetService<Clock>().DeltaTime;
			if (timeout > duration)
			{
				canExecute = false;
				break;
			}
		}

		if (canExecute && obj() is T typed)
			action?.Invoke(typed);
	}



	/// <summary>
	/// Plays a non-looping animation on the given sprite and waits until it finishes.
	/// </summary>
	/// <param name="sprite">The <see cref="AnimatedSprite"/> to animate.</param>
	/// <param name="name">The name of the animation to play.</param>
	/// <param name="resetAnimation">If true, resets the animation before playing. Defaults to true.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that completes once the animation has finished playing.</returns>
	/// <remarks>
	/// This method safely handles null references, missing animations, and looping animations. It will not yield
	/// if the animation is looping or invalid.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return Coroutine.WaitForAnimation(playerSprite, "attack");
	/// DoNextThing();
	/// </code>
	/// </example>
	public static IEnumerator WaitForAnimation(AnimatedSprite sprite, string name, bool resetAnimation = true)
	{
		if (sprite is null)
			yield break;

		if (!sprite.Exists(name))
			yield break;

		if (sprite.Get(name).Looped)
			yield break;

		sprite.EnginePlay(name, resetAnimation);

		while (sprite.IsPlaying)
			yield return null;
	}

	/// <summary>
	/// Plays a non-looping animation on the given sprite using an <see cref="Enum"/> name and waits until it finishes.
	/// </summary>
	/// <param name="sprite">The <see cref="AnimatedSprite"/> to animate.</param>
	/// <param name="name">The <see cref="Enum"/> representing the name of the animation to play.</param>
	/// <param name="resetAnimation">If true, resets the animation before playing. Defaults to true.</param>
	/// <returns>Returns an <see cref="IEnumerator"/> that completes once the animation has finished playing.</returns>
	/// <remarks>
	/// This overload converts the enum value to a string using <c>ToEnumString()</c> and plays the corresponding animation.
	/// It is a type-safe alternative to passing raw strings and helps avoid typos or invalid references.
	/// </remarks>
	/// <example>
	/// <code>
	/// yield return Coroutine.WaitForAnimation(sprite, PlayerAnimation.Attack);
	/// </code>
	/// </example>
	public static IEnumerator WaitForAnimation(AnimatedSprite sprite, Enum name, bool resetAnimation = true)
		=> WaitForAnimation(sprite, name.ToEnumString(), resetAnimation);
}
