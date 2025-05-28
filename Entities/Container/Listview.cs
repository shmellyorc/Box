namespace Box.Entities.Container;

/// <summary>
/// Represents a base class for creating items in a list view.
/// </summary>
public class ListviewItem : Entity
{
	private BoxColor _barColor = BoxColor.AllShades.Blue;
	private ColorRect _rect;
	private bool _selected;

	internal Listview ListviewEntity;

	/// <summary>
	/// Gets the list view associated with this list view item.
	/// </summary>
	public Listview Listview => ListviewEntity;

	/// <summary>
	/// Gets or sets a value indicating whether the list view item is selected.
	/// </summary>
	public bool Selected
	{
		get => _selected;
		set
		{
			var oldValue = _selected;
			_selected = value;

			if (_selected != oldValue)
			{
				if (_rect is not null)
					_rect.Visible = _selected;
			}
		}
	}

	/// <summary>
	/// Gets or sets the color of the selected list item.
	/// </summary>
	public BoxColor Color
	{
		get { return _barColor; }
		set
		{
			var oldValue = _barColor;
			_barColor = value;

			if (_barColor != oldValue)
			{
				if (_rect is not null)
					_rect.Color = _barColor;
			}
		}
	}

	public ListviewItem()
	{
		KeepAlive = true;
	}

	/// <summary>
	/// Called when the list view item enters a selected state.
	/// </summary>
	protected override void OnEnter()
	{
		AddChild(
			_rect = new ColorRect() { Size = Size, Color = _barColor, Visible = _selected }
		);

		base.OnEnter();
	}
}

/// <summary>
/// Represents a custom list view control entity.
/// </summary>
public class Listview : BoxRenderTarget
{
	private readonly int _maxItems;
	private List<ListviewItem> _children = new();
	// private VPanel _container;
	private int _index, _oldIndex = -1;
	private float _timeout;

	/// <summary>
	/// Determines whether to use deferred rendering for the list view.
	/// </summary>
	public bool UseDiffered { get; set; } = true;

	/// <summary>
	/// Gets the number of child items in the list view container.
	/// </summary>
	// public new int ChildCount => ChildCount; //_container?.ChildCount ?? _children.Length;

	/// <summary>
	/// Gets the index of the currently selected item in the list view.
	/// </summary>
	public int SelectedIndex => _index + ScrollIndex;

	/// <summary>
	/// Gets the currently selected item in the list view.
	/// </summary>
	public ListviewItem SelectedItem => ChildCount == 0 ? default : GetChild<ListviewItem>(SelectedIndex); //_container.GetChild<ListviewItem>(SelectedIndex);

	/// <summary>
	/// Indicates whether the list view is scrolled to the top.
	/// </summary>
	public bool AtTop => ChildCount == 0 || ChildCount < _maxItems || ScrollIndex == 0;

	/// <summary>
	/// Indicates whether the list view is scrolled to the bottom.
	/// </summary>
	public bool AtBottom => ChildCount == 0 || ChildCount < _maxItems || ScrollIndex == Math.Max(ChildCount - _maxItems, 0);

	/// <summary>
	/// Gets the current scroll index of the list view.
	/// </summary>
	public int ScrollIndex { get; private set; }

	/// <summary>
	/// Specifies the delay in seconds before input timeout occurs.
	/// </summary>
	public float InputTimeoutDelay = 0.225f;

	/// <summary>
	/// Retrieves the currently selected list view item cast as type T.
	/// </summary>
	/// <typeparam name="T">The type to cast the selected item to, must derive from ListviewItem.</typeparam>
	/// <returns>The currently selected item cast as type T, or default if no item is selected.</returns>
	public T SelectedItemAs<T>() where T : ListviewItem => (T)SelectedItem;

