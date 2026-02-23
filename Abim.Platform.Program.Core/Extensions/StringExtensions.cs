using Abim.Platform.Program.Core.Api.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts e.g. "myString" to "MyString"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{"testString"}, ShouldReturn = "TestString")]
        public static string CapitalizeStart(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            return Char.ToUpper(s[0]) + s.Substring(1);
        }
        
        /// <summary>
        /// Converts e.g. "aa bb cc" to "Aa Bb Cc"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{"test string"}, ShouldReturn = "Test String")]
        public static string CapitalizeStarts(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            string[] split = s.Split(' ');
            return string.Join(" ", split.Select(s1 => CapitalizeStart(s1)).ToArray());
        }
        
        /// <summary>
        /// Converts e.g. "MyString" to "myString"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{"TestString"}, ShouldReturn = "testString")]
        public static string LowerCaseStart(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            return Char.ToLower(s[0]) + s.Substring(1);
        }
        
        /// <summary>
        /// Converts e.g. "CMPRegistration" to "cmpRegistration", if the string starts with an acronym, otherwise converts only the first character to lower case
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{"CMPRegistration"}, ShouldReturn = "cmpRegistration")]
        public static string SmartLowerCaseStart(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            char[] c = s.ToCharArray();
            int index = 0;

            for(index = 0; index < c.Length && CharExtensions.IsLetterCharacter(c[index]) && Char.IsUpper(c[index]); index++){}

            if(index > 1) return s.Substring(0, index - 1).ToLower() + s.Substring(index - 1);
            
            return Char.ToLower(c[0]) + s.Substring(1);
        }
        
        /// <summary>
        /// Converts e.g. "myString" to "myStrings"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "pathwayType" }, ShouldReturn = "pathwayTypes")]
        public static string Pluralize(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            if(s.ToLower().EndsWith("s")) return s;
            return s + (CharExtensions.IsUpper(s[s.Length - 1]) ? 'S' : 's');
        }
        
        /// <summary>
        /// Converts e.g. "my strings" to "my string"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "SampleObjects" }, ShouldReturn = "SampleObject")]
        public static string Singularize(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            if(s.ToLower().EndsWith("es") && s.Length > 2) return s.Substring(0, s.Length - 2);
            if(s.ToLower().EndsWith("s") && s.Length > 1) return s.Substring(0, s.Length - 1);
            return s;
        }
        
        /// <summary>
        /// Converts e.g. "myString" to "my string"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "AddRegistrationADACommand" }, ShouldReturn = "add registration a d a command")]
        public static string Humanize(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            char[] c = s.ToCharArray();
            string output = "";
            for(int i = 0; i < c.Length; i++)
            {
                if(CharExtensions.IsUpper(c[i]))
                {
                    if(i > 0 && CharExtensions.IsLetterCharacter(c[i - 1])) output += " ";
                    output += Char.ToLower(c[i]);
                }
                else output += c[i];
            }
            return output;
        }
        
        /// <summary>
        /// Returns a string consisting of the specified number of spaces.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ 8 }, ShouldReturn = "        ")]
        public static string Spaces(int count)
        {
            char[] c = new char[count];
            for(int i = 0; i < count; i++) c[i] = ' ';
            return new string(c);
        }
        
        /// <summary>
        /// Counts the leading spaces at the start of a string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "        {" }, ShouldReturn = 8)]
        public static int Spaces(this string s)
        {
            if(s == null) throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Spaces)}() was passed a null string");
            return s.Length - s.TrimStart(' ').Length;
        }
        
        /// <summary>
        /// Gets the start space substring of a string
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "        {" }, ShouldReturn = "        ")]
        public static string InitialSpaces(this string s)
        {
            int intSpaces = s.Spaces();
            return Spaces(intSpaces);
        }
        
        /// <summary>
        /// Converts e.g. "my string" to "myString"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "sample object" }, ShouldReturn = "sampleObject")]
        public static string CamelCase(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            
            var withSpaces = s/*.ToLower()*/.Replace("\r", " ").Replace("\n", " ").Replace("\t", "");
            var withSingleSpaces = System.Text.RegularExpressions.Regex.Replace(withSpaces, @"\s+", " ");
            var words = withSingleSpaces.Split(' ').ToList();
            
            List<string> output = new List<string>();
            foreach(string word in words)
            {
                if(!output.Any()) output.Add(word.SmartLowerCaseStart());
                else output.Add(word.CapitalizeStart());
            }
            return string.Join("", output.ToArray());
        }
        
        /// <summary>
        /// Converts e.g. "my string" to "MyString"
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "sample object" }, ShouldReturn = "SampleObject")]
        public static string PascalCase(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            
            var withSpaces = s/*.ToLower()*/.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            var withSingleSpaces = System.Text.RegularExpressions.Regex.Replace(withSpaces, @"\s+", " ");
            var words = withSingleSpaces.Split(' ').ToList();
            
            List<string> output = new List<string>();
            foreach(string word in words)
            {
                output.Add(word.CapitalizeStart());
            }
            return string.Join("", output.ToArray());
        }
        
        /// <summary>
        /// Makes a comma/semicolon (depending on the joiner) separated list as a string, with the word "and" before the last entry.
        /// </summary>
        /// <param name="joiner">The joiner.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithAnd<TObject>(string joiner, IEnumerable<TObject> collection)
        {
            return JoinWithConjunction<TObject>(joiner, "and", collection);
        }
        
        /// <summary>
        /// Makes a comma/semicolon (depending on the joiner) separated list as a string, with the word "or" before the last entry.
        /// </summary>
        /// <param name="joiner">The joiner.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithOr<TObject>(string joiner, IEnumerable<TObject> collection)
        {
            return JoinWithConjunction<TObject>(joiner, "or", collection);
        }
        
        /// <summary>
        /// Makes a comma-separated list as a string, with the word "and" before the last entry.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithAnd<TObject>(IEnumerable<TObject> collection)
        {
            return JoinWithConjunction<TObject>(", ", "and", collection);
        }
        
        /// <summary>
        /// Makes a comma-separated list as a string, with the word "or" before the last entry.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithOr<TObject>(IEnumerable<TObject> collection)
        {
            return JoinWithConjunction<TObject>(", ", "or", collection);
        }
        
        /// <summary>
        /// Joins with a conjunction.
        /// </summary>
        /// <param name="joiner">The joiner.</param>
        /// <param name="conjunction">The conjunction.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        private static string JoinWithConjunction<TObject>(string joiner, string conjunction, IEnumerable<TObject> collection)
        {
            if(collection == null) return "none";
            if(!collection.Any()) return "none";
            var array = collection.ToArray();
            if(array.Length == 1)
            {
                if(array[0] == null) return "null";
                return array[0].ToString();
            }
            string val = "";
            for(int i = 0; i < array.Length; i++)
            {
                if(i > 0)
                {
                    if(array.Length >= 3) val += joiner.Trim();
                    val += " ";
                }
                if(i == array.Length - 1)
                    val += conjunction + " ";
                if(array[i] == null) val += "null";
                else val += array[i];
            }
            return val;
        }
        
        /// <summary>
        /// Returns "a" or "an".
        /// </summary>
        /// <remarks>
        /// Technically the choice of "a" vs "an" goes by pronunciation (e.g. "an herb" vs. "a house"), but we can't code for that here
        /// </remarks>
        /// <param name="nounPhrase">The noun phrase.</param>
        /// <param name="includeNounPhraseInReturnValue">if set to <c>true</c> [include noun phrase in return value].</param>
        /// <param name="capitalizeReturnPhrase">if set to <c>true</c> [capitalize return phrase].</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Administrator cancellation action", true, false }, ShouldReturn = "an Administrator cancellation action")]
        public static string AOrAn(string nounPhrase, bool includeNounPhraseInReturnValue = true, bool capitalizeReturnPhrase = false)
        {
            char startingLetter = nounPhrase.ToUpper()[0];
            var vowels = new[]{'A', 'E', 'I', 'O', 'U'};
            
            string aOrAn;
            if(vowels.Contains(startingLetter))
                aOrAn = "an";
            else
                aOrAn = "a";

            string returnValue = "";
            returnValue += aOrAn;
            if(includeNounPhraseInReturnValue)
                returnValue += " " + nounPhrase;
            if(capitalizeReturnPhrase)
                returnValue = returnValue.CapitalizeStart();
            return returnValue;
        }
        
        /// <summary>
        /// Returns "a" or "an", to go before a numeral in a proper English sentence. For an example, one would say "an 18 ounce glass", not "a 18 ounce glass".
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "4" }, ShouldReturn = "a")]
        public static string AOrAn(int number)
        {
            var str = number.ToString();
            if(str.StartsWith("8")) return "an";                             //"eight" or "eighty"
            if(str.StartsWith("11") && str.Length % 3 == 2) return "an";     //"eleven"
            if(str.StartsWith("18") && str.Length % 3 == 2) return "an";     //"eighteen"
            return "a";
        }
        
        /// <summary>
        /// Returns either "is" or "are" for string text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string IsOrAre<T>(this IEnumerable<T> collection)
        {
            if(collection.Count() == 1) return "is";
            return "are";
        }
        
        /// <summary>
        /// Returns either "has" or "have" for string text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string HasOrHave<T>(this IEnumerable<T> collection)
        {
            if(collection.Count() == 1) return "has";
            return "have";
        }
        
        /// <summary>
        /// Returns a pluralization 's'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string S<T>(this IEnumerable<T> collection)
        {
            if(collection.Count() == 1) return "";
            return "s";
        }
        
        /// <summary>
        /// Describes a division of a list into sublists, for logging purposes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPropertyValue">The type of the property value.</typeparam>
        /// <param name="fullList">The full list.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="getId">The get identifier.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="getProperty">The get property.</param>
        /// <returns></returns>
        public static string DescribeDivision<T, TPropertyValue>(List<T> fullList, string objectName, Func<T, string> getId, string propertyName,
            Func<T, TPropertyValue> getProperty)
        {
            var division = fullList.Divide(getProperty);
            string quote = "";
            if(typeof(TPropertyValue).Name == typeof(string).Name) quote = "'";
            var description = division.Select(list => $"{objectName}{list.S()} {list.Select(r => getId(r)).JoinWithAnd()} {list.HasOrHave()} " +
                $"{propertyName} {quote}{getProperty(list.First())}{quote}").JoinWithAnd(";");
            return description;
        }
        
        /// <summary>
        /// Performs a case-insensitive Contains()
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="substring">The substring.</param>
        /// <returns>
        ///   <c>true</c> if [contains case insensitive] [the specified substring]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Administration.AdministrationDate", "administration" }, ShouldReturn = true)]
        public static bool ContainsCaseInsensitive(this string str, string substring)
        {
            return str?.ToLower().Contains(substring.ToLower()) == true;
        }
        
        /// <summary>
        /// Performs a case-insensitive Replace.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="target">The target.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Administration.AdministrationDate", "administration.", "" }, ShouldReturn = "AdministrationDate")]
        public static string ReplaceCaseInsensitive(this string str, string target, string replacement)
        {
            if(str == null || target == null || replacement == null) return str;
            var indices = new List<int>();
            int index = -1;
            while(true)
            {
                if(index == -1) index = str.ToLower().IndexOf(target.ToLower());
                else index = index = str.ToLower().IndexOf(target.ToLower(), index + 1);
                if(index == -1) break;
                indices.Add(index);
            }
            if(!indices.Any()) return str;
            var builder = new StringBuilder();
            int nextStart = 0;
            for(int i = 0; i < indices.Count; i++)
            {
                builder.Append(str.Substring(nextStart, indices[i] - nextStart));
                builder.Append(replacement);
                nextStart = indices[i] + target.Length;
            }
            if(nextStart < str.Length) builder.Append(str.Substring(nextStart, str.Length - nextStart));
            var result = builder.ToString();
            return result;
        }
        
        /// <summary>
        /// Does a match based on "*" wildcards (only at the start and/or end of the pattern).
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "ABCDEFG", "*BCD*" }, ShouldReturn = true)]
        public static bool StarMatch(this string str, string pattern)
        {
            if(pattern == "*") return (str != null);
            if(pattern.StartsWith("*") && pattern.EndsWith("*")) return str.Contains(pattern.Substring(1, pattern.Length - 2));
            if(pattern.StartsWith("*")) return str.EndsWith(pattern.Substring(1));
            if(pattern.EndsWith("*")) return str.StartsWith(pattern.Substring(0, pattern.Length - 1));
            return str == pattern;
        }
        
        /// <summary>
        /// Gets a description of a match pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "*BCD*" }, ShouldReturn = "containing \"BCD\"")]
        public static string StarDescription(string pattern)
        {
            if(pattern == "*") return "of any value";
            if(pattern.StartsWith("*") && pattern.EndsWith("*")) return $"containing \"{pattern.Substring(1, pattern.Length - 2)}\"";
            if(pattern.StartsWith("*")) return $"ending with \"{pattern.Substring(1)}\"";
            if(pattern.EndsWith("*")) return $"starting with \"{pattern.Substring(0, pattern.Length - 1)}\"";
            return $"of \"{pattern}\"";
        }
        
        /// <summary>
        /// Returns a string containing only the alphanumeric characters and spaces from the original string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "nas24fadfgE#5 2  4t$fg" }, ShouldReturn = "nas24fadfgE5 2  4tfg")]
        public static string AlphanumericAndSpaces(this string str)
        {
            if(str == null) return null;
            return new string(str.ToCharArray().Where(c => CharExtensions.IsLetterOrDigitCharacter(c) || c == ' ').ToArray());
        }
        
        /// <summary>
        /// Returns a string containing only the numeric characters from the original string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "nas24fadfgE#5 2  4t$fg" }, ShouldReturn = "24524")]
        public static string NumericOnly(this string str)
        {
            if(str == null) return null;
            return new string(str.ToCharArray().Where(c => CharExtensions.IsDigitCharacter(c)).ToArray());
        }
        
        /// <summary>
        /// Gets a substring starting at, and optionally containing, a certain substring in the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startingWith">The start symbol.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Example{ Entry }", "{" }, ShouldReturn = "{ Entry }")]
        public static string Substring(this string str, string startingWith, bool includeStartingWithInOutput = false)
        {
            if(startingWith == "") throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given an empty starting substring");
            if(startingWith == null) throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given a null starting substring");
            if(str == null) return "";
            int ind1 = str.IndexOf(startingWith);
            if(ind1 == -1) return "";
            if(!includeStartingWithInOutput) ind1 += startingWith.Length;
            var cropped = str.Substring(ind1);
            return cropped;
        }
        
        /// <summary>
        /// Gets a substring between two symbols in a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startSymbol">The start symbol.</param>
        /// <param name="endSymbol">The end symbol.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Example{ Entry }", "{", "}" }, ShouldReturn = " Entry ")]
        public static string Substring(this string str, string startSymbol, string endSymbol)
        {
            if(startSymbol == "") throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given an empty starting substring");
            if(startSymbol == null) throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given a null starting substring");
            if(endSymbol == "") throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given an empty ending substring");
            if(endSymbol == null) throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(Substring)}() was given a null ending substring");
            if(str == null) return "";
            int ind1 = str.IndexOf(startSymbol);
            if(ind1 == -1) return "";
            ind1 += startSymbol.Length;
            int ind2 = str.IndexOf(endSymbol, ind1);
            if(ind2 == -1) return "";
            var cropped = str.Substring(ind1, ind2 - ind1);
            return cropped;
        }
        
        /// <summary>
        /// Determines whether this string is a guid.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is a unique identifier; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ "ef0ada94-3d06-46f2-8e71-0121cd1ee41f" }, ShouldReturn = true)]
        public static bool IsGuid(this string str)
        {
            return Guid.TryParse(str, out Guid guid);
        }
        
        /// <summary>
        /// Just returns a substring intuitively, meaning that if the start or length go out of bounds, it returns what it can anyway instead of failing.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="addEllipses">if true, and the substring started at index 0 and excluded part of the end of the original string, ellipses will be added to the end.</param>
        /// <returns>Never returns null (unless the string itself which was passed in is null) and should not throw an exception</returns>
        public static string SafeSubstring(this string str, int start, int? length = null, bool addEllipses = false)
        {
            if(str == null) return null;
            
            int len = 0;
            if(length != null) len = length.Value;
            else
            {
                if(str.Length == 0) return "";
                if(start <= 0) return str;
                if(start < str.Length) return str.Substring(start);
                return "";
            }
            
            bool startPassedAsZero = (start == 0);
            if(start < 0)
            {
                len -= (0 - start);
                start = 0;
            }
            if(len <= 0) return "";
            if(start >= str.Length) return "";
            if(start + len > str.Length)
                len -= ((start + len) - str.Length);
            var cropped = str.Substring(start, len);
            if(addEllipses && startPassedAsZero && len < str.Length) return $"{cropped}...";
            else return cropped;
        }
        
        /// <summary>
        /// Returns all the starting indices of a given substring.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="substring">The substring.</param>
        /// <returns></returns>
        public static int[] IndicesOf(this string str, string substring)
        {
            var indices = new List<int>();
            var c = str.ToCharArray();
            for(int i = 0; i < c.Length; i++)
            {
                if(str.SafeSubstring(i, substring.Length) == substring)
                    indices.Add(i);
            }
            return indices.ToArray();
        }
        
        /// <summary>
        /// Removes a sentence-ending period from the end of a string. If the string ends in an ellipsis ("..."), no period is removed
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Sentence.." }, ShouldReturn = "Sentence")]
        public static string TrimPeriod(this string str)
        {
            if(str == null) return null;
            str = str.TrimEnd(' ');
            if(str.EndsWith("...")) return str;
            return str.TrimEnd('.');
        }
        
        /// <summary>
        /// Performs a whole-word "Contains"
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="substring">The substring.</param>
        /// <returns>
        ///   <c>true</c> if [contains whole word] [the specified string]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ "ABCDEF GHIJK", "HIJ" }, ShouldReturn = false)]
        public static bool ContainsWholeWord(this string str, string substring)
        {
            return IndicesOf(str, substring).Any(i =>
                (i == 0 || !CharExtensions.IsLetterOrDigitCharacter(str[i - 1])) &&
                (i + substring.Length == str.Length || !CharExtensions.IsLetterOrDigitCharacter(str[i + substring.Length])));
        }
        
        /// <summary>
        /// Copies the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [ExpectedReturnExample(Arguments = new object[]{ "Out of Sync: Authorizations removed" }, ShouldReturn = "Out of Sync: Authorizations removed")]
        public static string Copy(this string str)
        {
            return new string(str.ToCharArray());
        }
        
        /// <summary>
        /// Shortens a string by a specified number of characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        [ExpectedReturnExample(Arguments = new object[]{ "Authorization", 2 }, ShouldReturn = "Authorizati")]
        public static string ShortenBy(this string str, int amount)
        {
            if(amount < 0) throw new ArgumentException($"{nameof(StringExtensions)}.{nameof(ShortenBy)}() was given a negative amount to shorten the string by");
            if(str == null) return null;
            int newLength = Math.Max(str.Length - amount, 0);
            return str.Substring(0, newLength);
        }
        
        /// Gets the lines of text betweens the specified other lines, in an array of text.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="line1">line1.</param>
        /// <param name="line2">line2.</param>
        /// <returns></returns>
        public static string[] LinesBetween(string[] lines, string line1, string line2)
        {
            for(int i = 0; i < lines.Length; i++)
            {
                if(lines[i] == line1)
                {
                    int j = i;
                    if(line2 == null)
                    {
                        j = lines.Length;
                        var array = new string[j - i - 1];
                        for(int x = i + 1; x < j; x++)
                            array[x - (i + 1)] = lines[x];
                        return array;
                    }

                    for(; j < lines.Length && lines[j] != line2; j++){}

                    if(j < lines.Length)
                    {
                        var array = new string[j - i - 1];
                        for(int x = i + 1; x < j; x++)
                            array[x - (i + 1)] = lines[x];
                        return array;
                    }
                }
            }
            return new string[0];
        }
    }
}
