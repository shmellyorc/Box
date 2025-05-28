using Box.Services.Types;

namespace Box.Systems;

/// <summary>
/// Represents a clock used to measure delta time and real elapsed time.
/// </summary>
public sealed class Clock
{
	private readonly SFMLClock _clock;
	private SFMLTime _time;

	public static Clock Instance { get; private set; }

	/// <summary>
	/// DeltaTime displayed in Milliseconds.
	/// </summary>
	public int DeltaTimeAsMilliseconds => _time.AsMilliseconds();

	/// <summary>
	/// DeltaTime displayed in Microseconds.
	/// </summary>
	public long DeltaTimeAsMicroseconds => _time.AsMicroseconds();

	/// <summary>
	/// Delta time displayed in seconds, clamped to a maximum of 1/30th of a second.
	/// </summary>
	public float DeltaTime => MathF.Min(DeltaTimeRaw, 1f / 30f);

	/// <summary>
	/// Raw delta time displayed in seconds (no clamping).
	/// </summary>
	public float DeltaTimeRaw => _time.AsSeconds();

	/// <summary>
	/// Total unscaled, real-world time since the clock was created, in seconds.
	/// </summary>
	public double RealTime { get; private set; }

	internal Clock()
	{
		Instance ??= this;

		_clock = new SFMLClock();
		_time = _clock.Restart();
		RealTime = 0.0;
	}

	/// <summary>
	/// Updates the clock, resetting the time and accumulating real time.
	/// </summary>
	internal void Update()
	{
		_time = _clock.Restart();
		RealTime += _time.AsSeconds();
	}

	/// <summary>
	/// Converts seconds to frames per second.
	/// </summary>
	/// <param name="seconds">The number of seconds to convert to frames per second.</param>
	/// <returns>The equivalent frames per second value.</returns>
	public static float ToFps(float seconds) => 1f / seconds;
}
