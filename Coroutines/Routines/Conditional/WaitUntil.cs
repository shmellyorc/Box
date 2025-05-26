namespace Box.Coroutines.Routines.Conditional;

/// <summary>
/// Waits until the given predicate returns true.
/// </summary>
public class WaitUntil : IEnumerator
{
	readonly Func<bool> _predicate;

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitUntil"/> class.
	/// </summary>
	/// <param name="predicate">
	/// The function to evaluate each frame. The coroutine will resume when this returns <c>true</c>.
	/// </param>
	public WaitUntil(Func<bool> predicate) => _predicate = predicate;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Advances the wait by invoking the predicate.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (predicate has not returned <c>true</c>); otherwise <c>false</c> to resume the coroutine.
	/// </returns>
	public bool MoveNext() => !_predicate();

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
