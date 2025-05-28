using Box.Graphics.Batch;

namespace Box.Systems;

/// <summary>
/// Represents a camera instance used for following, panning, with each screen having its own dedicated camera.
/// </summary>
public sealed class Camera
{
	private Vect2 _scale;
	private SFMLView _view;
	private readonly Screen _screen;
	private Vect2 _position, _shakeOFfset;
	private Rect2 _bounds, _area;
	private Entity _entity;
	private bool _shaking;
	private float _shakeMagnitude, _shakeDuration, _shakeTimer;
	private float NextFloat => FastRandom.Instance.NextFloat() * 2f - 1f;
	private Rect2 _edgeRegion;

	/// <summary>
	/// Restricts the camera position to stay within a specified rectangle.
	/// </summary>
	public Rect2 Clamp = Rect2.Empty;

	/// <summary>
	/// Adjusts the camera position by applying an offset.
	/// </summary>
	public Vect2 Offset = Vect2.Zero;

	/// <summary>
	/// 
	/// </summary>
	public Rect2 Bounds => _bounds;

	/// <summary>
	/// Represents the bounding area of the camera's visible viewport.
	/// </summary>
	public Rect2 Area => _area;

	/// <summary>
	/// Indicates whether the camera is currently following an entity.
	/// </summary>
	public bool IsFollowing => _entity is not null;

	/// <summary>
	/// Specifies the ease type to be used when the camera is following an entity.
	/// </summary>
	public EaseType CameraEaseType = EaseType.Linear;

	/// <summary>
	/// Gets the default scale factor based on the window size and the viewport size.
	/// </summary>
	/// <remarks>
	/// This property returns a <c>Vect2</c> representing the ratio of the engine window dimensions
	/// to the viewport dimensions. It is typically used for scaling coordinates, UI elements, or
	/// graphics to match the current resolution.
	/// </remarks>
	/// <value>
	/// A <c>Vect2</c> containing the horizontal and vertical scale factors.
	/// </value>
	public Vect2 DefaultScale => EngineSettings.Instance.Window / EngineSettings.Instance.Viewport;

	/// <summary>
	/// Represents the current scale factor of the camera, relative to the window size.
	/// </summary>
	public float Zoom
	{
		get => _scale.X;
		set
		{
			var oldScale = _scale;
			_scale = EngineSettings.Instance.Window / value;

			if (_scale != oldScale)
				_view = new SFMLView(new SFMLFloatRect(_position.X, _position.Y, _scale.X, _scale.Y));
		}
	}

	/// <summary>
	/// Represents the camera movement speed, typically used for camera following.
	/// </summary>
	public float Speed = 5f;

	/// <summary>
	/// Represents the position of the camera.
	/// </summary>
	public Vect2 Position
	{
		get => _position;
		set
		{
			if (IsFollowing)
				return;

			_position = value;
		}
	}

	internal Camera(Screen screen)
	{
		_screen = screen;

		_view = new SFMLView(new SFMLFloatRect(0, 0,
			EngineSettings.Instance.Viewport.X, EngineSettings.Instance.Viewport.Y));

		_scale = DefaultScale;

		_bounds = new Rect2(Vect2.Zero, EngineSettings.Instance.Viewport + EngineSettings.Instance.CullSize * 2);

		_area = new Rect2(Vect2.Zero, EngineSettings.Instance.Viewport);

		_edgeRegion = new Rect2(Vect2.Zero, EngineSettings.Instance.Viewport).Expand(-1, -1);

		_position = Renderer.Instance.Center;
	}

	/// <summary>
	/// Resets the internal view using the current position and viewport settings.
	/// </summary>
	/// <remarks>
	/// This method updates the view to match the current position of the object and the
	/// dimensions defined in <c>EngineSettings.Instance.Viewport</c>.
	/// </remarks>
	public void Reset()
	{
		if (_scale == DefaultScale)
			return;

		_view.Reset(new SFMLFloatRect(_position.X, _position.Y,
			EngineSettings.Instance.Viewport.X, EngineSettings.Instance.Viewport.Y));

		_scale = DefaultScale;
	}

