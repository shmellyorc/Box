namespace Box.Pathfind;

/// <summary>
/// Provides a flexible and efficient A* pathfinding system supporting weighted edges, multiple heuristics, object pooling, and directional graph traversal.
/// </summary>
public sealed class AStar
{
	private readonly Dictionary<int, Vertex> _vertices = new();
	private readonly Stack<List<int>> _pathPool = new();
	private readonly Stack<Dictionary<Vertex, Vertex>> _cameFromPool = new();
	private readonly Stack<HashSet<Vertex>> _visitedPool = new();
	private readonly Dictionary<(int, int), float> _heuristicCache = new();

	/// <summary>
	/// Gets the number of vertices currently in the graph.
	/// </summary>
	public int Count => _vertices.Count;

	/// <summary>
	/// Adds a new vertex to the graph with a specified ID and position.
	/// </summary>
	/// <param name="id">The unique ID of the vertex.</param>
	/// <param name="position">The position of the vertex.</param>
	/// <returns>True if the vertex was added; false if the ID already exists.</returns>
	public bool AddPoint(int id, Vect2 position)
	{
		if (_vertices.ContainsKey(id))
			return false;

		_vertices[id] = new Vertex(id, position);

		return true;
	}

	/// <summary>
	/// Removes a vertex and all its associated edges from the graph.
	/// </summary>
	/// <param name="id">The ID of the vertex to remove.</param>
	/// <returns>True if the vertex was found and removed; otherwise, false.</returns>
	public bool RemovePoint(int id)
	{
		if (!_vertices.TryGetValue(id, out var target))
			return false;

		foreach (var vertex in _vertices.Values)
			vertex.Edges.RemoveAll(e => e.Target.Id == id);

		return _vertices.Remove(id);
	}

	/// <summary>
	/// Connects two vertices in the graph with a weighted edge.
	/// </summary>
	/// <param name="id">The ID of the starting vertex.</param>
	/// <param name="toId">The ID of the destination vertex.</param>
	/// <param name="cost">The cost of traversing the edge.</param>
	/// <param name="bidirectional">If true, adds an edge in both directions.</param>
	/// <returns>True if the connection was created; otherwise, false.</returns>
	public bool ConnectPoints(int id, int toId, float cost = 1f, bool bidirectional = true)
	{
		if (!_vertices.TryGetValue(id, out var a) || !_vertices.TryGetValue(toId, out var b) || a.IsConnected(toId))
			return false;

		a.Edges.Add(new Edge(b, cost));

		if (bidirectional)
			b.Edges.Add(new Edge(a, cost));

		return true;
	}

	/// <summary>
	/// Disconnects two vertices in the graph by removing edges between them.
	/// </summary>
	/// <param name="id">The ID of the first vertex.</param>
	/// <param name="toId">The ID of the second vertex.</param>
	/// <returns>True if both vertices were found and disconnected; otherwise, false.</returns>
	public bool DisconnectPoints(int id, int toId)
	{
		if (!_vertices.TryGetValue(id, out var a) || !_vertices.TryGetValue(toId, out var b))
			return false;

		a.Edges.RemoveAll(e => e.Target.Id == toId);
		b.Edges.RemoveAll(e => e.Target.Id == id);

		return true;
	}

	/// <summary>
	/// Clears all internal object pools and cached heuristic values.
	/// </summary>
	public void ClearPools()
	{
		_pathPool.Clear();
		_cameFromPool.Clear();
		_visitedPool.Clear();
		_heuristicCache.Clear();
	}

	/// <summary>
	/// Finds the shortest path between two vertices using the A* algorithm.
	/// </summary>
	/// <param name="startId">The ID of the start vertex.</param>
	/// <param name="goalId">The ID of the goal vertex.</param>
	/// <param name="heuristicType">The type of heuristic to use (Euclidean, Manhattan, Diagonal).</param>
	/// <param name="costModifier">Optional cost modifier function to adjust edge cost dynamically.</param>
	/// <returns>A list of vertex IDs representing the path, or null if no path found.</returns>
	public PathResult? FindPath(int startId, int goalId, HeuristicType heuristicType = HeuristicType.Euclidean, Func<Vertex, Vertex, float> costModifier = null)
	{
		if (!_vertices.TryGetValue(startId, out var start) || !_vertices.TryGetValue(goalId, out var goal))
			return null;

		var openSet = new PriorityQueue<Vertex, float>();
		var cameFrom = _cameFromPool.Count > 0 ? _cameFromPool.Pop() : new Dictionary<Vertex, Vertex>();
		var gScore = new Dictionary<Vertex, float> { [start] = 0 };
		var fScore = new Dictionary<Vertex, float> { [start] = GetHeuristic(start, goal, heuristicType) };
		var visited = _visitedPool.Count > 0 ? _visitedPool.Pop() : new HashSet<Vertex>();

		openSet.Enqueue(start, fScore[start]);

		while (openSet.Count > 0)
		{
			var current = openSet.Dequeue();

			if (current.Id == goal.Id)
			{
				var result = ReconstructPath(cameFrom, current);
				cameFrom.Clear();
				visited.Clear();
				_cameFromPool.Push(cameFrom);
				_visitedPool.Push(visited);
				float totalCost = gScore[goal];
				return new PathResult { Path = result, TotalCost = totalCost };
			}

			if (!visited.Add(current))
				continue;

			foreach (var edge in current.Edges.Where(e => e.IsEnabled && e.Target.IsActive))
			{
				float modifier = costModifier?.Invoke(current, edge.Target) ?? 1f;
				float tentativeG = gScore[current] + edge.Cost * modifier;

				if (!gScore.TryGetValue(edge.Target, out var g) || tentativeG < g)
				{
					cameFrom[edge.Target] = current;
					gScore[edge.Target] = tentativeG;
					fScore[edge.Target] = tentativeG + GetHeuristic(edge.Target, goal, heuristicType) * 1.2f;
					openSet.Enqueue(edge.Target, fScore[edge.Target]);
				}
			}
		}

		cameFrom.Clear();
		visited.Clear();

		_cameFromPool.Push(cameFrom);
		_visitedPool.Push(visited);

		return null; // no path found
	}

