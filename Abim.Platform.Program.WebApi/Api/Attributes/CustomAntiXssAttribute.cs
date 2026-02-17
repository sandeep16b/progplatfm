using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web.Security.AntiXss;

namespace Abim.Platform.Program.WebApi.Api.Attributes
{
    /// <summary>
    /// An attribute which uses the Microsoft AntiXss to validate input strings being entered into a Web API model. 
    /// It will allow the newline in the input if input attribute provides the respective value in alllowStrings parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CustomAntiXssAttribute : ValidationAttribute
    {
        const string DefaultValidationMessageFormat = "The {0} property failed AntiXss validation";
        private readonly string errorMessage;
        private readonly string errorMessageResourceName;
        private readonly Type errorMessageResourceType;
        private readonly string allowedStrings;
        private readonly string disallowedStrings;
        private readonly Dictionary<string, string> allowedStringsDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAntiXssAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="allowedStrings">A comma separated string allowed characters or words.</param>
        /// <param name="disallowedStrings">A comma separated string disallowed characters or words.</param>
        public CustomAntiXssAttribute(
            string errorMessage = null, 
            string errorMessageResourceName = null, 
            Type errorMessageResourceType = null, 
            string allowedStrings = null, 
            string disallowedStrings = null)
        {
            this.errorMessage = errorMessage;
            this.errorMessageResourceName = errorMessageResourceName;
            this.errorMessageResourceType = errorMessageResourceType;
            this.allowedStrings = allowedStrings;
            this.disallowedStrings = disallowedStrings;
            allowedStringsDictionary = new Dictionary<string, string>();
        }

        public override bool IsValid(object value)
        {
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return base.IsValid(null, validationContext);
            }

            var encodedValue = AntiXssEncoder.HtmlEncode(value.ToString(), false);

            if (EncodedStringAndValueAreDifferent(value, encodedValue))
            {
                SetupAllowedStringsDictionary();

                foreach (var allowedString in allowedStringsDictionary)
                {
                    encodedValue = encodedValue.Replace(allowedString.Value, allowedString.Key);
                }

                if (EncodedStringAndValueAreDifferent(value, encodedValue))
                {
                    return new ValidationResult(SetErrorMessage(validationContext));
                }
            }

            if (!string.IsNullOrWhiteSpace(disallowedStrings) && disallowedStrings.Split(',').Select(x => x.Trim()).Any(x => value.ToString().Contains(x)))
            {
                return new ValidationResult(SetErrorMessage(validationContext));
            }

            return base.IsValid(value, validationContext);
        }

        private static bool EncodedStringAndValueAreDifferent(object value, string encodedValue)
        {
            return !value.ToString().Equals(encodedValue);
        }

        private void SetupAllowedStringsDictionary()
        {
            if (string.IsNullOrWhiteSpace(allowedStrings) && allowedStrings != "\n")
            {
                return;
            }

            foreach (var allowedString in allowedStrings.Split(',').Select(x => x != "\n" ? x.Trim() : x)
                .Where(allowedString => !allowedStringsDictionary.ContainsKey(allowedString)))
            {
                allowedStringsDictionary.Add(allowedString,
                    AntiXssEncoder.HtmlEncode(allowedString, false));
            }
        }

        private string SetErrorMessage(ValidationContext validationContext)
        {
            if (IsResourceErrorMessage())
            {
                var resourceManager = new ResourceManager(errorMessageResourceType);
                return resourceManager.GetString(errorMessageResourceName, CultureInfo.CurrentCulture);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            return  string.Format(DefaultValidationMessageFormat, validationContext.DisplayName);
        }

        private bool IsResourceErrorMessage()
        {
            return !string.IsNullOrEmpty(errorMessageResourceName) && errorMessageResourceType != null;
        }
    }
}
