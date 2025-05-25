namespace Box.Entities.Graphics;

/// <summary>
/// Struct representing an animation.
/// </summary>
public readonly struct Animation
{
	/// <summary>
	/// Gets the name of the animation.
	/// </summary>
	public readonly string Name { get; }

	/// <summary>
	/// Gets the surface associated with the animation.
	/// </summary>
	public readonly Surface Surface { get; }

	/// <summary>
	/// Gets the frames of the animation represented as an array of rectangles.
	/// </summary>
	public readonly Rect2[] Frames { get; }

	/// <summary>
	/// Gets the speed of the animation.
	/// </summary>
	public readonly float Speed { get; }

	/// <summary>
	/// Gets a value indicating whether the animation is looped.
	/// </summary>
	public readonly bool Looped { get; }

	/// <summary>
	/// Gets a value indicating whether the animation is considered empty.
	/// An animation is considered empty if it has no frames or its speed is zero.
	/// </summary>
	public readonly bool IsEmpty => Frames.IsEmpty() || Speed == 0f;

	internal Animation(string name, Surface surface, Rect2[] frames, float speed, bool looped)
	{
		Name = name;
		Surface = surface;
		Frames = frames;
		Speed = speed;
		Looped = looped;
	}

	/// <summary>
	/// Converts the animation to its string representation.
	/// </summary>
	/// <returns>A string that represents the current animation.</returns>
	public override string ToString() => $"{Name} ({Frames} frames)";
}

/// <summary>
/// Represents an animated sprite entity.
/// </summary>
public class AnimatedSprite : Entity
{
	private bool _sizeChanged;
	private int _frame, _frameIndex;
	private bool _animationFinished;
	private readonly Dictionary<string, Animation> _animations = new();
	private Animation _animation;
	private CoroutineHandle _routine;
	private string _name;

	/// <summary>
	/// Represents the color of the animated sprite.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Represents the vertical alignment of the sprite within its container.
	/// </summary>
	public VAlign VAlign = VAlign.Top;

	/// <summary>
	/// Represents the horizontal alignment of the sprite within its container.
	/// </summary>
	public HAlign HAlign = HAlign.Left;

	/// <summary>
	/// Represents the surface effects applied to the animated sprite.
	/// </summary>
	public SurfaceEffects Effect = SurfaceEffects.None;

	/// <summary>
	/// Gets the current state of the animation.
	/// </summary>
	public AnimationState State { get; private set; }

	/// <summary>
	/// Gets a value indicating whether the animation is currently playing.
	/// </summary>
	public bool IsPlaying => State == AnimationState.Playing;

	/// <summary>
	/// Gets a value indicating whether the animation is currently stopped.
	/// </summary>
	public bool IsStopped => State == AnimationState.Stopped;

	/// <summary>
	/// Represents the size of the sprite.
	/// </summary>
	public new Vect2 Size
	{
		get => base.Size;
		set
		{
			var oldValue = base.Size;
			base.Size = value;

			if (base.Size != oldValue)
			{
				_isDirty = true;

				foreach (var parent in GetParents<Entity>())
					parent._isDirty = true;

				if (!_sizeChanged)
					_sizeChanged = true;
			}
		}
	}


	#region Add
	/// <summary>
	/// Adds a new animation to the animated sprite.
	/// </summary>
	/// <param name="name">The name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="frames">An array of frame indices representing the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(string name, Surface surface, Vect2 cellSize, int[] frames, float speed, bool looped)
	{
		EngineAdd(name, surface, cellSize, frames, speed, looped);

		return this;
	}

	/// <summary>
	/// Adds a new animation to the animated sprite using an enum value as the name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="frames">An array of frame indices representing the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(Enum name, Surface surface, Vect2 cellSize, int[] frames, float speed, bool looped)
	{
		EngineAdd(name.ToEnumString(), surface, cellSize, frames, speed, looped);

		return this;
	}

	/// <summary>
	/// Adds a new animation to the animated sprite using frame indices range.
	/// </summary>
	/// <param name="name">The name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="start">The starting frame index of the animation sequence.</param>
	/// <param name="end">The ending frame index of the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(string name, Surface surface, Vect2 cellSize, int start, int end, float speed, bool looped)
	{
		EngineAdd(name, surface, cellSize, start, end, speed, looped);

		return this;
	}

	/// <summary>
	/// Adds a new animation to the animated sprite using an enum value as the name and frame indices range.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="start">The starting frame index of the animation sequence.</param>
	/// <param name="end">The ending frame index of the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(Enum name, Surface surface, Vect2 cellSize, int start, int end, float speed, bool looped)
	{
		EngineAdd(name.ToEnumString(), surface, cellSize, start, end, speed, looped);

		return this;
	}