	/// <summary>
	/// Initializes a new instance of the Listview class with the specified maximum items and child items.
	/// </summary>
	/// <param name="maxItems">The maximum number of items visible in the list view.</param>
	/// <param name="children">Child items to initialize the list view with.</param>
	public Listview(int maxItems, params ListviewItem[] children)
	{
		_maxItems = maxItems;
		_children = new List<ListviewItem>(children);

		Size = new Vect2(AvgSize(_children).X, AvgSize(_children).Y * _maxItems);
	}

	/// <summary>
	/// Called when the list view item enters a selected state.
	/// </summary>
	protected override void OnEnter()
	{
		// base.AddChild(
		// 	_container = new VPanel(spacing: 0) { KeepAlive = true }
		// );

		unsafe
		{
			float offset = 0f;

			fixed (ListviewItem* ptr = _children.ToArray())
			{
				for (int i = 0; i < _children.Count; i++)
				{
					var child = ptr + i;

					child->Selected = i == 0;
					child->ListviewEntity = this;
					child->KeepAlive = true;
					child->LocalPosition = new(0, offset);

					// _container.AddChild(*child);
					AddChild(*child);

					offset += child->Size.Y;
				}
			}
		}

		Connect(EngineSignals.EntityAdded, OnAddOrRemove);
		Connect(EngineSignals.EntityRemoved, OnAddOrRemove);

		// _children = null;
		_children.Clear();
		_isDirty = true;

		base.OnEnter();
	}

	private void OnAddOrRemove(SignalHandle handle)
	{
		var entity = handle.Get<Entity>(0);
		var parent = handle.Get<Entity>(1);

		if (!Children.Any(x => x == entity || x == parent))
			return;

		// Size = new Vect2(AvgSize(_children).X, AvgSize(_children).Y * _maxItems);

		foreach (var item in GetParents<Entity>())
			item._isDirty = true;

		_isDirty = true;
	}

	/// <summary>
	/// Moves selection to the previous item in the list view.
	/// </summary>
	public void PreviousItem()
	{
		// if (_container.ChildCount == 0)
		if (ChildCount == 0)
			return;
		if (_timeout > 0)
			return;

		if (_index > 0)
		{
			if (_index > 0)
				Emit(EngineSignals.ListviewSoundFx, this);

			_index = Math.Max(_index - 1, 0);
			_isDirty = true;
		}
		else
		{
			if (_index > 0)
				Emit(EngineSignals.ListviewSoundFx, this);

			ScrollIndex = Math.Max(ScrollIndex - 1, 0);
			_isDirty = true;
		}
	}

	/// <summary>
	/// Moves selection to the next item in the list view.
	/// </summary>
	public void NextItem()
	{
		// if (_container.ChildCount == 0)
		if (ChildCount == 0)
			return;
		if (_timeout > 0)
			return;

		// if (_container.ChildCount > _maxItems - 1 && _index == _maxItems - 1)
		if (ChildCount > _maxItems - 1 && _index == _maxItems - 1)
		{
			// if (ScrollIndex < _container.ChildCount - _maxItems)
			if (ScrollIndex < ChildCount - _maxItems)
				Emit(EngineSignals.ListviewSoundFx, this);

			// ScrollIndex = Math.Min(ScrollIndex + 1, _container.ChildCount - _maxItems);
			ScrollIndex = Math.Min(ScrollIndex + 1, ChildCount - _maxItems);

			_isDirty = true;
		}
		else
		{
			// if (_index < (_container.ChildCount > _maxItems - 1 ? _maxItems : _container.ChildCount - 1))
			if (_index < (ChildCount > _maxItems - 1 ? _maxItems : ChildCount - 1))
				Emit(EngineSignals.ListviewSoundFx, this);

			// _index = Math.Min(_index + 1, _container.ChildCount > _maxItems - 1 ? _maxItems : _container.ChildCount - 1);
			_index = Math.Min(_index + 1, ChildCount > _maxItems - 1 ? _maxItems : ChildCount - 1);

			_isDirty = true;
		}
	}

