using System;

namespace RaceSharp.Application
{
	public class ConflictException : Exception
	{
		public ConflictException(string name, object key)
			: base($"There is a conflict with entity \"{name}\", the key ({key}) must be unique.")
		{
		}
	}
}