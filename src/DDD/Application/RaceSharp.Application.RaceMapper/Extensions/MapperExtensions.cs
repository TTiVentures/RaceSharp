namespace RaceSharp.Application
{
	public static class MapperExtensions
	{
		public static T MapTo<T>(this IMapTo<T> origin)
		{
			return origin.MapTo(origin);
		}
	}
}
