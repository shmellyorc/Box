using SFMLListener = SFML.Audio.Listener;

namespace Box.Sounds;

/// <summary>
/// Represents a sound channel used by the sound manager, allowing you to set up multiple channels for music, sound effects, ambient sounds, etc.
/// </summary>
/// <remarks>
/// Note: The SoundChannel ID of zero is reserved for the master channel. The master channel can adjust audio levels across all channels.
/// </remarks>
public sealed class SoundChannel
{
	// Keep track of sounds that are being used. It can contain multiple of the same sound
	// private readonly List<SoundInstance> _sounds = new(), _removedSounds = new(), _addSounds = new();
	// private readonly CoroutineHandle _handle;
	// private readonly bool _isBlocking;
	private readonly List<SoundInstance> _sounds = new();
	private float _volume = 1.0f, _pitch = 1.0f, _pan = 1.0f;

	/// <summary>
	/// The total number of currently playing audio instances.
	/// </summary>
	public int Count => _sounds.Count;

	/// <summary>
	/// This audio identifier. Id zero is reserved for the master channel.
	/// </summary>
	public int Id { get; } = -1;

	/// <summary>
	/// Indicates if this is the master channel.
	/// </summary>
	public bool IsMasterChannel => Id == 0;

	/// <summary>
	/// Adjusts the pitch of the channel audio to make it deeper or higher.
	/// </summary>
	/// <remarks>
	/// Acceptable values range from -2.0 to 2.0.
	/// </remarks>
	public float Pitch
	{
		get => _pitch;
		set
		{
			float oldValue = _pitch;
			_pitch = Math.Clamp(value, -2, 2f);

			if (_pitch != oldValue)
			{
				foreach (var sound in _sounds.ToList())
				{
					var item = sound;

					if (item.State != SoundState.Playing)
						continue;

					item.Pitch = _pitch;
				}

				Signal.Instance.Emit(EngineSignals.SoundChannelPitchChanged, this);
			}
		}
	}

	/// <summary>
	/// Gets or sets the pan of the audio channel, which controls the stereo balance of sounds played on this channel.
	/// </summary>
	/// <value>
	/// The pan value, ranging from -1.0 (full left) to 1.0 (full right), with 0.0 being the center (equal balance between left and right channels).
	/// </value>
	/// <remarks>
	/// When the pan value is set, it will update the pan of all currently playing sounds on the channel.
	/// A signal is emitted to notify other systems of the change in pan value.
	/// </remarks>
	public float Pan
	{
		get => _pan;
		set
		{
			float oldValue = _pan;
			_pan = Math.Clamp(value, -2, 2f);

			if (_pan != oldValue)
			{
				foreach (var sound in _sounds.ToList())
				{
					var item = sound;

					if (item.State != SoundState.Playing)
						continue;

					item.Pan = _pan;
				}

				Signal.Instance.Emit(EngineSignals.SoundChannelPanChanged, this);
			}
		}
	}


	/// <summary>
	/// Sets the audio level of the specified channel.
	/// </summary>
	/// <remarks>
	/// Note: This level is also influenced by the master volume setting.
	/// <para>Acceptable values range from 0.0 to 1.0.</para>
	/// </remarks>
	public float Volume
	{
		get => _volume;
		set
		{
			float oldVolume = _volume;
			_volume = Math.Clamp(value, 0f, 1f);

			if (_volume != oldVolume)
			{
				if (Id == 0)
					SFMLListener.GlobalVolume = Math.Clamp(_volume * 100f, 0f, 100f);
				else
				{
					for (int i = _sounds.Count - 1; i >= 0; i--)
					{
						var current = _sounds[i];

						if (!current.IsEmpty && current.State != SoundState.Stopped)
							current.Volume = _volume;
					}
				}


			}
		}
	}


	/// <summary>
	/// Gets a list of currently playing sound instances.
	/// </summary>
	public IEnumerable<SoundInstance> Sounds
	{
		get
		{
			if (_sounds.Count == 0)
				yield break;

			for (int i = 0; i < _sounds.Count; i++)
			{
				if (_sounds[i].State != SoundState.Playing)
					continue;

				yield return _sounds[i];
			}
		}
	}

	// internal SoundChannel(float volume, float pitch, bool isBlocking)
	internal SoundChannel(int id, float volume, float pitch, float pan)
	{
		Id = id;
		_volume = Math.Clamp(volume, 0f, 1f);
		_pitch = Math.Clamp(pitch, -2f, 2f);
		_pan = Math.Clamp(pan, -1f, 1f);
	}

	internal SoundInstance Add(Sound sound)
	{
		var master = SoundManager.Instance.Get(0);
		var instance = sound.CreateInstance();

		instance.Volume = Math.Clamp(_volume * master.Volume, 0f, 1f);
		instance.Pitch = Math.Clamp(_pitch, -2f, 2f);
		instance.Pan = Math.Clamp(_pan, -1f, 1f);
		instance.Play();

		_sounds.Add(instance);

		Signal.Instance.Emit(EngineSignals.SoundStarted, this, sound);

		return instance;
	}

	internal bool IsAlreadyPlaying(SoundInstance instance)
	{
		var s = _sounds.FirstOrDefault(x => x == instance);

		return !s.IsEmpty && s.State == SoundState.Playing || s.State == SoundState.Paused;
	}

	internal bool IsAlreadyPlaying(Sound sound)
	{
		var s = _sounds.FirstOrDefault(x => x.Sound == sound);

		return !s.IsEmpty && s.State == SoundState.Playing || s.State == SoundState.Paused;
	}

	internal bool Remove(SoundInstance instance)
	{
		if (_sounds.Count == 0)
			return false;

		var sounds = _sounds
			.Where(x => !x.IsDisposed && x == instance)
			.ToList();

		if (sounds.IsEmpty())
			return false;

		var check = new bool[sounds.Count];
		var result = new bool[sounds.Count];

		for (int i = 0; i < sounds.Count; i++)
		{
			var current = sounds[i];

			check[i] = true;
			result[i] = _sounds.Remove(current);

			current.Stop();
		}

		return result.SequenceEqual(check);
	}

	private bool _engineExit;

	internal void Clear(bool forceExit)
	{
		_engineExit = forceExit;

		if (_sounds.Count == 0)
			return;

		foreach (var sound in _sounds.ToList())
		{
			sound.Stop();
			_sounds.Remove(sound);
		}
	}

	internal void UpdateSounds()
	{
		if (_engineExit)
			return;

		for (int i = _sounds.Count - 1; i >= 0; i--)
		{
			if (_engineExit)
				break;

			if (_sounds.Count > 0 && i > _sounds.Count - 1)
			{
				while (i > _sounds.Count - 1)
					i--;
			}

			var current = _sounds[i];

			// We store sound effects to keep track of what is playing, but
			// we don't really care about the sound effects themselves. However, we do need to
			// clear sound effects from the channel sound cache.
			//
			// For music effects, we need to keep track of when they finish playing and 
			// repeat them. We don't use the "Play" function because it may reset the volumes, 
			// so we use the "Repeat" function to maintain the current audio state.
			if (current.State == SoundState.Stopped)
			{
				_sounds.Remove(current);
				i--;
			}
		}
	}
}
