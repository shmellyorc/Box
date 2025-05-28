namespace Box.Helpers;

/// <summary>
/// Provides utility methods for aligning elements based on width, height, and viewport.
/// </summary>
public static class AlignmentHelpers
{
	/// <summary>
	/// Aligns the child width relative to the parent width using the specified horizontal alignment and offset.
	/// </summary>
	/// <param name="parent">The width of the parent.</param>
	/// <param name="child">The width of the child.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="offset">Additional horizontal offset to apply.</param>
	/// <returns>The calculated horizontal position for alignment.</returns>
	public static float AlignWidth(float parent, float child, HAlign hAlign, float offset)
	{
		return hAlign switch
		{
			HAlign.Center => Vect2.Center(parent, child, true) + offset,
			HAlign.Right => parent - child + offset,
			_ => 0 + offset,
		};
	}

	/// <summary>
	/// Aligns the child width relative to the parent width using the specified horizontal alignment.
	/// </summary>
	/// <param name="parent">The width of the parent.</param>
	/// <param name="child">The width of the child.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <returns>The calculated horizontal position for alignment.</returns>
	public static float AlignWidth(float parent, float child, HAlign hAlign)
		=> AlignWidth(parent, child, hAlign, 0f);



	/// <summary>
	/// Aligns the child height relative to the parent height using the specified vertical alignment and offset.
	/// </summary>
	/// <param name="parent">The height of the parent.</param>
	/// <param name="child">The height of the child.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <param name="offset">Additional vertical offset to apply.</param>
	/// <returns>The calculated vertical position for alignment.</returns>
	public static float AlignHeight(float parent, float child, VAlign vAlign, float offset)
	{
		return vAlign switch
		{
			VAlign.Center => Vect2.Center(parent, child, true) + offset,
			VAlign.Bottom => parent - child + offset,
			_ => 0 + offset,
		};
	}

	/// <summary>
	/// Aligns the child height relative to the parent height using the specified vertical alignment.
	/// </summary>
	/// <param name="parent">The height of the parent.</param>
	/// <param name="child">The height of the child.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>The calculated vertical position for alignment.</returns>
	public static float AlignHeight(float parent, float child, VAlign vAlign)
		=> AlignHeight(parent, child, vAlign, 0f);



	/// <summary>
	/// Aligns the position of the entity relative to the viewport (Renderer), based on 
	/// the specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="entity">The entity to align.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <param name="offset">Additional offset to apply.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static Vect2 AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign, Vect2 offset)
	{
		if (entity is null)
			return Vect2.Zero;

		var v = EngineSettings.Instance.Viewport;
		var x = AlignWidth(v.X, entity.Size.X, hAlign);
		var y = AlignHeight(v.Y, entity.Size.Y, vAlign);

		return new Vect2(x, y) + offset;
	}

	/// <summary>
	/// Aligns the position of the entity relative to the viewport (Renderer), based on the 
	/// specified vertical and horizontal alignments, with an optional vector offset.
	/// </summary>
	/// <param name="entity">The entity to align.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static Vect2 AlignToRenderer(Entity entity, HAlign hAlign, VAlign vAlign)
		=> AlignToRenderer(entity, hAlign, vAlign, Vect2.Zero);




	/// <summary>
	/// Aligns a child entity relative to a parent entity using the specified alignment and offset.
	/// </summary>
	/// <param name="parent">The parent entity.</param>
	/// <param name="child">The child entity.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <param name="offset">Additional offset to apply.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static Vect2 AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign, Vect2 offset)
	{
		if (parent is null || child is null)
			return Vect2.Zero;

		var x = AlignWidth(parent.Size.X, child.Size.X, hAlign);
		var y = AlignHeight(parent.Size.Y, child.Size.Y, vAlign);

		return new Vect2(x, y) + offset;
	}

	/// <summary>
	/// Aligns a child entity relative to a parent entity using the specified alignment.
	/// </summary>
	/// <param name="parent">The parent entity.</param>
	/// <param name="child">The child entity.</param>
	/// <param name="hAlign">The horizontal alignment type.</param>
	/// <param name="vAlign">The vertical alignment type.</param>
	/// <returns>A vector representing the aligned position.</returns>
	public static Vect2 AlignToEntity(Entity parent, Entity child, HAlign hAlign, VAlign vAlign)
		=> AlignToEntity(parent, child, hAlign, vAlign, Vect2.Zero);
}
