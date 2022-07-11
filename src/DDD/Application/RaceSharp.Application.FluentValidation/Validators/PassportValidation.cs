using System.Text.RegularExpressions;

namespace RaceSharp.Application.FluentValidation.Validators
{
	public class PassportValidation
	{
		/// <summary>
		/// Check if the value meets the validation rules or not
		/// </summary>
		/// <param name="value">Value to check</param>
		/// <param name="validationContext">Describe the context in which the test is carried out.</param>
		/// <returns></returns>
		public static bool IsValid(string passport)
		{
			Match match = Regex.Match(passport, "^(?!^0+$)[a-zA-Z0-9]{3,20}$", RegexOptions.None);

			return match.Success;
		}
	}
}