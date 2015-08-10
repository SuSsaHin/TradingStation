using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Utils
{
	public static class Tools
	{
		private static T Cast<T>(dynamic casted)
		{
			return (T)casted;
		}

		public static dynamic DynamicCast(this Type castType, object casted)
		{
			return typeof(Tools).GetMethod("Cast", BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(castType).Invoke(null, new [] { casted });
		}

		public static LinkedListNode<T> RemoveFromList<T>(this LinkedListNode<T> element)
		{
			return element.List.RemoveElement(element);
		}

		public static LinkedListNode<T> RemoveElement<T>(this LinkedList<T> list, LinkedListNode<T> element)
		{
			var next = element.Next;
			list.Remove(element);
			return next;
		}

		public static LinkedListNode<T> RemoveFront<T>(this LinkedList<T> list, LinkedListNode<T> element)
		{
			while (list.First != null && list.First != element)
			{
				list.RemoveFirst();
			}

			if (list.First != null)
				return list.RemoveElement(element);

			return null;
		}

		public static string ToEnString(this double num, int decimals = 3)
		{
			return Math.Round(num, decimals).ToString(new CultureInfo("en-us"));
		}
	}
}
