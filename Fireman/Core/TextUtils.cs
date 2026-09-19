using System.Globalization;
using System.Text;

namespace Fireman.Core;

public static class TextUtils {
    private const char ReplacementChar = '\uFFFD';

    /// <summary>
    /// Sanitizes strings so TextMeshProUGUI doesn't kill itself
    /// </summary>
    /// <param name="input">The string</param>
    /// <returns>A string safe for displaying</returns>
    public static string SanitizeForDisplay(string? input) {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var sb = new StringBuilder(input.Length);

        for (int i = 0; i < input.Length; i++) {
            char c = input[i];

            // Normalize dashes (en, em, horizontal bar, etc.)
            if (c is '\u2013' or '\u2014' or '\u2015' or '\u2012' or '\u2010' or '\u2011' or '\u2212') {
                sb.Append('-');
                continue;
            }

            // Exclude control chars etc
            UnicodeCategory category = char.GetUnicodeCategory(c);
            if (category == UnicodeCategory.Control || category == UnicodeCategory.OtherNotAssigned) {
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}
