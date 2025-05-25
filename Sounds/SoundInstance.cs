
namespace Box.Sounds;

/// <summary>
/// Represents an instance of a sound playing or managed within an application.
/// </summary>
public struct SoundInstance : IEquatable<SoundInstance>
{
    private readonly StringBuilder _sb = new();
    private readonly SFMLSoundInstance _instance;
    private float _volume = 1.0f, _pitch = 1f, _pan = 0f;

    /// <summary>
    /// 
    /// </summary>
    public readonly Sound Sound { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the current state of the sound instance.
    /// </summary>
    public readonly SoundState State => !IsEmpty ? (SoundState)_instance?.Status : SoundState.Stopped;

    /// <summary>
    /// Gets the total duration of the sound.
    /// </summary>
    public readonly TimeSpan Length => Sound.Buffer.Duration.ToTimeSpan();

    /// <summary>
    /// Gets a value indicating whether the sound instance is set to loop when it reaches the end.
    /// </summary>
    /// <value>
    /// <c>true</c> if the sound instance will automatically restart after finishing; otherwise, <c>false</c>.
    /// </value>
    public readonly bool Looped => Sound.Looped;

    /// <summary>
    /// Gets the current playback position of the sound.
    /// </summary>
    public readonly TimeSpan Position => !IsEmpty && _instance?.Status == SFMLSoundStatus.Playing
        ? _instance.PlayingOffset.ToTimeSpan() : TimeSpan.Zero;

    /// <summary>
    /// Gets a value indicating whether the sound instance is empty or uninitialized.
    /// </summary>
    public readonly bool IsEmpty => Sound.IsEmpty || Sound.Buffer is null || _instance is null || _instance.CPointer == 0x0000000000000000;

    /// <summary>
    /// Gets or sets the volume of the sound instance.
    /// </summary>
    public float Volume
    {
        readonly get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0f, 1f);

            if (!IsEmpty && _instance.Status == SFMLSoundStatus.Playing)
                _instance.Volume = Math.Clamp(_volume * 100f, 0f, 100f);
        }
    }

    /// <summary>
    /// Gets or sets the pitch of the sound instance.
    /// </summary>
    public float Pitch
    {
        readonly get => _pitch;
        set
        {
            _pitch = Math.Clamp(value, -2f, 2f);

            if (!IsEmpty && _instance.Status == SFMLSoundStatus.Playing)
                _instance.Pitch = Math.Clamp(_pitch, -2f, 2f);
        }
    }

    /// <summary>
    /// Gets or sets the stereo pan of the sound, determining its left-right positioning.
    /// </summary>
    /// <value>
    /// A float value ranging from -1.0 (full left) to 1.0 (full right). A value of 0.0 represents the center.
    /// </value>
    public float Pan
    {
        readonly get => _pan;
        set
        {
            _pan = Math.Clamp(value, -1f, 1f);

            if (!IsEmpty && _instance.Status == SFMLSoundStatus.Playing)
                _instance.Position = new SFMLVector3f(_pan, 0f, 1f);
        }
    }

    internal SoundInstance(Sound sound)
    {
        Sound = sound;
        IsDisposed = false;

        _instance = new SFMLSoundInstance(Sound.Buffer)
        {
            // Always set volume to zero at the start so it doesnt blerp 
            // to max volume than to sound manager volume
            Volume = 0
        };
    }

    /// <summary>
    /// Plays the sound instance from its current position.
    /// </summary>
    public readonly void Play()
    {
        if (_instance.Status != SFMLSoundStatus.Stopped)
            return;

        _instance.Volume = Math.Clamp(_volume * 100f, 0f, 100f);
        _instance.Pitch = Math.Clamp(_pitch, -2f, 2f);
        _instance.Position = new SFMLVector3f(Math.Clamp(_pan, -1f, 1f), 0f, 1f);
        _instance.Loop = Looped;

        _instance.Play();
    }

    /// <summary>
    /// Stops the sound instance playback.
    /// </summary>
    public void Stop()
    {
        // if (_instance.Status != SFMLSoundStatus.Playing)
        //     return;

        _instance?.Stop();
        _instance?.Dispose();

        IsDisposed = true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="SoundInstance"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="SoundInstance"/> to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="SoundInstance"/> is equal to the current instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Equals(SoundInstance other)
        => (Sound.Filename, State, Position, Length) == (other.Sound.Filename, other.State, other.Position, other.Length);

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified object is a <see cref="SoundInstance"/> and is equal to the current instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is SoundInstance value)
            return Equals(value);

        return false;
    }

    /// <summary>
    /// Serves as a hash function for a <see cref="SoundInstance"/> object.
    /// </summary>
    /// <returns>A hash code for the current <see cref="SoundInstance"/>.</returns>
    public readonly override int GetHashCode() => HashCode.Combine(Sound.Filename, State, Position, Length);

    /// <summary>
    /// Returns a string that represents the current <see cref="SoundInstance"/>.
    /// </summary>
    /// <returns>A string representing the current <see cref="SoundInstance"/> with its sound filename, state, position, and length.</returns>
    public readonly override string ToString()
    {
        if (_sb.Length > 0)
            _sb.Clear();

        _sb.Append($"{Sound.Filename}, ");
        _sb.Append($"{State}, ");
        _sb.Append($"{Position}");
        _sb.Append($"{Length}, ");

        return _sb.ToString();
    }

    /// <summary>
    /// Determines whether two specified <see cref="SoundInstance"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="SoundInstance"/> to compare.</param>
    /// <param name="right">The second <see cref="SoundInstance"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="SoundInstance"/> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(SoundInstance left, SoundInstance right)
        => (left.Sound.Filename, left.State, left.Position, left.Length) ==
            (right.Sound.Filename, right.State, right.Position, right.Length);

    /// <summary>
    /// Determines whether two specified <see cref="SoundInstance"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="SoundInstance"/> to compare.</param>
    /// <param name="right">The second <see cref="SoundInstance"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="SoundInstance"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(SoundInstance left, SoundInstance right) => !(left == right);
}
