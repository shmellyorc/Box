namespace Box.Entities.Triggers;

/// <summary>
/// Triggers an action after a specified amount of time has passed. Can be configured to trigger once or repeatedly.
/// </summary>
public class TimerTrigger : Entity
{
	private readonly float _time;
	private readonly Action _callback;
	private readonly bool _oneShot;
	private float _elapsed;

	/// <summary>
	/// Gets the total duration of the timer in seconds.
	/// </summary>
	public float Time => _time;

	/// <summary>
	/// Gets the time elapsed since the timer started.
	/// </summary>
	public float Elapsed => _elapsed;

	/// <summary>
	/// Indicates whether the timer is currently running.
	/// </summary>
	public bool IsRunning { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TimerTrigger"/> class.
	/// </summary>
	/// <param name="time">The time in seconds after which the trigger will activate.</param>
	/// <param name="onTriggered">The action to invoke when the timer completes.</param>
	/// <param name="oneShot">Whether the trigger should fire only once.</param>
	/// <param name="autoStart">Whether the timer should start immediately upon creation.</param>
	public TimerTrigger(float time, Action onTriggered, bool oneShot = true, bool autoStart = false)
	{
		_time = Math.Max(0, time);
		_callback = onTriggered;
		_oneShot = oneShot;

		if (autoStart)
			Start();
	}

	/// <summary>
	/// Starts or restarts the timer from zero.
	/// Useful for triggering the timer manually or restarting after a stop.
	/// </summary>
	/// <returns>Returns the current <see cref="TimerTrigger"/> instance for chaining.</returns>
	public TimerTrigger Start()
	{
		_elapsed = 0f;
		IsRunning = true;

		return this;
	}

	/// <summary>
	/// Stops the timer, preventing it from triggering until restarted.
	/// </summary>
	/// <returns>Returns the current <see cref="TimerTrigger"/> instance for chaining.</returns>
	public TimerTrigger Stop()
	{
		IsRunning = false;

		return this;
	}

	/// <summary>
	/// Stops and immediately restarts the timer from zero.
	/// This is a shortcut for calling <see cref="Stop"/> followed by <see cref="Start"/>.
	/// </summary>
	/// <returns>Returns the current <see cref="TimerTrigger"/> instance for chaining.</returns>
	public TimerTrigger Restart()
	{
		Stop();
		Start();

		return this;
	}

	/// <summary>
	/// Updates the timer. Should be called each frame.
	/// </summary>
	protected override void Update()
	{
		if (!IsRunning)
			return;

		_elapsed += Clock.DeltaTime;

		if (_elapsed >= _time)
		{
			_callback?.Invoke();

			if (_oneShot)
				Stop();
			else
				_elapsed = 0f; // reset for repeating
		}

		base.Update();
	}
}