	/// <summary>
	/// Adds a new animation to the animated sprite using a single frame index.
	/// </summary>
	/// <param name="name">The name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="frame">The frame index of the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(string name, Surface surface, Vect2 cellSize, int frame, float speed, bool looped)
	{
		EngineAdd(name, surface, cellSize, frame, speed, looped);

		return this;
	}

	/// <summary>
	/// Adds a new animation to the animated sprite using an enum value as the name and a single frame index.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation.</param>
	/// <param name="surface">The surface containing the animation frames.</param>
	/// <param name="cellSize">The size of each cell in the animation frames.</param>
	/// <param name="frame">The frame index of the animation sequence.</param>
	/// <param name="speed">The speed of the animation.</param>
	/// <param name="looped">True if the animation should loop; otherwise, false.</param>
	/// <returns>The updated instance of AnimatedSprite with the added animation.</returns>
	public AnimatedSprite Add(Enum name, Surface surface, Vect2 cellSize, int frame, float speed, bool looped)
	{
		EngineAdd(name.ToEnumString(), surface, cellSize, frame, speed, looped);

		return this;
	}
	#endregion



	#region Remove
	/// <summary>
	/// Removes an animation from the animated sprite by name.
	/// </summary>
	/// <param name="name">The name of the animation to remove.</param>
	/// <returns>True if the animation was successfully removed; otherwise, false.</returns>
	public bool Remove(string name)
	{
		if (!Exists(name))
			return false;

		if (_animations.Remove(name))
		{
			Coroutine.Stop(_routine);

			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes an animation from the animated sprite by enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation to remove.</param>
	/// <returns>True if the animation was successfully removed; otherwise, false.</returns>
	public bool Remove(Enum name) => Remove(name.ToEnumString());
	#endregion



	#region Exists
	/// <summary>
	/// Checks if an animation exists in the animated sprite by name.
	/// </summary>
	/// <param name="name">The name of the animation to check.</param>
	/// <returns>True if the animation exists; otherwise, false.</returns>
	public bool Exists(string name) => _animations.ContainsKey(name);

	/// <summary>
	/// Checks if an animation exists in the animated sprite by enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation to check.</param>
	/// <returns>True if the animation exists; otherwise, false.</returns>
	public bool Exists(Enum name) => _animations.ContainsKey(name.ToEnumString());
	#endregion



	#region Get
	/// <summary>
	/// Gets the animation with the specified name.
	/// </summary>
	/// <param name="name">The name of the animation to retrieve.</param>
	/// <returns>The animation object if found; otherwise, null.</returns>
	public Animation Get(string name) => !Exists(name) ? default : _animations[name];

	/// <summary>
	/// Gets the animation with the specified enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation to retrieve.</param>
	/// <returns>The animation object if found; otherwise, null.</returns>
	public Animation Get(Enum name) => Get(name.ToEnumString());

	/// <summary>
	/// Tries to retrieve the animation with the specified name.
	/// </summary>
	/// <param name="name">The name of the animation to retrieve.</param>
	/// <param name="animation">When this method returns, contains the animation associated with the specified name, if the animation exists; otherwise, null.</param>
	/// <returns>True if the animation was successfully retrieved; otherwise, false.</returns>
	public bool TryGet(string name, out Animation animation)
	{
		animation = Get(name);

		return !animation.IsEmpty;
	}

	/// <summary>
	/// Tries to retrieve the animation with the specified enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation to retrieve.</param>
	/// <param name="animation">When this method returns, contains the animation associated with the specified enum name, if the animation exists; otherwise, null.</param>
	/// <returns>True if the animation was successfully retrieved; otherwise, false.</returns>
	public bool TryGet(Enum name, out Animation animation) => TryGet(name.ToEnumString(), out animation);

	#endregion



	#region Play
	/// <summary>
	/// Plays the animation with the specified name.
	/// </summary>
	/// <param name="name">The name of the animation to play.</param>
	/// <param name="reset">True to reset the animation frames to zero if currently playing; otherwise, false.</param>
	/// <returns>The current instance of AnimatedSprite.</returns>
	public AnimatedSprite Play(string name, bool reset = true)
	{
		EnginePlay(name, reset);

		return this;
	}

	/// <summary>
	/// Plays the animation with the specified enum name.
	/// </summary>
	/// <param name="name">The enum value representing the name of the animation to play.</param>
	/// <param name="reset">True to reset the animation frames to zero if currently playing; otherwise, false.</param>
	/// <returns>The current instance of AnimatedSprite.</returns>
	public AnimatedSprite Play(Enum name, bool reset = true) => Play(name.ToEnumString(), reset);

	#endregion

	/// <summary>
	/// Updates the state of the AnimatedSprite, typically called once per frame.
	/// </summary>
	protected override void Update()
	{
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;
		if (_animation.IsEmpty)
			return;

		_frame = Math.Clamp(_frameIndex, 0, _animation.Frames.Length - 1);

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
			Renderer.Draw(_animation.Surface, Position + Alignment(), _animation.Frames[_frame], Effect, Color, Layer);
		else
			target.Draw(_animation.Surface, Position - target.Position + Alignment(), _animation.Frames[_frame], Effect, Color, Layer);
	}

	#region Engine Only
	private IEnumerator Loop(Animation animation)
	{
		_animationFinished = false;

		while (!_animationFinished)
		{
			if (!Visible || Color.Alpha == 0)
			{
				while (!Visible || Color.Alpha == 0)
					yield return null;
			}

			yield return Clock.ToFps(animation.Speed);

			if (_frameIndex > animation.Frames.Length - 1)
			{
				if (animation.Looped)
					_frameIndex = 0;
				else
				{
					_frameIndex = animation.Frames.Length - 1;

					State = AnimationState.Stopped;

					Emit(EngineSignals.AnimatedSpriteFinished, this, animation);

					_animationFinished = true;
				}
			}
			else
				_frameIndex++;
		}
	}

	internal void EngineAdd(string name, Surface surface, Vect2 cellSize, int[] frames, float speed, bool looped)
	{
		if (Exists(name))
			return;

		var rectFrames = new Rect2[frames.Length];

		for (int i = 0; i < frames.Length; i++)
			rectFrames[i] = Surface.GetSource(surface, cellSize, frames[i]);

		_animations.Add(name, new Animation(name, surface, rectFrames, speed, looped));
	}

	internal void EngineAdd(string name, Surface surface, Vect2 cellSize, int frame, float speed, bool looped)
	{
		if (Exists(name))
			return;

		Rect2[] rectFrames = new Rect2[] { Surface.GetSource(surface, cellSize, frame) };

		_animations.Add(name, new Animation(name, surface, rectFrames, speed, looped));
	}

	internal void EngineAdd(string name, Surface surface, Vect2 cellSize, int start, int end, float speed, bool looped)
	{
		if (Exists(name))
			return;

		Rect2[] frames;
		int index = 0;

		if (start == end)
		{
			frames = new[] { Surface.GetSource(surface, cellSize, start) };
		}
		else if (start < end) // 0 to 5
		{
			if (end - start < 0)
				return;

			frames = new Rect2[(end - start) + 1];

			for (int i = start; i <= end; i++)
			{
				frames[index] = Surface.GetSource(surface, cellSize, i);
				index++;
			}
		}
		else // 5 to 0
		{
			if (start - end < 0)
				return;

			frames = new Rect2[(start - end) + 1];

			for (int i = start; i >= end; i--)
			{
				frames[index] = Surface.GetSource(surface, cellSize, i);
				index++;
			}
		}

		_animations.Add(name, new Animation(name, surface, frames, speed, looped));
	}

	internal void EngineAdd(Enum name, Surface surface, Vect2 cellSize, int[] frames, float speed, bool looped)
		=> EngineAdd(name.ToEnumString(), surface, cellSize, frames, speed, looped);

	internal void EnginePlay(string name, bool reset = true)
	{
		if (!Exists(name))
			return;
		if (!reset) // Might have to change this
		{
			if (_name == name)
				return;
		}

		if (_routine.IsRunning)
			Coroutine.Stop(_routine);

		_frameIndex = reset ? 0 : Math.Clamp(_frameIndex, 0, _animations[name].Frames.Length - 1);
		_animation = _animations[name];
		_name = name;

		if (!_routine.IsRunning)
			_routine = StartRoutine(Loop(_animation));

		State = AnimationState.Playing;

		Signal.Emit(EngineSignals.AnimatedSpriteStarted, this, _animation);

		if (!_sizeChanged)
			Size = _animation.Frames[_frame].Size;
	}

	internal void EnginePlay(Enum name, bool reset = true)
		=> EnginePlay(name.ToEnumString(), reset);

	private Vect2 Alignment()
	{
		var rect = _animation.Frames[_frame];
		var result = Vect2.Zero;

		result.X = AlignmentHelpers.AlignWidth(Size.X, rect.Width, HAlign);
		result.Y = AlignmentHelpers.AlignHeight(Size.Y, rect.Height, VAlign);

		return result;
	}
	#endregion
}
