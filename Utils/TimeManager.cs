namespace Box.Utils;

/// <summary>
/// Manages time-related operations and calculations in the game.
/// </summary>
public sealed class TimeManager
{
    private readonly Dictionary<string, CoroutineHandle> _timers = new();

    /// <summary>
    /// Gets the number of active timers managed by the TimeManager.
    /// </summary>
    public int Count => _timers.Count;

    /// <summary>
    /// Adds a timed action to be managed by the TimeManager.
    /// </summary>
    /// <param name="name">The unique name of the timed action.</param>
    /// <param name="delay">The delay in seconds before the action starts.</param>
    /// <param name="repeat">Specifies if the action should repeat.</param>
    /// <param name="action">The action to execute when the timer expires.</param>
    public void Add(string name, float delay, bool repeat, Action action)
    {
        if (Exists(name))
            return;

        IEnumerator Routine(string name, float delay, bool repeat, Action action)
        {
            if (repeat)
            {
                while (true)
                {
                    yield return delay;

                    action?.Invoke();
                }
            }
            else
            {
                yield return delay;

                action?.Invoke();

                _timers.Remove(name);
            }
        }

        _timers.Add(name, Coroutine.Instance.Run(Routine(name, delay, repeat, action)));
    }

    /// <summary>
    /// Adds a timed action identified by an enum name to be managed by the TimeManager.
    /// </summary>
    /// <param name="name">The enum value that identifies the timed action.</param>
    /// <param name="delay">The delay in seconds before the action starts.</param>
    /// <param name="repeat">Specifies if the action should repeat.</param>
    /// <param name="action">The action to execute when the timer expires.</param>
    public void Add(Enum name, float delay, bool repeat, Action action)
        => Add(name.ToEnumString(), delay, repeat, action);

    /// <summary>
    /// Adds a timed action to be managed by the TimeManager.
    /// </summary>
    /// <param name="delay">The delay in seconds before the action starts.</param>
    /// <param name="repeat">Specifies if the action should repeat.</param>
    /// <param name="action">The action to execute when the timer expires.</param>
    public void Add(float delay, bool repeat, Action action)
        => Add(Guid.NewGuid().ToString(), delay, repeat, action);

    /// <summary>
    /// Checks if a timer with the specified name exists in the TimeManager.
    /// </summary>
    /// <param name="name">The name of the timer to check.</param>
    /// <returns>True if a timer with the specified name exists; otherwise, false.</returns>
    public bool Exists(string name) => _timers.ContainsKey(name);

    /// <summary>
    /// Checks if a timer with the specified enum name exists in the TimeManager.
    /// </summary>
    /// <param name="name">The enum representing the name of the timer to check.</param>
    /// <returns>True if a timer with the specified enum name exists; otherwise, false.</returns>
    public bool Exists(Enum name) => Exists(name.ToEnumString());

    /// <summary>
    /// Stops and removes the timer with the specified name from the TimeManager.
    /// </summary>
    /// <param name="name">The name of the timer to stop and remove.</param>
    /// <returns>True if the timer was successfully found and stopped; otherwise, false.</returns>
    public bool Stop(string name)
    {
        if (!Exists(name))
            return false;

        var timer = _timers[name];

        timer.Stop();

        return _timers.Remove(name);
    }

    /// <summary>
    /// Stops and removes the timer associated with the specified Enum identifier from the TimeManager.
    /// </summary>
    /// <param name="name">The Enum identifier of the timer to stop and remove.</param>
    /// <returns>True if the timer was successfully found and stopped; otherwise, false.</returns>
    public bool Stop(Enum name) => Stop(name.ToEnumString());

    /// <summary>
    /// Clears all timers managed by the TimeManager.
    /// </summary>
    public void Clear()
    {
        if (_timers.Count == 0)
            return;

        foreach (var timer in _timers.Keys)
            Stop(timer);
    }
}
