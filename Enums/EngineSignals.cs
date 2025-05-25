namespace Box.Enums;

/// <summary>
/// Enumeration of signals used within the engine for communication or event triggering.
/// </summary>
public enum EngineSignals
{
    /// <summary>
    /// 0: Vect2 (New size state)
    /// </summary>    
    WindowSizeChanged,

    /// <summary>
    /// 0: Screen
    /// </summary>
    ScreenDirty,

    /// <summary>
    /// 0: Bool (New fullscreen state)
    /// </summary>
    WindowFullscreenChanged,

    /// <summary>
    /// 0: Entity,
    /// 1: Parent
    /// </summary>
    EntityAdded,

    /// <summary>
    /// 0: Entity,
    /// 1: Parent
    /// </summary>
    EntityRemoved,

    /// <summary>
    /// 0: AudioChannel
    /// </summary>
    SoundChannelVolumeChanged,

    /// <summary>
    /// 0: AudioChannel
    /// </summary>
    SoundChannelPitchChanged,

    /// <summary>
    /// 0: AniatedSprite
    /// 1: Animation
    /// </summary>
    AnimatedSpriteStarted,

    /// <summary>
    /// 0: AniatedSprite
    /// 1: Animation
    /// </summary>
    AnimatedSpriteFinished,

    /// <summary>
    /// 0: Listview
    /// </summary>
    ListviewSoundFx,

    /// <summary>
    /// 0: Listview
    /// 1: ListviewItem
    /// 2: int (SelectedIndex)
    /// </summary>
    ListviewSelected,

    /// <summary>
    /// 0: Entity
    /// 1: String (new name)
    /// </summary>
    EntityNameChanged,

    /// <summary>
    /// 0: SoundChannel
    /// 1: Sound
    /// </summary>
    SoundStarted,

    /// <summary>
    /// 0: SoundChannel
    /// 1: Sound
    /// </summary>
    SoundRemoved,

    /// <summary>
    /// 0: AudioChannel
    /// </summary>
    SoundChannelPanChanged,
}
