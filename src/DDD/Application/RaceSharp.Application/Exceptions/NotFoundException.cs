﻿using System;

namespace RaceSharp.Application
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string name, object key)
			: base($"Entity '{name}' (with key: {key}) was not found.")
		{
		}

		public NotFoundException(string name)
			: base($"Entity '{name}' was not found.")
		{
		}
	}
}