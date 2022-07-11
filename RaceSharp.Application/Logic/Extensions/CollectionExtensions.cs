using System;
using System.Collections.Generic;

namespace RaceSharp.Application
{
	public static class CollectionExtensions
	{
		public static void ForEach<T>(this ICollection<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}
	}
}