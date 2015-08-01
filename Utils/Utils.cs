using System;
using System.Collections.Generic;
using System.Globalization;

namespace Utils
{
	public static class Utils
	{
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

		public static string ToEnString(this double num, int decimals = 6)
		{
			return Math.Round(num, decimals).ToString(new CultureInfo("en-us"));
		}
	}
}
