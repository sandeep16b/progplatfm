using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Relational.Validation.Impl
{
    /// <summary>
    /// ValidationResult Class.
    /// </summary>
    public class AbimValidationResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AbimValidationResult"/> has succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public IEnumerable<IValidationResult> Results { get; set; }

        /// <summary>
        /// Gets a single combined message representing all errors (NOT info messages)
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                if(Results == null || !Results.Any()) return "";
                return string.Join("; ", Results.Where(r => r != null).Select(r => r.Message).ToArray());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbimValidationResult"/> class.
        /// </summary>
        public AbimValidationResult()
        {
        }
    }

    /// <summary>
    /// Extension class
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// ToValidationResult method.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static AbimValidationResult ToAbimValidationResult(this FluentValidation.Results.ValidationResult result)
        {
            if(result == null)
                throw new Exception("Cannot convert null ValidationResult to AbimValidationResult");
            try
            {
                return new AbimValidationResult()
                {
                    Succeeded = result.IsValid,
                    Results = result.Errors.Select(o =>
                        new ValidationErrorResult
                        {
                            Message = o.ErrorMessage,
                            Property = o.PropertyName
                        }
                    )
                };
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                string json = "";
                try
                {
                    json = JsonConvert.SerializeObject(result);
                }
                catch(Exception ex2)
                {
                    json = "[unable to stringify]";
                }
                throw new Exception(string.Format("Error converting ValidationResult {0} to AbimValidationResult", json));
            }
        }
    }
}
