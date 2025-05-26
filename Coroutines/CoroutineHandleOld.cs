// /*
//     MIT License

//     Copyright (c) 2017 Chevy Ray Johnston

//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:

//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.

//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// */

// namespace Box.Coroutines;

// /// <summary>
// /// A handle to a (potentially running) coroutine.
// /// </summary>
// public readonly struct CoroutineHandle : IEquatable<CoroutineHandle>
// {
// 	public Coroutine Coroutine => Engine.GetService<Coroutine>();

// 	/// <summary>
// 	/// Reference to the routine's enumerator.
// 	/// </summary>
// 	public IEnumerator Enumerator { get; }

//     /// <summary>
//     /// True if the enumerator is currently running.
//     /// </summary>
//     public bool IsRunning
//         => Enumerator is not null && Coroutine.IsRunning(Enumerator);

//     internal CoroutineHandle(IEnumerator routine) => Enumerator = routine;

//     /// <summary>
//     /// Stop this coroutine if it is running.
//     /// </summary>
//     /// <returns>True if the coroutine was stopped.</returns>
//     public bool Stop() => IsRunning && Coroutine.Stop(Enumerator);

//     /// <summary>
//     /// A routine to wait until this coroutine has finished running.
//     /// </summary>
//     /// <returns>The wait enumerator.</returns>
//     public IEnumerator Wait()
//     {
//         if (Enumerator == null)
//             yield break;

//         while (Coroutine.IsRunning(Enumerator))
//             yield return null;
//     }

//     /// <summary>
//     /// Checks if two coroutine handles are equal.
//     /// </summary>
//     /// <param name="left">The left-hand side coroutine handle.</param>
//     /// <param name="right">The right-hand side coroutine handle.</param>
//     /// <returns>True if the coroutine handles are equal, false otherwise.</returns>
//     public static bool operator ==(CoroutineHandle left, CoroutineHandle right) => left.Enumerator == right.Enumerator;

//     /// <summary>
//     /// Checks if two coroutine handles are not equal.
//     /// </summary>
//     /// <param name="left">The left-hand side coroutine handle.</param>
//     /// <param name="right">The right-hand side coroutine handle.</param>
//     /// <returns>True if the coroutine handles are not equal, false otherwise.</returns>
//     public static bool operator !=(CoroutineHandle left, CoroutineHandle right) => !(left == right);

//     /// <summary>
//     /// Checks if this coroutine handle is equal to another coroutine handle.
//     /// </summary>
//     /// <param name="other">The coroutine handle to compare.</param>
//     /// <returns>True if this coroutine handle is equal to the specified coroutine handle, false otherwise.</returns>
//     public bool Equals(CoroutineHandle other) => Enumerator == other.Enumerator;

//     /// <summary>
//     /// Checks if this coroutine handle is equal to another object.
//     /// </summary>
//     /// <param name="obj">The object to compare.</param>
//     /// <returns>True if this coroutine handle is equal to the specified object, false otherwise.</returns>
//     public override bool Equals([NotNullWhen(true)] object obj)
//     {
//         if (obj is CoroutineHandle value)
//             return Equals(value);

//         return false;
//     }

//     /// <summary>
//     /// Returns the hash code for this coroutine handle.
//     /// </summary>
//     /// <returns>A hash code value for this coroutine handle.</returns>
//     public override int GetHashCode() => HashCode.Combine(Enumerator);

//     /// <summary>
//     /// Returns a string representation of this coroutine handle.
//     /// </summary>
//     /// <returns>A string that represents this coroutine handle.</returns>
//     public override string ToString()
//     {
// 		var sb = new StringBuilder();

// 		sb.Append($"Enumerator: {Enumerator}, IsRunning: {IsRunning}");

//         return sb.ToString();
//     }
// }

