using Box.Services.Types;

namespace Box.Sounds;

/// <summary>
/// The sound manager handles all sound channels, allowing you to manage music, sound effects, and other audio effects.
/// </summary>
public sealed class SoundManager
{
	#region Fields
	private readonly Dictionary<int, SoundChannel> _channels = new();
	#endregion


	#region Properties
	/// <summary>
	/// Singleton instance of the sound manager.
	/// </summary>
	public static SoundManager Instance { get; private set; }

	/// <summary>
	/// Total number of sound channels managed by the sound manager.
	/// </summary>
	public int ChannelCount => _channels.Sum(x => x.Value.Count);

	/// <summary>
	/// Total count of currently playing sound instances across all channels.
	/// </summary>
	public int PlayCount => _channels.Sum(x => x.Value.Count);
	#endregion


	#region Constructor

	internal SoundManager()
	{
		Instance ??= this;

		// Add master channel:
		_channels.Add(0, new SoundChannel(0, 1f, 1f, 1f));
	}
	#endregion


	#region Add
	/// <summary>
	/// Adds a new audio channel for music, sound effects, or other types as needed.
	/// </summary>
	/// <param name="id">The ID of the channel.</param>
	/// <param name="volume">The initial volume of the channel, ranging from 0.0 to 1.0.</param>
	/// <param name="pitch">The initial pitch of the channel, ranging from -2.0 to 2.0.</param>
	/// <param name="pan">The initial stereo pan of the channel, ranging from -1.0 (full left) to 1.0 (full right), with 0.0 being centered.</param>
	/// <exception cref="ArgumentException">Thrown if attempting to add to channel zero, which is reserved for the master channel.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the channel ID already exists.</exception>
	public void Add(int id, float volume = 1.0f, float pitch = 1.0f, float pan = 1.0f)
	{
		if (id == 0)
			throw new ArgumentException($"Cannot use ID '{id}', as it is reserved for the Master Audio Channel.", nameof(id));

		if (Exists(id))
			throw new InvalidOperationException($"A sound channel with ID '{id}' already exists.");

		_channels.Add(id, new SoundChannel(id, volume, pitch, pan));
	}

	/// <summary>
	/// Adds a new audio channel for music, sound effects, or other types as needed.
	/// </summary>
	/// <param name="id">The ID of the channel.</param>
	/// <param name="volume">The initial volume of the channel, ranging from 0.0 to 1.0.</param>
	/// <param name="pitch">The initial pitch of the channel, ranging from -2.0 to 2.0.</param>
	/// <param name="pan">The initial stereo pan of the channel, ranging from -1.0 (full left) to 1.0 (full right), with 0.0 being centered.</param>
	/// <exception cref="ArgumentException">Thrown if attempting to add to channel zero, which is reserved for the master channel.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the channel ID already exists.</exception>
	public void Add(Enum id, float volume = 1.0f, float pitch = 1.0f, float pan = 1.0f) => Add(Convert.ToInt32(id), volume, pitch, pan);
	#endregion


