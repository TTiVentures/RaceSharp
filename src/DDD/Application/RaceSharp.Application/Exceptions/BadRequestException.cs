using System;

namespace RaceSharp.Application
{
	public class BadRequestException : Exception
	{
		public BadRequestException(string message)
			: base(message)
		{
		}
	}
}