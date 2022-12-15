using System;
using System.Linq;
using System.Linq.Expressions;

namespace RaceSharp.Application
{
	public static class DbContextExtensions
	{
		public static T FirstOrNotFound<T>(this IQueryable<T> query)
		{
			var temp = query.FirstOrDefault();
			if (temp is null)
			{
				throw new NotFoundException(typeof(T).Name);
			}
			return temp;
		}		
		public static T FirstOrNotFound<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			var temp = query.FirstOrDefault(predicate);
			if (temp is null)
			{
				throw new NotFoundException(typeof(T).Name);
			}
			return temp;
		}
	}
}
