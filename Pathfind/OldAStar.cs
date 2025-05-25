// namespace Box.Pathfind;

// /// <summary>
// /// Provides methods to perform the A* algorithm for pathfinding on an edge-weighted graph.
// /// </summary>
// public sealed class AStar
// {
// 	private readonly List<Vertex> _vertexes = new();

// 	/// <summary>
// 	/// Gets the number of vertices in the graph.
// 	/// </summary>
// 	public int Count => _vertexes.Count;

// 	/// <summary>
// 	/// Finds the shortest path between two vertices identified by their IDs and returns the path as a sequence of Vect2 points.
// 	/// </summary>
// 	/// <param name="id">The ID of the starting vertex.</param>
// 	/// <param name="toId">The ID of the target vertex.</param>
// 	/// <returns>An enumerable collection of Vect2 points representing the shortest path.</returns>
// 	public IEnumerable<Vect2> ShortestPathAsVector(int id, int toId)
// 	{
// 		var path = ShortestPathAsPoints(id, toId);

// 		if (!path.IsEmpty())
// 		{
// 			foreach (var item in path)
// 			{
// 				yield return InternalGetPoint(item).Position;
// 			}
// 		}
// 	}

// 	/// <summary>
// 	/// Finds the shortest path between two vertices identified by their IDs and returns the path as a list of vertex IDs.
// 	/// </summary>
// 	/// <param name="id">The ID of the starting vertex.</param>
// 	/// <param name="toId">The ID of the target vertex.</param>
// 	/// <returns>A list of vertex IDs representing the shortest path.</returns>
// 	public unsafe List<int> ShortestPathAsPoints(int id, int toId)
// 	{
// 		Vertex startingVertex = InternalGetPoint(id);

// 		if (startingVertex is null)
// 			return null;

// 		if (InternalGetPoint(id) is null || InternalGetPoint(toId) is null)
// 			return default;

// 		Queue<Vertex> queue = new();
// 		List<int> returnList = new();
// 		List<Vertex> wasVisited = new();
// 		Vertex[] parents = new Vertex[_vertexes.Count];

// 		queue.Enqueue(startingVertex);

// 		Vertex currentVertex = startingVertex;

// 		parents[_vertexes.IndexOf(startingVertex)] = null;

// 		do
// 		{
// 			if (!wasVisited.Contains(currentVertex))
// 			{
// 				wasVisited.Add(currentVertex);
// 				returnList.Add(currentVertex.Id);
// 			}

// 			fixed (Vertex* ptr = currentVertex.Edges.ToArray())
// 			{
// 				for (int i = 0; i < currentVertex.Edges.Count; i++)
// 				{
// 					var edge = ptr + i;

// 					if (!wasVisited.Contains(*edge))
// 					{
// 						queue.Enqueue(*edge);
// 						parents[_vertexes.IndexOf(*edge)] = currentVertex;
// 					}
// 				}
// 			}

// 			currentVertex = queue.Dequeue();
// 		} while (!currentVertex.Id.Equals(toId) && queue.Count > 0 && currentVertex is not null);

// 		returnList.Clear();

// 		while (currentVertex is not null)
// 		{
// 			returnList.Add(currentVertex.Id);
// 			currentVertex = parents[_vertexes.IndexOf(currentVertex)];
// 		}

// 		returnList.Reverse();

// 		return returnList;
// 	}

// 	/// <summary>
// 	/// Retrieves the position of a point (vertex) identified by its ID.
// 	/// </summary>
// 	/// <param name="id">The ID of the point.</param>
// 	/// <returns>The position of the point as an integer.</returns>
// 	public int GetPoint(int id)
// 	{
// 		foreach (Vertex vertex in _vertexes)
// 		{
// 			if (vertex.Id.Equals(id))
// 				return vertex.Id;
// 		}

// 		return -1;
// 	}

