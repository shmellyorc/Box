using Box.Services.Types;

namespace Box.Systems;

/// <summary>
/// Represents a clock used to measure delta time.
/// </summary>
public sealed class Clock : UpdatableService
{
	private readonly SFMLClock _clock;
	private SFMLTime _time;

	/// <summary>
	/// DeltaTime displayed in Milliseconds.
	/// </summary>
	public int DeltaTimeAsMilliseconds => _time.AsMilliseconds();

	/// <summary>
	/// DeltaTime displayed in Microseconds.
	/// </summary>
	public long DeltaTimeAsMicroseconds => _time.AsMicroseconds();

	/// <summary>
	/// DeltaTime displayed in Seconds.
	/// </summary>
	public float DeltaTime => MathF.Min(DeltaTimeRaw, 1f / 30f);

	/// <summary>
	/// DeltaTime displayed in Seconds.
	/// </summary>
	public float DeltaTimeRaw => _time.AsSeconds();

	internal Clock()
	{
		_clock = new SFMLClock();
		_time = _clock.Restart();
	}

	/// <summary>
	/// Updates the clock, resetting the time and starting a new measurement cycle.
	/// </summary>
	/// <remarks>
	/// This method is called every frame to update the clock's time. It uses the underlying timer (e.g., <see cref="_clock"/>)
	/// to restart and capture the current time, which can be used for time-based operations within the engine.
	/// </remarks>
	public override void Update() => _time = _clock.Restart();

	/// <summary>
	/// Converts seconds to frames per second.
	/// </summary>
	/// <param name="seconds">The number of seconds to convert to frames per second.</param>
	/// <returns>The equivalent number of frames per second based on the input seconds.</returns>
	public static float ToFps(float seconds) => 1f / seconds;
}
