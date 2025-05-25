using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System;

public static class EntityExtentions
{
	// Generic Settings (All):
	public static T SetPosition<T>(this T e, Vect2 position) where T : Entity
	{
		e.Position = position;

		return e;
	}
	public static T SetPosition<T>(this T e, float x, float y) where T : Entity
	{
		e.Position = new Vect2(x, y); ;

		return e;
	}
	public static T SetSize<T>(this T e, Vect2 size) where T : Entity
	{
		e.Size = size;

		return e;
	}
	public static T SetLayer<T>(this T e, int layer) where T : Entity
	{
		e.Layer = layer;

		return e;
	}
	public static T SetVisible<T>(this T e, bool visible) where T : Entity
	{
		e.Visible = visible;

		return e;
	}
	public static T SetKeepAlive<T>(this T e, bool keepAlive) where T : Entity
	{
		e.KeepAlive = keepAlive;

		return e;
	}

	// Label Settings:
	public static T SetText<T>(this T e, string text) where T : Label
	{
		e.Text = text;

		return e;
	}
	public static T SetHAlign<T>(this T e, HAlign align) where T : Label
	{
		e.HAlign = align;

		return e;
	}
	public static T SetVAlign<T>(this T e, VAlign align) where T : Label
	{
		e.VAlign = align;

		return e;
	}



	// ColorRect
	public static T SetColor<T>(this T e, BoxColor color) where T : ColorRect
	{
		e.Color = color;

		return e;
	}
}