	/// <summary>
	/// Checks if the rectangle is currently within the viewport.
	/// </summary>
	/// <param name="rectangle">The entity to check.</param>
	/// <returns>True if the rectangle is visible within the viewport; otherwise, false.</returns>
	public bool InViewport(Rect2 rectangle) => _bounds.Intersects(rectangle);

	/// <summary>
	/// Checks if the entity is currently within the viewport.
	/// </summary>
	/// <param name="entity">The entity to check.</param>
	/// <returns>True if the entity is visible within the viewport; otherwise, false.</returns>
	public bool InViewport(Entity entity) => InViewport(entity.Bounds);

	/// <summary>
	/// Sets the camera to follow an entity automatically.
	/// </summary>
	/// <param name="entity">The entity that the camera will follow.</param>
	/// <param name="teleportToEntity">Specifies whether to instantly teleport the camera to the entity, bypassing smooth transition.</param>
	public void Follow(Entity entity, bool teleportToEntity)
	{
		if (entity is null)
			return;
		if (entity.IsExiting)
			return;
		if (_entity == entity)
			return;

		if (teleportToEntity)
			_position = entity.Position;

		_entity = entity;
	}

	/// <summary>
	/// Stops the camera from following the selected entity previously set by the Follow method.
	/// </summary>
	public void UnFollow()
	{
		if (_entity is null)
			return;

		_entity = null;
	}

	/// <summary>
	/// Shakes the camera with a specified magnitude and duration.
	/// </summary>
	/// <param name="magnitude">The strength of the shake.</param>
	/// <param name="duration">The duration of the shake in seconds.</param>
	public void Shake(float magnitude, float duration)
	{
		_shakeTimer = 0f;
		_shakeMagnitude = magnitude;
		_shakeDuration = duration;

		_shaking = true;
	}


	internal void Update()
	{
		UpdateShake();
		UpdateFollow();
		UpdateClamp();

		_bounds.Position = _position - _bounds.Size / 2;

		_area.Position = _position - _area.Size / 2;

		_view.Center = _position.ToSFML_F();
	}

	private void UpdateShake()
	{
		if (!_shaking)
			return;

		_shakeTimer += Clock.Instance.DeltaTime;

		if (_shakeTimer >= _shakeDuration)
		{
			_shaking = false;
			_shakeTimer = _shakeDuration;
		}

		float progress = _shakeTimer / _shakeDuration;
		float magnitude = _shakeMagnitude * (1f - progress * progress);

		_shakeOFfset = new Vect2(NextFloat, NextFloat) * magnitude;
	}

	private void UpdateFollow()
	{
		if (_entity is null)
			return;

		_position = EasingHelpers.Ease(CameraEaseType, _position, _entity.Position + Offset, Speed 
			* Clock.Instance.DeltaTime);

		if (!ViewOnEdge() && _position.Round() != _entity.Position + Offset)
			_screen.IsDirty = true;
	}

	private bool ViewOnEdge()
	{
		var center = Renderer.Instance.Center;
		var left = _position.X - center.X;
		var right = _position.X + center.X;
		var top = _position.Y - center.Y;
		var bottom = _position.Y + center.Y;

		return !_edgeRegion.Contains(left, top) && !_edgeRegion.Contains(right, bottom);
	}

	private void UpdateClamp()
	{
		var newPosition = _entity is null
			? _position + Offset + _shakeOFfset
			: _position + _shakeOFfset;

		if (!Clamp.IsEmpty)
		{
			var center = Renderer.Instance.Center;
			var left = Clamp.Left + center.X;
			var right = Clamp.Right - center.X;
			var top = Clamp.Top + center.Y;
			var bottom = Clamp.Bottom - center.Y;

			if (left < right)
				newPosition.X = Math.Clamp(newPosition.X, Clamp.Left + center.X, Clamp.Right - center.X);
			else
				newPosition.X = Clamp.Size.X / 2;

			if (top < bottom)
				newPosition.Y = Math.Clamp(newPosition.Y, Clamp.Top + center.Y, Clamp.Bottom - center.Y);
			else
				newPosition.Y = Clamp.Size.Y / 2;
		}

		_position = newPosition;
	}

	internal SFMLView ToSFML() => _view;
}
