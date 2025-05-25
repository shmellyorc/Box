namespace Box.Pathfind;

/// <summary>
/// Represents a node in the graph with a position, ID, and connections.
/// </summary>
public sealed class Vertex
{
	/// <summary>
	/// Indicates whether the vertex is currently active and should be considered during pathfinding.
	/// </summary>
	public bool IsActive { get; set; } = true;
	/// <summary>
	/// Gets or sets the unique identifier of the vertex.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	/// Gets or sets the 2D position of the vertex in the graph.
	/// </summary>
	public Vect2 Position { get; set; }
	/// <summary>
	/// A list of edges connecting this vertex to its neighbors.
	/// </summary>
	public List<Edge> Edges = new();

	/// <summary>
	/// Initializes a new instance of the Vertex class with the given ID and position.
	/// </summary>
	/// <param name="id">The unique identifier for the vertex.</param>
	/// <param name="position">The position of the vertex.</param>
	internal Vertex(int id, Vect2 position)
	{
		Id = id;
		Position = position;
	}

	/// <summary>
	/// Determines whether this vertex is connected to another vertex by ID.
	/// </summary>
	/// <param name="id">The ID of the vertex to check connectivity with.</param>
	/// <returns>True if connected; otherwise, false.</returns>
	public bool IsConnected(int id) => Edges.Any(e => e.Target.Id == id);
}