	/// <summary>
	/// Updates the state and rendering of the list view.
	/// </summary>
	protected override void Update()
	{
		if (_timeout > 0)
			_timeout -= BE.Clock.DeltaTime;

		if (_isDirty)
		{
			UpdateItems();

			_isDirty = false;
		}

		base.Update();
	}

	private void UpdateItems(bool updateDelayed = true)
	{
		// if (_container.ChildCount == 0)
		if (ChildCount == 0)
		{
			if (UseDiffered)
				Emit(EngineSignals.ListviewSelected, this, null, -1);
			else
				EmitDelayed(BE.Clock.DeltaTime, EngineSignals.ListviewSelected, this, null, -1);

			return;
		}

		float offset = 0f;

		// if (ScrollIndex > 0)
		// 	ScrollIndex = Math.Clamp(ScrollIndex, 0, _container.ChildCount - _maxItems);
		// else
		// 	_index = Math.Clamp(_index, 0, _container.ChildCount > _maxItems - 1 ? _maxItems : _container.ChildCount - 1);
		if (ScrollIndex > 0)
			ScrollIndex = Math.Clamp(ScrollIndex, 0, ChildCount - _maxItems);
		else
			_index = Math.Clamp(_index, 0, ChildCount > _maxItems - 1 ? _maxItems : ChildCount - 1);

		// for (int i = 0; i < _container.ChildCount; i++)
		// {
		// 	var child = _container.GetChild<ListviewItem>(i);

		// 	child.Selected = i == _index + ScrollIndex;

		// 	if (child.Selected)
		// 	{
		// 		if (_oldIndex != i)
		// 		{
		// 			if (UseDiffered)
		// 				Emit(EngineSignals.ListviewSelected, this, child, i);
		// 			else
		// 				EmitDelayed(Clock.DeltaTime, EngineSignals.ListviewSelected, this, child, i);

		// 			_oldIndex = i;
		// 		}
		// 	}
		// }
		for (int i = 0; i < ChildCount; i++)
		{
			var child = GetChild<ListviewItem>(i);

			child.Selected = i == _index + ScrollIndex;
			child.Position = new(0, offset);

			if (child.Selected)
			{
				if (_oldIndex != i)
				{
					if (UseDiffered)
						Emit(EngineSignals.ListviewSelected, this, child, i);
					else
						EmitDelayed(BE.Clock.DeltaTime, EngineSignals.ListviewSelected, this, child, i);

					_oldIndex = i;
				}
			}

			offset += child.Size.Y;
		}

		if (updateDelayed)
			_timeout += InputTimeoutDelay;

		StartRoutine(SelectorWaitForRendererRoutine());
	}






	#region Entity (Override)
	/// <summary>
	/// Adds a child entity to the list view.
	/// </summary>
	/// <param name="entity">The entity to add as a child.</param>
	public new void AddChild(Entity entity)
	{
		AddChild(entities: entity);
	}

	/// <summary>
	/// Adds multiple child entities to the list view.
	/// </summary>
	/// <param name="entities">The entities to add as children.</param>
	public new void AddChild(params Entity[] entities)
	{
		// _container.AddChild(entities);
		base.AddChild(entities);

		foreach (var item in GetParents<Entity>())
			item._isDirty = true;

		_oldIndex = -1; // force to update selection

		_isDirty = true;
	}

	/// <summary>
	/// Adds a child entity of type T to the list view.
	/// </summary>
	/// <typeparam name="T">The type of child entity to add.</typeparam>
	/// <param name="entity">The entity to add as a child.</param>
	/// <returns>The added child entity cast to type T.</returns>
	public new T AddChild<T>(params Entity[] entity) where T : Listview
	{
		AddChild(entity);

		_isDirty = true;

		return (T)this;
	}

	// /// <summary>
	// /// Inserts a child entity into the list view at the specified index.
	// /// </summary>
	// /// <param name="index">The zero-based index at which the entity should be inserted.</param>
	// /// <param name="entity">The entity to insert as a child.</param>
	// public new void InsertChild(int index, Entity entity)
	// {
	// 	// _container.InsertChild(index, entity);
	// 	base.InsertChild(index, entity);

