using Abim.Platform.Program.Core.Api.Attributes;


namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// Returns whether a character is in the range 0-9
        /// </summary>
        /// <remarks>
        /// Both Char.IsNumber() and CharExtensions.IsDigitCharacter() return true for characters other than these. See https://stackoverflow.com/questions/228532/difference-between-char-isdigit-and-char-isnumber-in-c-sharp
        /// </remarks>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if [is digit character] [the specified c]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ '5' }, ShouldReturn = true)]
        public static bool IsDigitCharacter(this char c)
        {
            return c >= '0' && c <= '9';
        }
        
        /// <summary>
        /// Returns whether a character is in the range A-Z or a-z
        /// </summary>
        /// <remarks>
        /// Both Char.IsLetter() allows returns true for non-English letters
        /// </remarks>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if [is digit character] [the specified c]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ 'S' }, ShouldReturn = true)]
        public static bool IsLetterCharacter(this char c)
        {
            return IsUpper(c) || IsLower(c);
        }
        
        /// <summary>
        /// Returns whether a character is in the range A-Z
        /// </summary>
        /// <remarks>
        /// Both Char.IsLetter() allows returns true for non-English letters
        /// </remarks>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if [is digit character] [the specified c]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ 'S' }, ShouldReturn = true)]
        public static bool IsUpper(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }
        
        /// <summary>
        /// Returns whether a character is in the range a-z
        /// </summary>
        /// <remarks>
        /// Both Char.IsLetter() allows returns true for non-English letters
        /// </remarks>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if [is digit character] [the specified c]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ 'e' }, ShouldReturn = true)]
        public static bool IsLower(this char c)
        {
            return c >= 'a' && c <= 'z';
        }
        
        /// <summary>
        /// Determines whether a character is an English letter character or digit character
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if [is letter or digit character] [the specified c]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ "2" }, ShouldReturn = true)]
        public static bool IsLetterOrDigitCharacter(this char c)
        {
            return IsLetterCharacter(c) || IsDigitCharacter(c);
        }
    }
}
