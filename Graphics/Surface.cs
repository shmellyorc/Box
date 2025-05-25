namespace Box.Graphics;

// The supported image formats are bmp, png, tga, jpg, gif, psd, hdr, pic and pnm. Some format options are not supported, 
// like jpeg with arithmetic coding or ASCII pnm. If this function fails, the image is left unchanged.

/// <summary>
/// Represents a texture asset used for rendering.
/// </summary>
public class Surface : IAsset, IEquatable<Surface>
{
	private readonly SFMLTexture _texture;

	public uint Id { get; private set; }

	/// <summary>
	/// Gets the size of the surface. Returns Vect2.Zero if the surface is null.
	/// </summary>
	public Vect2 Size => _texture is not null ? new(_texture.Size) : Vect2.Zero;

	/// <summary>
	/// Gets the bounds of the surface as a Rect2 starting from Vect2.Zero with the size of Size.
	/// </summary>
	public Rect2 Bounds => new(Vect2.Zero, Size);

	/// <summary>
	/// Gets the width of the surface.
	/// </summary>
	public int Width => (int)Size.X;

	/// <summary>
	/// Gets the height of the surface.
	/// </summary>
	public int Height => (int)Size.Y;

	/// <summary>
	/// Gets the filename associated with the surface.
	/// </summary>
	public string Filename { get; }

	/// <summary>
	/// Gets or sets whether smoothing is applied when rendering the surface.
	/// </summary>
	public bool Smooth
	{
		get => IsValid && _texture.Smooth;
		set
		{
			if (!IsValid)
				return;

			_texture.Smooth = value;
		}
	}

	/// <summary>
	/// Gets or sets whether the surface is repeated when rendered beyond its dimensions.
	/// </summary>
	public bool Repeated
	{
		get => IsValid && _texture.Repeated;
		set
		{
			if (!IsValid)
				return;

			_texture.Repeated = value;
		}
	}

	// /// <summary>
	// /// Gets a value indicating whether the surface is empty (null).
	// /// </summary>
	// public readonly bool IsEmpty => _texture is null;

	public bool IsValid => !IsDisposed && _texture.NativeHandle != 0;
	public ulong Handle => _texture.NativeHandle;

	/// <summary>
	/// Gets or sets a value indicating whether the surface has been disposed.
	/// </summary>
	public bool IsDisposed { get; internal set; }

	/// <summary>
	/// Gets or sets a value indicating whether the surface has been initialized.
	/// </summary>
	public bool Initialized { get; internal set; }

	internal Surface(string filename, byte[] bytes, Rect2 region, bool repeat, bool smooth)
	{
		Filename = filename;

		if (region.IsEmpty)
			_texture = new SFMLTexture(bytes);
		else
		{
			var image = new SFMLImage(bytes);

			_texture = new SFMLTexture(image, region.ToSFMLI()) // Creates a new image
			{
				Repeated = repeat,
				Smooth = smooth
			};
		}

		Id = HashHelpers.Hash32(
			$"{filename}{_texture.NativeHandle:X8}{bytes.Length:X8}{(int)region.X:X8}{(int)region.Y:X8}{(int)region.Width:X8}{(int)region.Height:X8}");
	}

	/// <summary>
	/// Initializes a new instance of the Surface struct using a region of another surface.
	/// </summary>
	/// <param name="surface">The source surface from which to create the new surface.</param>
	/// <param name="region">The specific region of the source surface to be used for the new surface.</param>
	/// <param name="repeat">A boolean indicating whether the region should be repeated to fill the new surface.</param>
	/// <param name="smooth">A boolean indicating whether the new surface should be smoothed (anti-aliased).</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="surface"/> or <paramref name="region"/> is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="surface"/> is empty or <paramref name="region"/> is empty.</exception>
	public Surface(Surface surface, Rect2 region, bool repeat, bool smooth)
	{
		if (surface == null)
			throw new ArgumentNullException(nameof(surface), "Surface is empty or null");
		if (region.IsEmpty)
			throw new InvalidOperationException("Surface region is empty");

		var image = new SFMLImage(surface._texture.CopyToImage());

		_texture = new SFMLTexture(image, region.ToSFMLI())
		{
			Repeated = repeat,
			Smooth = smooth
		};

		Filename = surface.Filename;

		Id = HashHelpers.Hash32(
			$"{Filename}{_texture.NativeHandle:X8}{(int)region.X:X8}{(int)region.Y:X8}{(int)region.Width:X8}{(int)region.Height:X8}");
	}

	internal Surface(string filename, bool repeat, bool smooth)
		: this(filename, File.ReadAllBytes(filename), Rect2.Empty, repeat, smooth) { }
	internal Surface(string filename, byte[] bytes, bool repeat, bool smooth)
		: this(filename, bytes, Rect2.Empty, repeat, smooth) { }

	internal Surface(SFMLTexture texture)
	{
		_texture = texture;

		Filename = string.Empty;
		Id = HashHelpers.Hash32($"{_texture.NativeHandle:X8}{_texture.Size.X:X8}{_texture.Size.Y:X8}");
	}

