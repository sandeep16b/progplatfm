using System;

namespace Abim.Platform.Program.WebApi.Util.General.Helpers
{
    /// <summary>
    /// Does less verbose integer parsing
    /// </summary>
    public static class Int
    {
        /// <summary>
        /// Parses a string to an integer.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Cannot parse a null string as an integer
        /// or
        /// Cannot parse an empty string as an integer
        /// </exception>
        public static int Parse(string text)
        {
            if(text == null)
                throw new Exception("Cannot parse a null string as an integer");
            if(text == "")
                throw new Exception("Cannot parse an empty string as an integer");
            if(text.TrimStart('0') == "")
                return 0;
            return int.Parse(text.TrimStart('0'));
        }
        
        /// <summary>
        /// Returns whether a string represents an integer. Useful instead of having to include garbage out parameters
        /// that you don't care about and which make the code uglier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static bool TryParse(string text)
        {
            int outIntTheCallerDoesntCareAbout;
            return int.TryParse(text, out outIntTheCallerDoesntCareAbout);
        }
    }
}
