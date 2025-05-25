namespace Box.Pathfind;

/// <summary>
/// Specifies which heuristic function to use for A* pathfinding.
/// </summary>
/// <summary>
/// Specifies which heuristic function to use for A* pathfinding.
/// </summary>
public enum HeuristicType
{
	/// <summary>
	/// Uses Euclidean distance (straight-line) as the heuristic.
	/// </summary>
	Euclidean,

	/// <summary>
	/// Uses Manhattan distance (grid-aligned movement only) as the heuristic.
	/// </summary>
	Manhattan,

	/// <summary>
	/// Uses Diagonal distance (maximum axis difference) as the heuristic.
	/// </summary>
	Diagonal
}
