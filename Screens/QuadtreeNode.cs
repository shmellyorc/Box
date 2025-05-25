namespace Box.Screens;

public class QuadtreeNode<T> where T : Entity
{
	private readonly Rect2 _bounds;
	private readonly int _capacity;
	private readonly List<T> _entities = new();
	private QuadtreeNode<T>[] _children;

	public QuadtreeNode(Rect2 bounds, int capacity = 8)
	{
		_bounds = bounds;
		_capacity = capacity;
	}

	public void Insert(T e)
	{
		if (!_bounds.Contains(e.Position))
			return;

		// if leaf and under capacity, stash here
		if (_children == null && _entities.Count < _capacity)
		{
			_entities.Add(e);
			return;
		}

		// otherwise subdivide (if needed) and delegate
		if (_children == null)
			Subdivide();

		foreach (var child in _children)
			child.Insert(e);
	}

	private void Subdivide()
	{
		_children = new QuadtreeNode<T>[4];
		float hw = _bounds.Width / 2;
		float hh = _bounds.Height / 2;
		_children[0] = new QuadtreeNode<T>(new Rect2(_bounds.X, _bounds.Y, hw, hh), _capacity);
		_children[1] = new QuadtreeNode<T>(new Rect2(_bounds.X + hw, _bounds.Y, hw, hh), _capacity);
		_children[2] = new QuadtreeNode<T>(new Rect2(_bounds.X, _bounds.Y + hh, hw, hh), _capacity);
		_children[3] = new QuadtreeNode<T>(new Rect2(_bounds.X + hw, _bounds.Y + hh, hw, hh), _capacity);

		// re-distribute existing
		foreach (var e in _entities)
			foreach (var child in _children)
				child.Insert(e);
		_entities.Clear();
	}

	public bool Remove(T e)
	{
		// if entity not even in this bounds, skip
		if (!_bounds.Contains(e.Position))
			return false;

		// try remove from this node
		if (_entities.Remove(e))
			return true;

		// otherwise, attempt in children
		if (_children != null)
		{
			for (int i = 0; i < 4; i++)
				if (_children[i].Remove(e))
					return true;
		}

		return false;
	}

	public void Clear()
	{
		_entities.Clear();
		if (_children != null)
		{
			foreach (var child in _children)
				child.Clear();
			_children = null;
		}
	}

	public void Update(T e, Rect2 oldPos)
	{
		// remove from old spot, then re-insert at new
		if (Remove(e))
			Insert(e);
		else
		{
			// fallback: if bounds shifted, might need full rebuild
			// (or track parent nodes separately)
		}
	}

	public void Query(Rect2 range, List<T> found)
	{
		if (!_bounds.Intersects(range))
			return;

		// any in this node?
		foreach (var e in _entities)
			if (range.Contains(e.Position))
				found.Add(e);

		// descend
		if (_children != null)
			foreach (var child in _children)
				child.Query(range, found);
	}
}