	// 	_isDirty = true;
	// }

	// /// <summary>
	// /// Gets a child entity at the specified index from the list view.
	// /// </summary>
	// /// <typeparam name="T">The type of entity to retrieve.</typeparam>
	// /// <param name="index">The zero-based index of the child entity.</param>
	// /// <returns>The child entity at the specified index, cast to type T.</returns>
	// public new T GetChild<T>(int index) where T : Entity
	// {
	// 	// return _container.GetChild<T>(index);
	// 	return GetChild<T>(index);
	// }

	// /// <summary>
	// /// Checks if the list view has a child entity of the specified type.
	// /// </summary>
	// /// <typeparam name="T">The type of entity to check for.</typeparam>
	// /// <returns>True if a child of the specified type exists; otherwise, false.</returns>
	// public new bool HasChild<T>() where T : Entity
	// {
	// 	// return _container.HasChild<T>();
	// 	return HasChild<T>();
	// }

	// /// <summary>
	// /// Checks if the list view contains the specified child entity.
	// /// </summary>
	// /// <param name="entity">The entity to search for.</param>
	// /// <returns>True if the entity is a child of the list view; otherwise, false.</returns>
	// public new bool HasChild(Entity entity)
	// {
	// 	// return _container.HasChild(entity);
	// 	return HasChild(entity);
	// }

	/// <summary>
	/// Removes the specified child entity from the list view.
	/// </summary>
	/// <param name="entity">The entity to remove from the list view.</param>
	/// <returns>True if the entity was successfully removed; otherwise, false.</returns>
	public new bool RemoveChild(Entity entity)
	{
		var result = base.RemoveChild(entities: entity);

		if (result)
			StartRoutine(WaitForDeletionRoutine(entity));

		return result;
	}

	/// <summary>
	/// Removes multiple child entities from the list view.
	/// </summary>
	/// <param name="entities">The entities to remove from the list view.</param>
	/// <returns>True if all specified entities were successfully removed; otherwise, false.</returns>
	public new bool RemoveChild(params Entity[] entities)
	{
		// var result = _container.RemoveChild(entities);
		var result = RemoveChild(entities);

		if (result)
			StartRoutine(WaitForDeletionRoutine(entities));

		return result;
	}

	/// <summary>
	/// Removes all child entities from the list view.
	/// </summary>
	public new void ClearChildren()
	{
		// if (_children is not null)
		// 	_children = new Entity[0];
		if (_children.Count > 0)
			_children.Clear();

		if (!Children.Any())
			return;

		// _container.ClearChildren();
		base.ClearChildren();

		_oldIndex = -1; // force to update selection
		_index = 0;
		ScrollIndex = 0;

		_isDirty = true;
	}
	#endregion

	#region Routines
	private IEnumerator SelectorWaitForRendererRoutine()
	{
		while (IsRendering)
			yield return null;

		// Offset = new Vect2(0, ScrollIndex * AvgSize(_container.Children).Y);
		Offset = new Vect2(0, ScrollIndex * AvgSize(Children).Y);
	}

	private IEnumerator WaitForDeletionRoutine(params Entity[] entities)
	{
		for (int i = 0; i < entities.Length; i++)
		{
			// if (_container.HasChild(entities[i]))
			// {
			// 	while (_container.HasChild(entities[i]))
			// 		yield return null;
			// }
			if (HasChild(entities[i]))
			{
				while (HasChild(entities[i]))
					yield return null;
			}

			// change old index to anything to force it to update the last selected item
			_oldIndex = int.MaxValue;
		}

		foreach (var item in GetParents<Entity>())
			item._isDirty = true;

		_isDirty = true;
	}

	internal Vect2 AvgSize(IEnumerable<Entity> items)
		=> items is not null && items.Any() ? items.Max(x => x.Size) : Vect2.Zero;
	#endregion
}
