namespace Box.Enums;

/// <summary>
/// Flags representing surface effects.
/// </summary>
[Flags]
public enum SurfaceEffects
{
	/// <summary>
	/// No effects applied.
	/// </summary>
	None = 0,

	/// <summary>
	/// Applies a vertical effect.
	/// </summary>
	FlipVErtically = 1,

	/// <summary>
	/// Applies a horizontal effect.
	/// </summary>
	FlipHorizontally = 2,
	FlipBoth = FlipHorizontally | FlipVErtically
}