// 	/// <summary>
// 	/// Adds a new point (vertex) with the specified position to the graph.
// 	/// </summary>
// 	/// <param name="id">The ID of the new point.</param>
// 	/// <param name="position">The position of the new point as a Vect2.</param>
// 	/// <returns>True if the point was successfully added; otherwise, false.</returns>
// 	public bool AddPoints(int id, Vect2 position)
// 	{
// 		if (InternalGetPoint(id) is not null)
// 			return false;

// 		_vertexes.Add(new Vertex(id, position));

// 		return true;
// 	}

// 	/// <summary>
// 	/// Removes a point (vertex) from the graph identified by its ID.
// 	/// </summary>
// 	/// <param name="id">The ID of the point to remove.</param>
// 	/// <returns>True if the point was successfully removed; otherwise, false.</returns>
// 	public bool RemovePoints(int id)
// 	{
// 		Vertex targetVertex = InternalGetPoint(id);

// 		if (targetVertex is null)
// 			return false;

// 		int start = targetVertex.Edges.Count - 1;

// 		for (int i = start; i >= 0; i--)
// 			DisconnectPoints(id, targetVertex.Edges[i].Id);

// 		return true;
// 	}

// 	/// <summary>
// 	/// Removes a point (vertex) from the graph identified by its ID.
// 	/// </summary>
// 	/// <param name="id">The ID of the point to remove.</param>
// 	/// <returns>True if the point was successfully removed; otherwise, false.</returns>
// 	public bool RemovePoint(int id)
// 	{
// 		if (!RemovePoints(id))
// 			return false;

// 		Vertex targetVertex = InternalGetPoint(id);

// 		return _vertexes.Remove(targetVertex);
// 	}

// 	/// <summary>
// 	/// Connects two points (vertices) in the graph identified by their IDs.
// 	/// </summary>
// 	/// <param name="id">The ID of the first point.</param>
// 	/// <param name="toId">The ID of the second point.</param>
// 	/// <returns>True if the points were successfully connected; otherwise, false.</returns>
// 	public bool ConnectPoints(int id, int toId)
// 	{
// 		Vertex vertex = InternalGetPoint(id);

// 		if (vertex is null || vertex.IsConnected(toId))
// 			return false;

// 		Vertex otherVertex = InternalGetPoint(toId);

// 		if (otherVertex is null)
// 			return false;

// 		vertex.Edges.Add(otherVertex);
// 		otherVertex.Edges.Add(vertex);

// 		return true;
// 	}

// 	/// <summary>
// 	/// Removes a specific connection between two points (vertices) in the graph.
// 	/// </summary>
// 	/// <param name="id">The ID of the first point.</param>
// 	/// <param name="toId">The ID of the second point.</param>
// 	/// <returns>True if the connection was successfully removed; otherwise, false.</returns>
// 	public bool DisconnectPoints(int id, int toId)
// 	{
// 		Vertex vertex = InternalGetPoint(id);

// 		if (vertex is null || !vertex.IsConnected(toId))
// 			return false;

// 		Vertex otherVertex = InternalGetPoint(toId);

// 		if (otherVertex is null)
// 			return false;

// 		vertex.Edges.Remove(otherVertex);
// 		otherVertex.Edges.Remove(vertex);

// 		return true;
// 	}

// 	/// <summary>
// 	/// Checks if two points (vertices) in the graph are connected.
// 	/// </summary>
// 	/// <param name="id">The ID of the first point.</param>
// 	/// <param name="toId">The ID of the second point.</param>
// 	/// <returns>True if the points are connected; otherwise, false.</returns>
// 	public bool ArePointsConnected(int id, int toId)
// 	{
// 		Vertex vertex = InternalGetPoint(id);

// 		if (vertex is null)
// 			return false;

// 		return vertex.IsConnected(toId);
// 	}


// 	private Vertex InternalGetPoint(int id)
// 	{
// 		foreach (Vertex vertex in _vertexes)
// 		{
// 			if (vertex.Id != id)
// 				continue;

// 			return vertex;
// 		}

// 		return null;
// 	}
// }
