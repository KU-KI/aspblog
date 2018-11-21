using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspBlog.Helpers
{
    public class StringHelper
    {
        public static string Slugify(string text)
        {
            string str = RemoveDiacritics(text.ToLower());
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", " ");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveDiacritics(string text)
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

        public static string WordLimiter(string text, int word_count)
        {
            if (text == null || text.Length < word_count || text.IndexOf(" ", word_count) == -1)
                return text;

            return text.Substring(0, text.IndexOf(" ", word_count));
        }
    }
}
