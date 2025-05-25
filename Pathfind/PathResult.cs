namespace Box.Pathfind;

/// <summary>
/// Represents the result of a pathfinding operation including the path and total cost.
/// </summary>
public struct PathResult
{
	/// <summary>
	/// The list of vertex IDs representing the path from start to goal.
	/// </summary>
	public List<int> Path;

	/// <summary>
	/// The total cost of the resulting path.
	/// </summary>
	public float TotalCost;
}
