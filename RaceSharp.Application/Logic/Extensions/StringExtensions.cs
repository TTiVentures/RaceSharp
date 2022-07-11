using System.Globalization;
using System.Text;

namespace RaceSharp.Application
{
	public static class StringExtensions
	{
		/// <summary>
		/// Text to upper and removed diacritics
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string Flat(this string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text.Trim().RemoveDiacritics().ToUpper();
			}
			return text;
		}

		/// <summary>
		/// Removes diacritics from text, for example á -> a
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveDiacritics(this string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				var normalizedString = text.Normalize(NormalizationForm.FormD);
				var stringBuilder = new StringBuilder();

				foreach (var c in normalizedString)
				{
					var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
					if (unicodeCategory != UnicodeCategory.NonSpacingMark)
					{
						stringBuilder.Append(c);
					}
				}

				return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
			}
			return string.Empty;
		}
	}
}