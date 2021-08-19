using System;

namespace RaceSharp.Application
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string name, object key)
			: base($"Entity '{name}' ({key}) was not found.")
		{
		}
	}
}