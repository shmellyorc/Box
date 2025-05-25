namespace Box.Entities.Container;

/// <summary>
/// Represents a generic panel that serves as a container for UI elements.
/// </summary>
public class Panel : Entity
{
	private Entity[] _children;

	/// <summary>
	/// Gets or sets a value indicating whether the panel is in a dirty state,
	/// indicating that its content or layout has changed and needs updating.
	/// </summary>
	public bool IsDirty
	{
		get => _isDirty;
		set => _isDirty = value;
	}

	/// <summary>
	/// Initializes a new instance of the Panel class with the specified child entities.
	/// </summary>
	/// <param name="children">The child entities to be added to the panel.</param>
	public Panel(params Entity[] children) => _children = children;

	/// <summary>
	/// Called when the panel enters the active state.
	/// </summary>
	protected override void OnEnter()
	{
		AddChild(_children);

		Connect(EngineSignals.EntityAdded, OnAddOrRemove);
		Connect(EngineSignals.EntityRemoved, OnAddOrRemove);

		_children = null;

		base.OnEnter();
	}

	private void OnAddOrRemove(SignalHandle handle)
	{
		if (Screen.IsExiting)
			return;

		var entity = handle.Get<Entity>(0);
		var parent = handle.Get<Entity>(1);

		if (!Children.Any(x => x == entity || x == parent))
			return;

		foreach (var item in GetParents<Entity>())
		{
			if (item is Panel panel)
				panel.IsDirty = true;
			else
				item._isDirty = true;
		}

		IsDirty = true;
	}

	/// <summary>
	/// Updates the panel's state and behavior.
	/// </summary>
	protected override void Update()
	{
		if (IsDirty)
		{
			UpdateDirtyState();

			IsDirty = false;
		}

		if (Engine.GetService<EngineSettings>().DebugDraw)
			Renderer.DrawRectangleOutline(Position.X, Position.Y, Size.X, Size.Y, 1f,  BoxColor.AllShades.Red);

		base.Update();
	}

	/// <summary>
	/// Updates the dirty state of the panel.
	/// </summary>
	protected virtual void UpdateDirtyState() { }

	#region Entity (Override)
	/// <summary>
	/// Adds children entity to the panel.
	/// </summary>
	/// <param name="entity">Entities to add as children.</param>
	public new void AddChild(Entity entity) => AddChild(children: entity);

	/// <summary>
	/// Adds children entities to the panel.
	/// </summary>
	/// <param name="children">Entities to add as children.</param>
	public new void AddChild(params Entity[] children)
	{
		base.AddChild(children);

		foreach (var item in GetParents<Entity>())
		{
			if (item is Panel panel)
				panel.IsDirty = true;
			else
				item._isDirty = true;
		}

		IsDirty = true;
	}

	// /// <summary>
	// /// Adds children entities to the panel and returns the added child panel of type T.
	// /// </summary>
	// /// <typeparam name="T">Type of panel to add.</typeparam>
	// /// <param name="children">Entities to add as children.</param>
	// /// <returns>The added child panel of type T.</returns>
	// public new T AddChild<T>(params Entity[] children) where T : Panel
	// {
	// 	AddChild(children);

	// 	return (T)this;
	// }

	// /// <summary>
	// /// Inserts an entity at the specified index in the panel's children collection.
	// /// </summary>
	// /// <param name="index">The index at which to insert the entity.</param>
	// /// <param name="entity">The entity to insert.</param>
	// public new void InsertChild(int index, Entity entity)
	// {
	// 	base.InsertChild(index, entity);

	// 	foreach (var item in GetParents<Entity>())
	// 	{
	// 		if (item is Panel panel)
	// 			panel.IsDirty = true;
	// 		else
	// 			item._isDirty = true;
	// 	}

	// 	IsDirty = true;
	// }

	/// <summary>
	/// Removes the specified entity from the panel's children collection.
	/// </summary>
	/// <param name="entity">The entity to remove.</param>
	/// <returns><c>true</c> if the entity was successfully removed; otherwise, <c>false</c>.</returns>
	public new bool RemoveChild(Entity entity) => RemoveChild(children: entity);

	/// <summary>
	/// Removes the specified entities from the panel's children collection.
	/// </summary>
	/// <param name="children">The entities to remove.</param>
	/// <returns><c>true</c> if all entities were successfully removed; otherwise, <c>false</c>.</returns>
	public new bool RemoveChild(params Entity[] children)
	{
		var result = base.RemoveChild(children);

		foreach (var item in GetParents<Entity>())
		{
			if (item is Panel panel)
				panel.IsDirty = true;
			else
				item._isDirty = true;
		}

		IsDirty = true;

		return result;
	}

	/// <summary>
	/// Clears all children from the panel.
	/// </summary>
	public new unsafe void ClearChildren()
	{
		if (_children is not null)
			_children = null;

		if (!Children.Any())
			return;

		base.ClearChildren();

		IsDirty = true;
	}
	#endregion
}
