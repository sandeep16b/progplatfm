using System.Text.RegularExpressions;

namespace Abim.Platform.Program.Relational.Validation.Regex
{
    /// <summary>
    /// Regex class, for validators
    /// </summary>
    public static class RegexValidation
    {
        /// <summary>
        /// The email regex
        /// </summary>
        /// <remarks>
        /// Source: http://stackoverflow.com/questions/16167983/best-regular-expression-for-email-validation-in-c-sharp 
        /// </remarks>
        public const string EmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    
        /// <summary>
        /// Matches an RFC 2822 format email address
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>
        ///   <c>true</c> if the specified email is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmail(string email)
        {
            if(email == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase);
        }
        
        /// <summary>
        /// The US phone regex
        /// </summary>
        /// <remarks>
        /// Source: http://www.regexlib.com/DisplayPatterns.aspx?cattabindex=6&categoryId=7&AspxAutoDetectCookieSupport=1 
        /// </remarks>
        public const string USPhoneRegex = @"^(?:\([2-9]\d{2}\)\ ?|[2-9]\d{2}(?:\-?|\ ?))[2-9]\d{2}[- ]?\d{4}$";
          
        /// <summary>
        /// Description (quoting source http://www.regexlib.com): "US Phone Number: This regular expression for US phone numbers conforms to NANP A-digit and
        /// D-digit requirments (ANN-DNN-NNNN). Area Codes 001-199 are not permitted; Central Office Codes 001-199 are not permitted. Format validation accepts
        /// 10-digits without delimiters, optional parens on area code, and optional spaces or dashes between area code, central office code and station code.
        /// Acceptable formats include 2225551212, 222 555 1212, 222-555-1212, (222) 555 1212, (222) 555-1212, etc. You can add/remove formatting options to
        /// meet your needs."
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns>
        ///   <c>true</c> if the phone number meets the specifications above; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUSPhone(string phone)
        {
            if(phone == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(phone, USPhoneRegex, RegexOptions.IgnoreCase);
        }
    }
}
