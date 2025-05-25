namespace Box.Pathfind;

/// <summary>
/// Represents a connection (edge) between two vertices with a weight and enabled state.
/// </summary>
public sealed class Edge
{
	/// <summary>
	/// Indicates whether the edge is enabled and should be considered during pathfinding.
	/// </summary>
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// The target vertex that this edge leads to.
	/// </summary>
	public Vertex Target { get; }

	/// <summary>
	/// The cost (weight) of traversing this edge.
	/// </summary>
	public float Cost { get; }

	/// <summary>
	/// Initializes a new edge to a target vertex with a given traversal cost.
	/// </summary>
	/// <param name="target">The vertex this edge connects to.</param>
	/// <param name="cost">The cost to traverse this edge.</param>
	public Edge(Vertex target, float cost)
	{
		Target = target;
		Cost = cost;
	}
}

