namespace Box.Coroutines.Routines.Time;

/// <summary>
/// Waits until a specified signal is emitted, then invokes a callback and resumes the coroutine.
/// </summary>
/// <remarks>
/// This routine subscribes to a named signal upon creation and unsubscribes automatically when the signal fires.
/// </remarks>
public sealed class WaitForSignal : IEnumerator
{
	private bool _done;
	private string _signalName;
	private readonly Action<SignalHandle> _callback;
	private Signal _signalService;

	/// <summary>
	/// Gets the current yield value, which is the <see cref="SignalHandle"/> passed when the signal is emitted.
	/// </summary>
	public object Current { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitForSignal"/> class.
	/// Subscribes to the specified signal name and will invoke the provided callback when the signal is emitted.
	/// </summary>
	/// <param name="signalName">The name of the signal to wait for.</param>
	/// <param name="callback">The action to invoke with the <see cref="SignalHandle"/> when the signal is raised.</param>
	public WaitForSignal(string signalName, Action<SignalHandle> callback)
	{
		_signalName = signalName;
		_callback = callback;

		Signal.Instance.Connect(_signalName, OnSignalEmitted);
	}

	private void OnSignalEmitted(SignalHandle handle)
	{
		Current = handle;

		_signalService.Disconnect(_signalName, OnSignalEmitted);
		_callback?.Invoke(handle);
		_done = true;
	}

	/// <summary>
	/// Advances the wait; returns <c>true</c> if still waiting for the signal, or <c>false</c> to resume the coroutine.
	/// </summary>
	/// <returns>
	/// <c>true</c> while waiting for the signal; <c>false</c> once the signal has been emitted.
	/// </returns>
	public bool MoveNext() => !_done;

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
