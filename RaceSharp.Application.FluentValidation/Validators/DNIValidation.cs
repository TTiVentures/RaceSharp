using System;

namespace RaceSharp.Application.FluentValidation.Validators
{
	public partial class DNIValidation
	{
		private const string CORRESPONDENCY = "TRWAGMYFPDXBNJZSQVHLCKE";

		/// <summary>
		/// Check if the value meets the validation rules or not
		/// </summary>
		/// <param name="value">Value to check</param>
		/// <param name="validationContext">Describe the context in which the test is carried out.</param>
		/// <returns></returns>
		public static bool IsValid(string dni)
		{
			try
			{
				if (dni.Length != 9)
				{
					return false;
				}

				var letter = CORRESPONDENCY[Int16.Parse(dni.Substring(0, 7)) % 23];
				return letter == dni[8];
			}
			catch
			{
				return false;
			}
		}
	}
}