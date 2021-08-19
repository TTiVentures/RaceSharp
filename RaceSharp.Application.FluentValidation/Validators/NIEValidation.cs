namespace RaceSharp.Application.FluentValidation.Validators
{
	public class NIEValidation
	{
		private const string CORRESPONDENCY = "TRWAGMYFPDXBNJZSQVHLCKE";

		/// <summary>
		/// Check if the value meets the validation rules or not
		/// </summary>
		/// <param name="value">Value to check</param>
		/// <param name="validationContext">Describe the context in which the test is carried out.</param>
		/// <returns></returns>
		public static bool IsValid(string nie)
		{
			// Change the initial letter for the corresponding number and validate as DNI
			var nie_prefix = nie[0];

			switch (nie_prefix)
			{
				case 'X': nie_prefix = '0'; break;
				case 'Y': nie_prefix = '1'; break;
				case 'Z': nie_prefix = '2'; break;
			}

			return DNIValidation.IsValid(nie_prefix + nie[1..]);
		}
	}
}