namespace System;

/// <summary>
/// Provides extension methods for retrieving the volume of a sound channel.
/// </summary>
public static class SoundExtentions
{
    /// <summary>
    /// Gets the volume of the specified sound channel using an <see cref="Enum"/> identifier.
    /// </summary>
    /// <param name="channel">The enumerated sound channel.</param>
    /// <returns>The volume level of the specified sound channel, ranging from 0.0 to 1.0.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified channel does not exist.</exception>
    public static float GetSoundVolume(this Enum channel) => SoundManager.Instance.Get(channel).Volume;

    /// <summary>
    /// Gets the volume of the specified sound channel using an integer identifier.
    /// </summary>
    /// <param name="channel">The ID of the sound channel.</param>
    /// <returns>The volume level of the specified sound channel, ranging from 0.0 to 1.0.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified channel does not exist.</exception>
    public static float GetSoundVolume(this int channel) => SoundManager.Instance.Get(channel).Volume;


    /// <summary>
    /// Sets the volume for a specific sound channel based on its enum value.
    /// </summary>
    /// <param name="channel">An enum value representing the sound channel whose volume you want to modify.</param>
    /// <param name="volume">A float value representing the desired volume level (usually between 0.0f and 1.0f).</param>
    /// <returns>The volume value after being set on the specified sound channel.</returns>
    public static float SetSoundVolume(this Enum channel, float volume) => SoundManager.Instance.Get(channel).Volume = volume;

    /// <summary>
    /// Sets the volume for a specific sound channel based on an integer channel identifier.
    /// </summary>
    /// <param name="channel">An integer representing the sound channel's identifier.</param>
    /// <param name="volume">A float value representing the desired volume level (usually between 0.0f and 1.0f).</param>
    /// <returns>The volume value after being set on the specified sound channel.</returns>
    public static float SetSoundVolume(this int channel, float volume) => SoundManager.Instance.Get(channel).Volume = volume;
}