	/// <summary>
	/// Finds the shortest path between two vertices and returns a list of vertex IDs.
	/// </summary>
	/// <param name="startId">The ID of the starting vertex.</param>
	/// <param name="goalId">The ID of the target vertex.</param>
	/// <param name="heuristicType">The heuristic type to use for pathfinding (default is Euclidean).</param>
	/// <param name="costModifier">An optional function to dynamically modify edge traversal cost.</param>
	/// <returns>A list of vertex IDs representing the shortest path, or null if no path is found.</returns>
	public List<int> ShortestPath(int startId, int goalId, HeuristicType heuristicType = HeuristicType.Euclidean, Func<Vertex, Vertex, float> costModifier = null)
	{
		var result = FindPath(startId, goalId, heuristicType, costModifier);

		return result?.Path;

	}

	/// <summary>
	/// Wrapper for FindPath that returns the list of Vect2 positions instead of IDs.
	/// </summary>
	/// <param name="startId">The ID of the starting vertex.</param>
	/// <param name="goalId">The ID of the target vertex.</param>
	/// <param name="heuristicType">The heuristic to use for distance calculation.</param>
	/// <param name="costModifier">Optional edge cost modifier function.</param>
	/// <returns>The shortest path as a list of Vect2 positions, or null if no path is found.</returns>
	public List<Vect2> ShortestPathAsPositions(int startId, int goalId, HeuristicType heuristicType = HeuristicType.Euclidean, Func<Vertex, Vertex, float> costModifier = null)
	{
		var result = FindPath(startId, goalId, heuristicType, costModifier);

		if (result == null)
			return null;

		var positions = new List<Vect2>(result.Value.Path.Count);

		foreach (var id in result.Value.Path)
		{
			if (TryGetPosition(id, out var pos))
				positions.Add(pos);
		}

		return positions;
	}


	public bool ArePointsConnected(int fromId, int toId)
	{
		if (!_vertices.TryGetValue(fromId, out var a) || !_vertices.TryGetValue(toId, out var _))
			return false;

		return a.IsConnected(toId);
	}

	/// <summary>
	/// Enables or disables an edge between two connected vertices.
	/// </summary>
	/// <param name="fromId">The ID of the originating vertex.</param>
	/// <param name="toId">The ID of the destination vertex.</param>
	/// <param name="enabled">True to enable the edge; false to disable it.</param>
	/// <returns>True if the edge was found and updated; otherwise, false.</returns>
	public bool SetEdgeEnabled(int fromId, int toId, bool enabled)
	{
		if (!_vertices.TryGetValue(fromId, out var from) || !_vertices.TryGetValue(toId, out var to))
			return false;

		bool updated = false;

		foreach (var edge in from.Edges)
		{
			if (edge.Target.Id == toId)
			{
				edge.IsEnabled = enabled;
				updated = true;
			}
		}

		foreach (var edge in to.Edges)
		{
			if (edge.Target.Id == fromId)
			{
				edge.IsEnabled = enabled;
				updated = true;
			}
		}

		return updated;
	}

	/// <summary>
	/// Activates or deactivates a vertex by its ID. A deactivated vertex will be ignored during pathfinding.
	/// </summary>
	/// <param name="id">The ID of the vertex to modify.</param>
	/// <param name="active">Set to true to enable the vertex; false to disable it.</param>
	/// <returns>True if the vertex exists and was updated; otherwise, false.</returns>
	public bool SetNodeActive(int id, bool active)
	{
		if (!_vertices.TryGetValue(id, out var v))
			return false;
		v.IsActive = active;
		return true;
	}

	/// <summary>
	/// Gets the position of a vertex by its ID.
	/// </summary>
	/// <param name="id">The ID of the vertex.</param>
	/// <param name="position">The position of the vertex if found.</param>
	/// <returns>True if the vertex was found and position was returned; otherwise, false.</returns>
	public bool TryGetPosition(int id, out Vect2 position)
	{
		if (_vertices.TryGetValue(id, out var v))
		{
			position = v.Position;

			return true;
		}

		position = default;

		return false;
	}

	private float GetHeuristic(Vertex a, Vertex b, HeuristicType type)
	{
		var key = (a.Id, b.Id);

		if (_heuristicCache.TryGetValue(key, out var cached))
			return cached;

		float result = type switch
		{
			HeuristicType.Manhattan => MathF.Abs(a.Position.X - b.Position.X) + MathF.Abs(a.Position.Y - b.Position.Y),
			HeuristicType.Diagonal => MathF.Max(MathF.Abs(a.Position.X - b.Position.X), MathF.Abs(a.Position.Y - b.Position.Y)),
			_ => Vect2.Distance(a.Position, b.Position)
		};

		_heuristicCache[key] = result;
		return result;
	}

	private List<int> ReconstructPath(Dictionary<Vertex, Vertex> cameFrom, Vertex current)
	{
		var path = _pathPool.Count > 0 ? _pathPool.Pop() : new List<int>();

		path.Clear();
		path.Add(current.Id);

		while (cameFrom.TryGetValue(current, out current))
			path.Insert(0, current.Id);

		_pathPool.Push(path);

		return new List<int>(path); // return a copy to avoid reuse issues
	}
}
