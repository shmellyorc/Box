namespace Box.Services.Types;

/// <summary>
/// Represents a base service that participates in the engine update loop and can be toggled visible.
/// Inherits from <see cref="GameService"/>.
/// </summary>
public class UpdatableService : GameService
{
	/// <summary>
	/// Indicates whether the service is currently visible or should be skipped in rendering or debug UI.
	/// </summary>
	public bool Visible { get; set; } = true;

	/// <summary>
	/// Called once per frame to update the service's logic. Override to implement custom update behavior.
	/// </summary>
	public virtual void Update(float dt) { }
}
