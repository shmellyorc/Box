namespace Box.Sounds;

// Here is a complete list of all the supported audio formats:
// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam,
// w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64.3

/// <summary>
/// Represents a sound asset that implements the <see cref="IAsset"/> interface.
/// </summary>
public struct Sound : IAsset, IEquatable<Sound>
{
    internal readonly SFMLSoundBuffer Buffer;

	public uint Id { get; private set; }

    /// <summary>
    /// Gets the filename associated with this sound.
    /// </summary>
    public readonly string Filename { get; }

    /// <summary>
    /// Gets a value indicating whether the sound is set to loop when it reaches the end.
    /// </summary>
    /// <value>
    /// <c>true</c> if the sound will automatically restart after finishing; otherwise, <c>false</c>.
    /// </value>
    public readonly bool Looped { get; }

    /// <summary>
    /// Gets a value indicating whether the sound is considered empty.
    /// </summary>
    public readonly bool IsEmpty => Buffer is null || Filename.IsEmpty();

    /// <summary>
    /// Gets a value indicating whether the sound is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sound is initialized.
    /// </summary>
    public bool Initialized { get; internal set; }


    internal Sound(string filename, byte[] bytes, bool looped)
    {
        Filename = filename;
        Looped = looped;

        Buffer = new SFMLSoundBuffer(bytes);

		Id = HashHelpers.Hash32($"{Filename}{bytes.Length}");
	}

    /// <summary>
    /// Disposes of the sound, releasing any allocated resources.
    /// </summary>

    public void Dispose()
    {
        if (IsDisposed)
            return;

        Buffer?.Dispose();

        IsDisposed = true;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SoundInstance"/> associated with this sound.
    /// </summary>
    /// <returns>A new <see cref="SoundInstance"/> instance.</returns>
    public readonly SoundInstance CreateInstance()
    {
        return new SoundInstance(this);
    }

    /// <summary>
    /// Determines whether two Sound instances are equal.
    /// </summary>
    /// <param name="left">The left-hand side Sound instance.</param>
    /// <param name="right">The right-hand side Sound instance.</param>
    /// <returns>True if the instances are equal, false otherwise.</returns>
    public static bool operator ==([NotNullWhen(true)] Sound left, [NotNullWhen(true)] Sound right)
    {
        return left.Filename == right.Filename;
    }

    /// <summary>
    /// Determines whether two Sound instances are not equal.
    /// </summary>
    /// <param name="left">The left-hand side Sound instance.</param>
    /// <param name="right">The right-hand side Sound instance.</param>
    /// <returns>True if the instances are not equal, false otherwise.</returns>
    public static bool operator !=([NotNullWhen(true)] Sound left, [NotNullWhen(true)] Sound right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Checks whether this Sound instance is equal to another Sound instance.
    /// </summary>
    /// <param name="other">The Sound instance to compare with.</param>
    /// <returns>True if the instances are equal, false otherwise.</returns>
    public readonly bool Equals([NotNullWhen(true)] Sound other)
    {
        return Filename == other.Filename;
    }

    /// <summary>
    /// Determines whether this Sound instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with this Sound instance.</param>
    /// <returns>True if the object is a Sound instance and equal to this instance, false otherwise.</returns>
    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is Sound value)
            return Equals(value);

        return false;
    }

    /// <summary>
    /// Returns the hash code for this Sound instance.
    /// </summary>
    /// <returns>The hash code value.</returns>
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(Filename);
    }

    /// <summary>
    /// Initializes the Sound instance.
    /// </summary>
    public readonly void Initialize() { }
}