	/// <summary>
	/// Initializes a new instance of the Surface class with specified width, height, and color.
	/// </summary>
	/// <param name="width">The width of the surface.</param>
	/// <param name="height">The height of the surface.</param>
	/// <param name="color">The initial color of the surface.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.</exception>
	public Surface(int width, int height, BoxColor color)
	{
		if (width <= 0)
			throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
		if (height <= 0)
			throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

		// {
		_texture = new SFMLTexture(
			new SFMLImage((uint)width, (uint)height, color.ToSFML())
		);

		Filename = string.Empty;
		// }

		Id = HashHelpers.Hash32($"{_texture.NativeHandle:X8}{Width:X8}{Height:X8}{color.Red:X8}{color.Green:X8}{color.Blue:X8}{color.Alpha:X8}");
	}

	/// <summary>
	/// Initializes a new instance of the Surface class with specified width, height.
	/// </summary>
	/// <param name="width">The width of the surface.</param>
	/// <param name="height">The height of the surface.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.</exception>
	public Surface(int width, int height) : this(width, height, BoxColor.AllShades.White) { }



	#region GetSource
	/// <summary>
	/// Gets the source rectangle on the surface based on cell size and index.
	/// </summary>
	/// <param name="cellSize">The size of each cell on the surface.</param>
	/// <param name="index">The index of the cell.</param>
	/// <returns>The source rectangle on the surface.</returns>
	public Rect2 GetSource(Vect2 cellSize, int index) => GetSource(this, cellSize, index);

	/// <summary>
	/// Gets the source rectangle on the surface based on cell size and index.
	/// </summary>
	/// <param name="surface">The surface from which to retrieve the source rectangle.</param>
	/// <param name="cellSize">The size of each cell on the surface.</param>
	/// <param name="index">The index of the cell.</param>
	/// <returns>The source rectangle on the surface.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="surface"/> is empty.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="cellSize"/> has non-positive dimensions.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative</exception>
	public static Rect2 GetSource(Surface surface, Vect2 cellSize, int index)
	{
		if (surface == null)
			throw new ArgumentNullException(nameof(surface), "Surface cannot be null.");
		if (cellSize.X <= 0 || cellSize.Y <= 0)
			throw new ArgumentException("Cell size dimensions must be greater than zero.", nameof(cellSize));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

		var location = Vect2.To2D(index, surface.Width / cellSize.X);
		var position = location * cellSize;

		return new Rect2(position, cellSize);
	}
	#endregion


	internal SFMLTexture ToSFML() => _texture;

	/// <summary>
	/// Disposes the resources held by the Surface instance.
	/// </summary>
	public void Dispose()
	{
		if (IsDisposed)
			return;

		_texture?.Dispose();

		IsDisposed = true;
	}

	/// <summary>
	/// Checks if two Surface objects are equal.
	/// </summary>
	/// <param name="left">The left-hand side Surface.</param>
	/// <param name="right">The right-hand side Surface.</param>
	/// <returns>True if the Surfaces are equal, false otherwise.</returns>
	public static bool operator ==(Surface left, Surface right)
	{
		if (ReferenceEquals(left, right)) return true;
		if (left is null || right is null) return false;

		return (left._texture.CPointer, left.Size, left.Smooth, left.Repeated, left.Filename)
			== (right._texture.CPointer, right.Size, right.Smooth, right.Repeated, right.Filename);
	}

	/// <summary>
	/// Checks if two Surface objects are not equal.
	/// </summary>
	/// <param name="left">The left-hand side Surface.</param>
	/// <param name="right">The right-hand side Surface.</param>
	/// <returns>True if the Surfaces are not equal, false otherwise.</returns>
	public static bool operator !=(Surface left, Surface right) => !(left == right);

	/// <summary>
	/// Checks if the current Surface is equal to another Surface.
	/// </summary>
	/// <param name="other">The other Surface to compare to.</param>
	/// <returns>True if the Surfaces are equal, false otherwise.</returns>
	public bool Equals(Surface other)
		=> (_texture.CPointer, _texture.Size, Smooth, Repeated, Filename)
			== (other._texture.CPointer, other._texture.Size, other.Smooth, other.Repeated, other.Filename);

	/// <summary>
	/// Checks if the current Surface is equal to an object.
	/// </summary>
	/// <param name="obj">The object to compare to.</param>
	/// <returns>True if obj is a Surface and is equal to the current Surface, false otherwise.</returns>
	public override bool Equals([NotNullWhen(true)] object obj)
	{
		if (obj is Surface value)
			return Equals(value);

		return false;
	}

	/// <summary>
	/// Gets the hash code for the Surface.
	/// </summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode()
		=> HashCode.Combine(_texture.CPointer, _texture.Size, Smooth, Repeated, Filename);

	/// <summary>
	/// Initializes the Surface.
	/// </summary>
	public void Initialize() => Initialized = true;
}
