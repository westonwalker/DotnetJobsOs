using Sprache;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetJobs.Application.Helpers
{
	public static class StringHelpers
	{
		public static string Slugify(string str)
		{
			// Remove all accents and make the string lower case.  
			string output = str.RemoveAccents().ToLower();

			// Remove all special characters from the string.  
			output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

			// Remove all additional spaces in favour of just one.  
			output = Regex.Replace(output, @"\s+", " ").Trim();

			// Replace all spaces with the hyphen.  
			output = Regex.Replace(output, @"\s", "-");

			// Return the slug.  
			return output;
		}

		public static string RemoveAccents(this string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return text;

			text = text.Normalize(NormalizationForm.FormD);
			char[] chars = text
				.Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
				!= UnicodeCategory.NonSpacingMark).ToArray();

			return new string(chars).Normalize(NormalizationForm.FormC);
		}

		public static string RemoveSubstrings(this string input, params string[] substringsToRemove)
		{
            foreach (string substring in substringsToRemove)
            {
                input = Regex.Replace(input, substring, string.Empty, RegexOptions.IgnoreCase);
            }
            return input;
        }
	}
}
