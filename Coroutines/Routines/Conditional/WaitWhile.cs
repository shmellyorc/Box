namespace Box.Coroutines.Routines.Conditional;

/// <summary>
/// Waits while the given predicate returns true.
/// </summary>
public class WaitWhile : IEnumerator
{
	private readonly Func<bool> _predicate;

	/// <summary>
	/// Initializes a new instance of the <see cref="WaitWhile"/> class.
	/// </summary>
	/// <param name="predicate">
	/// A function evaluated each frame; the coroutine will continue waiting while this returns <c>true</c>.
	/// </param>
	public WaitWhile(Func<bool> predicate) => _predicate = predicate;

	/// <summary>
	/// Gets the current yield value (always <c>null</c> for this instruction).
	/// </summary>
	public object Current => null;

	/// <summary>
	/// Advances the wait by invoking the predicate.
	/// </summary>
	/// <returns>
	/// <c>true</c> if still waiting (predicate returned <c>true</c>); otherwise <c>false</c> to resume the coroutine.
	/// </returns>
	public bool MoveNext() => _predicate();

	/// <summary>
	/// Not supported. Calling this method will always throw a <see cref="NotSupportedException"/>.
	/// </summary>
	public void Reset() => throw new NotSupportedException();
}
