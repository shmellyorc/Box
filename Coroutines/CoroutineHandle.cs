namespace Box.Coroutines;

/// <summary>
/// Represents a handle to a running coroutine, allowing control and status queries.
/// </summary>
public readonly struct CoroutineHandle : IDisposable, IEquatable<CoroutineHandle>
{
	private static readonly Coroutine Svc = Engine.GetService<Coroutine>();

	/// <summary>
	/// Represents an uninitialized or non-existent coroutine handle.
	/// </summary>
	public static readonly CoroutineHandle None = default;

	/// <summary>
	/// The underlying IEnumerator of the coroutine.
	/// </summary>
	public IEnumerator Enumerator { get; }

	/// <summary>
	/// Indicates whether this handle references a valid coroutine.
	/// </summary>
	public bool IsValid => Enumerator != null;

	/// <summary>
	/// Indicates whether the referenced coroutine is currently running.
	/// </summary>
	public bool IsRunning => IsValid && Svc.IsRunning(Enumerator);

	internal CoroutineHandle(IEnumerator routine) => Enumerator = routine;

	/// <summary>
	/// Stops the referenced coroutine if it is running.
	/// </summary>
	/// <returns><c>true</c> if the coroutine was successfully stopped; otherwise, <c>false</c>.</returns>
	public bool Stop() => IsRunning && Svc.Stop(Enumerator);

	/// <summary>
	/// Disposes the coroutine handle by stopping the coroutine if it is running.
	/// </summary>
	public void Dispose() => Stop();

	/// <summary>
	/// Implicitly converts the handle to <c>true</c> if the coroutine is running.
	/// </summary>
	/// <param name="h">The coroutine handle to evaluate.</param>
	/// <returns><c>true</c> if <paramref name="h"/> is running; otherwise, <c>false</c>.</returns>
	public static implicit operator bool(CoroutineHandle h) => h.IsRunning;

	/// <summary>
	/// Determines whether this instance and another specified <see cref="CoroutineHandle"/> have the same 
	/// underlying routine.
	/// </summary>
	/// <param name="other">The other coroutine handle to compare.</param>
	/// <returns><c>true</c> if both handles reference the same coroutine; otherwise, <c>false</c>.</returns>

	public readonly bool Equals(CoroutineHandle other) =>
		Enumerator == other.Enumerator;

	/// <summary>
	/// Determines whether this instance and a specified object, which must also be a <see cref="CoroutineHandle"/>, 
	/// have the same value.
	/// </summary>
	/// <param name="obj">The object to compare to this instance.</param>
	/// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="CoroutineHandle"/> and references the same 
	/// coroutine; otherwise, <c>false</c>.
	/// </returns>
	public readonly override bool Equals(object obj) =>
		obj is CoroutineHandle h && Equals(h);

	/// <summary>
	/// Returns the hash code for this coroutine handle.
	/// </summary>
	/// <returns>An integer hash code representing the handle.</returns>
	public readonly override int GetHashCode() =>
		Enumerator?.GetHashCode() ?? 0;

	/// <summary>
	/// Returns a string that represents the current coroutine handle.
	/// </summary>
	/// <returns>A string representation of this handle, including the routine and its running state.</returns>
	public readonly override string ToString() =>
		$"CoroutineHandle[{Enumerator}, Running={IsRunning}]";

	/// <summary>
	/// Determines whether two <see cref="CoroutineHandle"/> instances are equal,
	/// by comparing their underlying enumerators.
	/// </summary>
	/// <param name="left">The first handle to compare.</param>
	/// <param name="right">The second handle to compare.</param>
	/// <returns>
	/// <c>true</c> if both handles refer to the same coroutine; otherwise <c>false</c>.
	/// </returns>
	public static bool operator ==(CoroutineHandle left, CoroutineHandle right) => left.Equals(right);

	/// <summary>
	/// Determines whether two <see cref="CoroutineHandle"/> instances are not equal.
	/// </summary>
	/// <param name="left">The first handle to compare.</param>
	/// <param name="right">The second handle to compare.</param>
	/// <returns>
	/// <c>true</c> if the handles refer to different coroutines; otherwise <c>false</c>.
	/// </returns>
	public static bool operator !=(CoroutineHandle left, CoroutineHandle right) => !(left == right);
}
