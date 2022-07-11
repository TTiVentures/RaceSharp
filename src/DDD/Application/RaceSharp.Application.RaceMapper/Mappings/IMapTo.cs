namespace RaceSharp.Application
{
	public interface IMapTo<T>
	{
		public T MapTo(IMapTo<T> origin);
	}
}