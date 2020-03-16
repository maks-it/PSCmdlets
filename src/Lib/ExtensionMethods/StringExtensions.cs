using System.Text.RegularExpressions;

namespace Lib.ExtensionMethods
{
    /*
        Main StringExtensions class
        Extends String class with additional methods
    */
    /// <summary>
    /// Main <c>StringExtensions</c> class.
    /// <list type="bullet">
    /// <item>
    /// <term>Like</term>
    /// <description>SQL Like implementation</description>
    /// </item>
    /// <item>
    /// <term>Left</term>
    /// <description>VB.Net Left implementation</description>
    /// </item>
    /// <item>
    /// <term>Right</term>
    /// <description>VB.Net Right implementation</description>
    /// </item>
    /// <item>
    /// <term>Mid</term>
    /// <description>VB.Net Rid implementation</description>
    /// </item>
    /// <item>
    /// <term>ToInteger</term>
    /// <description>VB.Net ToInteger implementation</description>
    /// </item>
    /// <item>
    /// <term>IsInteger</term>
    /// <description>VB.Net IsInteger implementation</description>
    /// </item>
    /// </list>
    /// </summary>
    public static class StringExtensions
    {
        // SQL Like implementation
        /// <summary>
        /// SQL Like implementation.
        /// </summary>
        /// <example>
        /// <code>
        /// string test = "My String";
        /// bool result = test.Like("*string");
        /// </code>
        /// </example>
        /// <seealso cref="WildCardToRegular"/>
        /// <param name="text"></param>
        /// <param name="wildcardedText"></param>
        /// <returns></returns>
        public static bool Like(this string text, string wildcardedText)
        {
            return Regex.IsMatch(text.ToLower(), WildCardToRegular(wildcardedText.ToLower()));
        }

        // Converts string with WildCards to regular expression
        /// <summary>
        /// Converts string with WildCards to regular expression.
        /// <list type="bullet">
        /// <item>
        /// <term>?</term>
        /// <description>any character (one and only one)</description>
        /// </item>
        /// <item>
        /// <term>*</term>
        /// <description>any characters (zero or more)</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <example>
        /// <code>
        /// Boolean endsWithEx = Regex.IsMatch(test, WildCardToRegular("*X"));
        /// Boolean startsWithS = Regex.IsMatch(test, WildCardToRegular("S*"));
        /// Boolean containsD = Regex.IsMatch(test, WildCardToRegular("*D*"));
        /// // Starts with S, ends with X, contains "me" and "a" (in that order)
        /// Boolean complex = Regex.IsMatch(test, WildCardToRegular("S*me*a*X"));
        /// </code>
        /// </example>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        // VB.Net Left implementation
        /// <summary>
        /// VB.Net Left implementation.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Left(this string s, int count)
        {
            return s.Substring(0, count);
        }

        // VB.Net Right implementation
        /// <summary>
        /// VB.Net Right implementation.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Right(this string s, int count)
        {
            return s.Substring(s.Length - count, count);
        }

        // VB.Net Mid implementation
        /// <summary>
        /// VB.Net Mid implementation.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Mid(this string s, int index, int count)
        {
            return s.Substring(index, count);
        }

        // VB.Net ToInteger implementation
        /// <summary>
        /// VB.Net ToInteger implementation.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInteger(this string s)
        {
            int integerValue = 0;
            int.TryParse(s, out integerValue);
            return integerValue;
        }

        // VB.Net IsInteger implementation
        /// <summary>
        /// VB.Net IsInteger implementation.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(this string s)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(s).Success;
        }
    }
}