	#region Remove
	/// <summary>
	/// Removes an audio channel that was previously added.
	/// </summary>
	/// <param name="id">The unique ID of the channel to remove.</param>
	/// <returns>
	/// <c>true</c> if the sound channel was successfully removed; <c>false</c> otherwise. 
	/// Removal will fail automatically if attempting to remove the master channel (ID zero) or if the channel does not exist.
	/// </returns>
	/// <remarks>
	/// This method attempts to remove the specified audio channel. The removal will fail if the specified ID is 0 (master channel), 
	/// or if the channel does not exist in the collection. If successful, it also clears any associated data for the channel.
	/// </remarks>
	public bool Remove(int id)
	{
		if (id == 0)
			return false;
		if (!Exists(id))
			return false;

		if (_channels.Remove(id))
		{
			_channels[id].Clear(false);

			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes an audio channel that was previously added.
	/// </summary>
	/// <param name="id">The unique ID of the channel to remove.</param>
	/// <returns>
	/// <c>true</c> if the sound channel was successfully removed; <c>false</c> otherwise. 
	/// Removal will fail automatically if attempting to remove the master channel (ID zero) or if the channel does not exist.
	/// </returns>
	/// <remarks>
	/// This method attempts to remove the specified audio channel. The removal will fail if the specified ID is 0 (master channel), 
	/// or if the channel does not exist in the collection. If successful, it also clears any associated data for the channel.
	/// </remarks>
	public bool Remove(Enum id) => Remove(Convert.ToInt32(id));
	#endregion


	#region Clear
	/// <summary>
	/// Removes all sound channels except the master channel, along with any playing sound instances on those channels.
	/// </summary>
	public void Clear()
	{
		if (_channels.Count == 0)
			return;

		foreach (var channel in _channels)
		{
			if (channel.Value.IsMasterChannel)
				continue;

			Remove(channel.Key);
		}
	}
	#endregion


	#region Exists
	/// <summary>
	/// Checks if the specified channel exists.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <returns>
	/// <c>true</c> if the channel exists; <c>false</c> otherwise.
	/// </returns>
	/// <remarks>
	/// This method checks whether a channel with the specified ID is present in the collection of channels. 
	/// If the channel exists, it returns <c>true</c>; otherwise, it returns <c>false</c>.
	/// </remarks>
	public bool Exists(int id) => _channels.ContainsKey(id);

	/// <summary>
	/// Checks if the specified channel exists.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <returns>
	/// <c>true</c> if the channel exists; <c>false</c> otherwise.
	/// </returns>
	/// <remarks>
	/// This method checks whether a channel with the specified ID is present in the collection of channels. 
	/// If the channel exists, it returns <c>true</c>; otherwise, it returns <c>false</c>.
	/// </remarks>
	public bool Exists(Enum id) => Exists(Convert.ToInt32(id));
	#endregion


	#region Get
	/// <summary>
	/// Retrieves the sound channel based on the channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to retrieve.</param>
	/// <returns>
	/// The sound channel object if the channel ID exists; otherwise, returns <c>null</c>.
	/// <remarks>
	/// If the channel ID does not exist, <c>null</c> will be returned, indicating that the specified channel was not found.
	/// </remarks>
	/// </returns>
	public SoundChannel Get(int id)
	{
		if (!Exists(id))
			return null;

		return _channels[id];
	}

	/// <summary>
	/// Retrieves the sound channel based on the channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to retrieve.</param>
	/// <returns>
	/// The sound channel object if the channel ID exists; otherwise, returns <c>null</c>.
	/// <remarks>
	/// If the channel ID does not exist, <c>null</c> will be returned, indicating that the specified channel was not found.
	/// </remarks>
	/// </returns>
	public SoundChannel Get(Enum id) => Get(Convert.ToInt32(id));

	/// <summary>
	/// Tries to retrieve the sound channel based on the channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to retrieve.</param>
	/// <param name="channel">Outputs the sound channel if found; otherwise, null.</param>
	/// <returns>
	/// <c>true</c> if the channel was found and retrieved successfully; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks>
	/// This method attempts to get the sound channel with the specified ID. If the channel exists, it is assigned to the 
	/// <paramref name="channel"/> output parameter, and the method returns <c>true</c>. If the channel does not exist, 
	/// <paramref name="channel"/> will be set to <c>null</c>, and the method returns <c>false</c>.
	/// </remarks>
	public bool TryGet(int id, out SoundChannel channel)
	{
		channel = Get(id);

		return channel is not null;
	}

	/// <summary>
	/// Tries to retrieve the sound channel based on the channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to retrieve.</param>
	/// <param name="channel">Outputs the sound channel if found; otherwise, null.</param>
	/// <returns>
	/// <c>true</c> if the channel was found and retrieved successfully; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks>
	/// This method attempts to get the sound channel with the specified ID. If the channel exists, it is assigned to the 
	/// <paramref name="channel"/> output parameter, and the method returns <c>true</c>. If the channel does not exist, 
	/// <paramref name="channel"/> will be set to <c>null</c>, and the method returns <c>false</c>.
	/// </remarks>
	public bool TryGet(Enum id, out SoundChannel channel) => TryGet(Convert.ToInt32(id), out channel);
	#endregion


	#region Play
	/// <summary>
	/// Plays a sound instantly without assigning it to a sound channel.
	/// </summary>
	/// <param name="sound">The sound to play.</param>
	/// <param name="volume">The volume level of the sound (0.0 to 1.0).</param>
	/// <param name="pitch">The pitch adjustment of the sound (e.g., 1.0 for normal pitch).</param>
	/// <param name="pan">The stereo pan of the sound (-1.0 for full left, 1.0 for full right, 0.0 for center).</param>
	/// <returns>A <see cref="SoundInstance"/> representing the playing sound.</returns>
	/// <exception cref="Exception">Thrown if the provided sound is empty.</exception>
	public SoundInstance Play(Sound sound, float volume, float pitch, float pan)
	{
		if (sound.IsEmpty)
			throw new ArgumentException("Cannot play an empty sound.", nameof(sound));

		var instance = sound.CreateInstance();

		instance.Volume = volume;
		instance.Pitch = pitch;
		instance.Pan = pan;

		instance.Play();

		return instance;
	}

	/// <summary>
	/// Plays a sound on the specified sound channel.
	/// </summary>
	/// <param name="id">The ID of the sound channel to play the sound on.</param>
	/// <param name="sound">The sound to play.</param>
	/// <returns>A <see cref="SoundInstance"/> representing the playing sound.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the channel ID is 0, as it is invalid.</exception>
	/// <exception cref="KeyNotFoundException">Thrown if the specified channel ID does not exist.</exception>
	public SoundInstance Play(int id, Sound sound)
	{
		if (id == 0)
			throw new ArgumentOutOfRangeException(nameof(id), "Channel ID 0 is reserved for the master channel and cannot be used.");

		if (!Exists(id))
			throw new KeyNotFoundException($"Sound channel with ID '{id}' does not exist.");

		return _channels[id].Add(sound);
	}

	/// <summary>
	/// Plays a sound on the specified sound channel.
	/// </summary>
	/// <param name="id">The ID of the sound channel to play the sound on.</param>
	/// <param name="sound">The sound to play.</param>
	/// <returns>A <see cref="SoundInstance"/> representing the playing sound.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the channel ID is 0, as it is invalid.</exception>
	/// <exception cref="KeyNotFoundException">Thrown if the specified channel ID does not exist.</exception>
	public SoundInstance Play(Enum id, Sound sound) => Play(Convert.ToInt32(id), sound);
	#endregion


	#region Stop
	/// <summary>
	/// Stops a specific sound on the identified channel.
	/// </summary>
	/// <param name="id">The unique ID of the channel where the sound will be stopped.</param>
	/// <param name="sound">The sound instance to be stopped.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), if the channel doesn't exist, if no sounds are playing on the channel, or if the specified sound is not found on the channel.
	/// <c>true</c> if the specified sound was successfully stopped on the identified channel.
	/// </returns>
	public bool Stop(int id, SoundInstance sound)
	{
		if (id == 0)
			return false;
		if (!Exists(id))
			return false;
		if (_channels[id].Count == 0)
			return false;

		_channels[id].Remove(sound);

		return true;
	}

	/// <summary>
	/// Stops a specific sound on the identified channel.
	/// </summary>
	/// <param name="id">The unique ID of the channel where the sound will be stopped.</param>
	/// <param name="sound">The sound instance to be stopped.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), if the channel doesn't exist, if no sounds are playing on the channel, or if the specified sound is not found on the channel.
	/// <c>true</c> if the specified sound was successfully stopped on the identified channel.
	/// </returns>
	public void Stop(Enum id, SoundInstance sound) => Stop(Convert.ToInt32(id), sound);

	/// <summary>
	/// Stops all audio playback on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel where all sounds will be stopped.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), if the channel doesn't exist, or if there are no sounds playing on the channel.
	/// <c>true</c> if the audio playback on the specified channel was successfully stopped.
	/// </returns>
	public bool Stop(int id)
	{
		if (id == 0)
			return false;
		if (!Exists(id))
			return false;
		if (_channels[id].Count == 0)
			return false;

		_channels[id].Clear(false);

		return true;
	}

	/// <summary>
	/// Stops all audio playback on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel where all sounds will be stopped.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), if the channel doesn't exist, or if there are no sounds playing on the channel.
	/// <c>true</c> if the audio playback on the specified channel was successfully stopped.
	/// </returns>
	public void Stop(Enum id) => Stop(Convert.ToInt32(id));

	/// <summary>
	/// Will stop all audio on all channels.
	/// </summary>
	public void StopAll()
	{
		foreach (var channel in _channels)
			channel.Value.Clear(false);
	}
	#endregion


	#region IsAlreadyPlaying
	/// <summary>
	/// Checks if a specific sound is currently playing on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <param name="sound">The sound to check for on the specified channel.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), or if the channel doesn't exist.
	/// <c>true</c> if the sound is currently playing on the specified channel ID; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAlreadyPlaying(int id, SoundInstance sound)
	{
		if (id == 0)
			return false;
		if (!Instance.TryGet(id, out var channel))
			return false;
		if (sound.IsEmpty)
			return false;

		return channel.IsAlreadyPlaying(sound);
	}

	/// <summary>
	/// Checks if a specific sound is currently playing on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <param name="sound">The sound to check for on the specified channel.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), or if the channel doesn't exist.
	/// <c>true</c> if the sound is currently playing on the specified channel ID; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAlreadyPlaying(Enum id, SoundInstance sound) => IsAlreadyPlaying(Convert.ToInt32(id), sound);

	/// <summary>
	/// Checks if a specific sound is currently playing on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <param name="sound">The sound to check for on the specified channel.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), or if the channel doesn't exist.
	/// <c>true</c> if the sound is currently playing on the specified channel ID; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAlreadyPlaying(int id, Sound sound)
	{
		if (id == 0)
			return false;
		if (!Instance.TryGet(id, out var channel))
			return false;
		if (sound.IsEmpty)
			return false;

		return channel.IsAlreadyPlaying(sound);
	}

	/// <summary>
	/// Checks if a specific sound is currently playing on the specified channel ID.
	/// </summary>
	/// <param name="id">The unique ID of the channel to check.</param>
	/// <param name="sound">The sound to check for on the specified channel.</param>
	/// <returns>
	/// <c>false</c> if the channel ID is 0 (master channel), or if the channel doesn't exist.
	/// <c>true</c> if the sound is currently playing on the specified channel ID; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAlreadyPlaying(Enum id, Sound sound) => IsAlreadyPlaying(Convert.ToInt32(id), sound);
	#endregion


	#region Engine
	private bool _engineExit;

	internal void EngineClear()
	{
		_engineExit = true;

		foreach (var channel in _channels)
		{
			channel.Value.Clear(true);
		}

		_channels.Clear();
	}

	/// <summary>
	/// Updates the sound manager, processing all active sounds and their states.
	/// </summary>
	/// <remarks>
	/// This method is called every frame to update the state of active sounds. It handles tasks like updating sound effects,
	/// checking for completion, and managing the playback state of sounds within the engine.
	/// </remarks>
	internal void Update()
	{
		if (_engineExit)
			return;
		if (_channels.Count == 1)
			return;

		foreach (var channel in new Dictionary<int, SoundChannel>(_channels))
		{
			if (_engineExit)
				break;
			if (channel.Value.IsMasterChannel)
				continue;
			if (channel.Value.Count == 0)
				continue;

			channel.Value.UpdateSounds();
		}
	}
	#endregion
}
