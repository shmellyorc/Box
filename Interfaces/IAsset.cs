namespace Box.Interfaces;

/// <summary>
/// Represents an asset that can be managed, disposed, and initialized.
/// </summary>
public interface IAsset
{
	public uint Id { get; }

	/// <summary>
	/// Gets the filename of the asset.
	/// </summary>
	string Filename { get; }

	/// <summary>
	/// Gets a value indicating whether the asset has been disposed.
	/// </summary>
	bool IsDisposed { get; }

	/// <summary>
	/// Gets a value indicating whether the asset has been initialized.
	/// </summary>
	bool Initialized { get; }

	/// <summary>
	/// Disposes of the asset and releases any resources.
	/// </summary>
	void Dispose();

	/// <summary>
	/// Initializes the asset and prepares it for use.
	/// </summary>
	void Initialize();
}
