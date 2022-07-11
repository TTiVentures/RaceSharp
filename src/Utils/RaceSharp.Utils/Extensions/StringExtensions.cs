using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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


		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;
			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		public static string PascalToKebabCase(this string value, char separator = '-')
		{
			if (string.IsNullOrWhiteSpace(value)) return value;

			return Regex.Replace(
							value,
							"(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
							$"{separator}$1",
							RegexOptions.Compiled
						)
						.Trim()
						.ToLower();
		}
	}
}