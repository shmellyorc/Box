using Box.Graphics.Batch;

namespace Box.Entities.Graphics;

/// <summary>
/// Represents a nine-patch entity used for scalable UI elements.
/// </summary>
public class Ninepatch : Entity
{
	private const int TopLeft = 0, TopMiddle = 1, TopRight = 2;
	private const int MiddleLeft = 3, Middle = 4, MiddleRight = 5;
	private const int BottomLeft = 6, BottomMiddle = 7, BottomRight = 8;

	private readonly Rect2[] _dstPatches = new Rect2[9];
	private readonly Rect2[] _srcPatches = new Rect2[9];
	private readonly Rect2 _texRect;
	private readonly float _left, _right, _top, _bottom;
	private readonly Surface _surface;

	private Rect2 _srcRect, _dstRect;
	private Vect2 _oldPosition = Vect2.Zero, _oldSize = Vect2.Zero;

	/// <summary>
	/// The color of the nine-patch entity, defaulting to white.
	/// </summary>
	public BoxColor Color = BoxColor.AllShades.White;

	/// <summary>
	/// Constructs a nine-patch entity from the specified surface and source rectangle, with defined corner patches.
	/// </summary>
	/// <param name="surface">The surface used for rendering.</param>
	/// <param name="source">The source rectangle defining the area of the surface to render.</param>
	/// <param name="corners">The rectangle defining the corners of the nine-patch.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="surface"/> is null.</exception>
	public Ninepatch(Surface surface, Rect2 source, Rect2 corners)
	{
		if (surface == null)
			throw new ArgumentNullException(nameof(surface), "Surface is empty!");

		_surface = surface;

		_texRect = source;
		_left = corners.X;
		_top = corners.Y;
		_right = corners.Width;
		_bottom = corners.Height;

		UpdateNinePatch();
	}

	/// <summary>
	/// Constructs a nine-patch entity from the specified surface, spritesheet, and sprite name.
	/// </summary>
	/// <param name="surface">The surface used for rendering.</param>
	/// <param name="sheet">The spritesheet from which to fetch the sprite.</param>
	/// <param name="name">The name of the sprite in the spritesheet.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="surface"/>, <paramref name="sheet"/>, or <paramref name="name"/> is null.</exception>
	public Ninepatch(Surface surface, Spritesheet sheet, string name)
	{
		if (surface == null)
			throw new ArgumentNullException(nameof(surface), "Surface is empty!");
		if (surface == null)
			throw new ArgumentNullException(nameof(sheet), "Spritesheet is empty!");
		if (!sheet.Contains(name))
			throw new ArgumentNullException(nameof(name), $"Spritesheet {name} doesnt exist!");

		_surface = surface;
		_texRect = sheet.GetBounds(name);

		_left = sheet.GetNinepatch(name).Left;
		_top = sheet.GetNinepatch(name).Top;
		_right = sheet.GetNinepatch(name).Right;
		_bottom = sheet.GetNinepatch(name).Bottom;

		UpdateNinePatch();
	}

	/// <summary>
	/// Updates the nine-patch entity.
	/// </summary>
	protected unsafe override void Update()
	{
		if (Size == Vect2.Zero)
			return;
		if (!Visible)
			return;
		if (Color.Alpha == 0)
			return;

		UpdateNinePatch();

		if (!AnyParentOfType<BoxRenderTarget>(out var target))
		{
			fixed (Rect2* dst = _dstPatches)
			{
				fixed (Rect2* src = _srcPatches)
				{
					for (var i = 0; i < _srcPatches.Length; i++)
					{
						var dstPtr = dst + i;
						var srcPtr = src + i;

						Renderer.Instance.Draw(_surface, *dstPtr, *srcPtr, SurfaceEffects.None, Color, Layer);
					}
				}
			}
		}
		else
		{
			fixed (Rect2* dst = _dstPatches)
			{
				fixed (Rect2* src = _srcPatches)
				{
					for (var i = 0; i < _srcPatches.Length; i++)
					{
						var dstPtr = dst + i;
						var srcPtr = src + i;

						target.Draw(_surface, *dstPtr, *srcPtr, SurfaceEffects.None, Color, Layer);
					}
				}
			}
		}

		if (EngineSettings.Instance.DebugDraw)
			Renderer.Instance.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f, BoxColor.AllShades.Blue);
	}

	private void UpdateNinePatch()
	{
		if (_oldPosition != Position || _oldSize != Size)
		{
			_srcRect = _texRect;
			_dstRect = AnyParentOfType<BoxRenderTarget>(out var target)
				? new Rect2(Position - target.Position, Size)
				: new Rect2(Position, Size);

			CreatePatches(_srcRect, _srcPatches);
			CreatePatches(_dstRect, _dstPatches);

			if (_oldPosition != Position)
				_oldPosition = Position;
			if (_oldSize != Size)
				_oldSize = Size;
		}
	}

	private void CreatePatches(Rect2 sourceRectangle, Rect2[] patchCache)
	{
		var x = sourceRectangle.X;
		var y = sourceRectangle.Y;
		var w = sourceRectangle.Width;
		var h = sourceRectangle.Height;
		var middleWidth = w - _left - _right;
		var middleHeight = h - _top - _bottom;
		var bottomY = y + h - _bottom;
		var rightX = x + w - _right;
		var leftX = x + _left;
		var topY = y + _top;

		patchCache[TopLeft] = new Rect2(x, y, _left, _top);
		patchCache[TopMiddle] = new Rect2(leftX, y, middleWidth, _top);
		patchCache[TopRight] = new Rect2(rightX, y, _right, _top);

		patchCache[MiddleLeft] = new Rect2(x, topY, _left, middleHeight);
		patchCache[Middle] = new Rect2(leftX, topY, middleWidth, middleHeight);
		patchCache[MiddleRight] = new Rect2(rightX, topY, _right, middleHeight);

		patchCache[BottomLeft] = new Rect2(x, bottomY, _left, _bottom);
		patchCache[BottomMiddle] = new Rect2(leftX, bottomY, middleWidth, _bottom);
		patchCache[BottomRight] = new Rect2(rightX, bottomY, _right, _bottom);
	}
}
