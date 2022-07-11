using System.Linq;

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
	}
}
