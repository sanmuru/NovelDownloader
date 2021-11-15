using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.RegularExpressions
{
    public class Wildcard
    {
        protected internal static readonly Regex regPattern = new Regex(@"[.$^{[(|*+?\\]", RegexOptions.Compiled);

        protected readonly Regex regex;

        public Wildcard(string pattern) : this(pattern, WildcardOptions.None) { }

        public Wildcard(string pattern, WildcardOptions options)
        {
            if (pattern is null) throw new ArgumentNullException(nameof(pattern));

            this.regex = new Regex(Wildcard.ToRegexPattern(pattern), Wildcard.ToRegexOptions(options));
        }

        public Wildcard(string pattern, WildcardOptions options, TimeSpan matchTimeout)
        {
            if (pattern is null) throw new ArgumentNullException(nameof(pattern));

            this.regex = new Regex(Wildcard.ToRegexPattern(pattern), Wildcard.ToRegexOptions(options), matchTimeout);
        }

        public bool IsMatch(string input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            return this.regex.IsMatch(input);
        }

        public static bool IsMatch(string input, string pattern) => Wildcard.IsMatch(input, pattern, WildcardOptions.None);

        public static bool IsMatch(string input, string pattern, WildcardOptions options) =>
            new Wildcard(pattern, options).IsMatch(input);

        public static bool IsMatch(string input, string pattern, WildcardOptions options, TimeSpan matchTimeout) =>
            new Wildcard(pattern, options, matchTimeout).IsMatch(input);

        protected internal static string ToRegexPattern(string pattern)
        {
            return '^' + Wildcard.regPattern.Replace(pattern,
                m => {
                    switch (m.Value) {
                        case "?":
                            return ".?";
                        case "*":
                            return ".*";
                        default:
                            return "\\" + m.Value;
                    }
                }) + "$";
        }

        protected internal static RegexOptions ToRegexOptions(WildcardOptions options)
        {
            RegexOptions roptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            if (options == WildcardOptions.None) return roptions;

            if (options.HasFlag(WildcardOptions.Compiled)) roptions |= RegexOptions.Compiled;
            if (options.HasFlag(WildcardOptions.RightToLeft)) roptions |= RegexOptions.RightToLeft;
            if (options.HasFlag(WildcardOptions.CultureInvariant)) roptions |= RegexOptions.CultureInvariant;

            return roptions;
        }
    }
}
