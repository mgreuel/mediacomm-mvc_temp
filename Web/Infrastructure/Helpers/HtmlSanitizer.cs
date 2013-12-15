using System.Text.RegularExpressions;

namespace MediaCommMvc.Web.Infrastructure.Helpers
{
    /// <summary>
    ///     By Jeff Atwoood http://refactormycode.com/codes/333-sanitize-html
    /// </summary>
    public static class HtmlSanitizer
    {
        private static readonly Regex ImageWhiteList =
            new Regex(
                @"^<img\ssrc=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""(\swidth=""\d{1,3}"")?(\sheight=""\d{1,3}"")?(\salt=""[^""<>]*"")?(\stitle=""[^""<>]*"")?\s?/?>$", 
                RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex LinkWhiteList =
            new Regex(
                @"^<a\shref=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""(\stitle=""[^""<>]+"")?\s?>$|^</a>$", 
                RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex Tags = new Regex(
            "<[^>]*(>|$)", 
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex Whitelist =
            new Regex(
                @"^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|^<(b|h)r\s?/?>$", 
                RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        ///     sanitize any potentially dangerous tags from the provided raw HTML input using
        ///     a whitelist based approach, leaving the "safe" HTML tags
        ///     CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public static string Sanitize(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            string tagname;
            Match tag;

            MatchCollection tags = Tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(Whitelist.IsMatch(tagname) || LinkWhiteList.IsMatch(tagname) || ImageWhiteList.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                }
            }

            return html;
        }
    }
}