namespace RaceSharp.Application
{
	public interface IMapFromTo<TOrigin, TDestination>
		where TDestination : IMapFromTo<TOrigin, TDestination>
	{
		public TDestination MapFrom(TOrigin origin);
	}
}