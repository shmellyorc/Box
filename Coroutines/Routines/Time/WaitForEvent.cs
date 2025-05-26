namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Waits until a specified event is raised, then resumes the coroutine.
/// </summary>
/// <typeparam name="T">
/// The type of the event argument.
/// </typeparam>
public sealed class WaitForEvent<T> : IEnumerator
{
	private readonly Action<Action<T>> _subscribe;
	private readonly Action<Action<T>> _unsubscribe;
	private bool _done;

	/// <summary>
	/// Gets the current yield value, which is the event argument passed when the event fires.
	/// </summary>
	public object Current { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitForEvent{T}"/> class.
	/// Subscribes to the event using <paramref name="subscribe"/>, and will unsubscribe via <paramref name="unsubscribe"/>
	/// when the event is fired.
	/// </summary>
	/// <param name="subscribe">
	/// Action that takes a handler to attach to the event.
	/// </param>
	/// <param name="unsubscribe">
	/// Action that takes a handler to detach from the event.
	/// </param>
	public WaitForEvent(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
	{
		_subscribe = subscribe;
		_unsubscribe = unsubscribe;
		_subscribe(OnFired);
	}

	private void OnFired(T arg)
	{
		Current = arg;
		_done = true;
		_unsubscribe(OnFired);
	}

	/// <summary>
	/// Advances the wait; returns <c>true</c> if still waiting for the event, 
	/// or <c>false</c> to resume the coroutine.
	/// </summary>
	/// <returns>
	/// <c>true</c> while waiting for the event; <c>false</c> once the event has fired.
	/// </returns>
	public bool MoveNext() => !_done;

	/// <summary>
	/// Reset is not supported for this instruction and does nothing.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
