using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// Stores all shared error messages
    /// </summary>
    public static class ErrorMessages
    {
        /// <summary>
        /// No objects were found
        /// </summary>
        private const string _noneFound = "No {0} found that matches your parameters";

        /// <summary>
        /// NoneFound.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static string NoneFound(string typeName)
        {
            return string.Format(_noneFound, typeName);
        }

        /// <summary>
        /// NoneFound.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        public static string NoneFound(Type entityType)
        {
            return string.Format(_noneFound, entityType.Name);
        }
        
        /// <summary>
        /// That object (by id) was not found
        /// </summary>
        private const string _notFound = "{0} {1} not found";

        /// <summary>
        /// NotFound.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static string NotFound(string typeName, Object id)
        {
            return string.Format(_notFound, typeName, id);
        }

        /// <summary>
        /// NotFound.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static string NotFound(string typeName)
        {
            return string.Format(_notFound, typeName, "of that Id");
        }

        /// <summary>
        /// NotFound.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static string NotFound(Type entityType, Object id)
        {
            return string.Format(_notFound, entityType.Name, id);
        }

        /// <summary>
        /// NotFound.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        public static string NotFound(Type entityType)
        {
            return string.Format(_notFound, entityType.Name, "of that Id");
        }
        
        /// <summary>
        /// There were problems with the paging parameters supplied
        /// </summary>
        private const string _invalidPaging = "Invalid paging parameters were given: {0}";

        /// <summary>
        /// InvalidPaging.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public static string InvalidPaging(string reason)
        {
            return string.Format(_invalidPaging, reason);
        }

        /// <summary>
        /// PageIndexTooHigh.
        /// </summary>
        /// <returns></returns>
        public static string PageIndexTooHigh()
        {
            return InvalidPaging("PageIndex too high");
        }
        
        /// <summary>
        /// The ProfileId for the user was not found
        /// </summary>
        public const string ProfileIdNotFound = "ProfileId not found";
        
        /// <summary>
        /// The AbimId for the user was not found
        /// </summary>
        public const string AbimIdNotFound = "AbimId not found";
        
        /// <summary>
        /// User profile was not found
        /// </summary>
        public const string UserProfileNotFound = "User Profile is missing";
        
        /// <summary>
        /// User profile has been marked as deceased
        /// </summary>
        public const string UserProfileMarkedDeceased = "User Profile is marked as deceased";
        
        /// <summary>
        /// Invalid query parameters were supplied
        /// </summary>
        private const string _invalidParameters = "Invalid query parameters were given: {0}";

        /// <summary>
        /// Invalid query parameters were supplied
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns></returns>
        public static string InvalidParameters(string paramName)
        {
            return string.Format(_invalidParameters, paramName);
        }
        
        /// <summary>
        /// Invalid enum type
        /// </summary>
        public const string InvalidEnumType = "Invalid enumeration type";

        /// <summary>
        /// Invalid enum type, in which the valid types are presented to the user
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="supportedTypes">The supported types.</param>
        /// <param name="includingNestedEnumTypes">if set to <c>true</c> [including nested enum types].</param>
        /// <returns></returns>
        public static string InvalidEnumType_ValidTypesAre(Type modelType, IEnumerable<Type> supportedTypes, bool includingNestedEnumTypes)
        {
            string extraNoteText = includingNestedEnumTypes ? ", including enums inside nested objects," : "";
            var msg = string.Format("{0}. Valid types for the {1} model{2} include {3}",
                InvalidEnumType, modelType.Name, extraNoteText, string.Join(", ", supportedTypes.Select(t => t.Name).ToArray()));
            return msg;
        }
        
        /// <summary>
        /// The invalid unique identifier error message
        /// </summary>
        public const string InvalidGuidId = "Invalid Id. Id must be a valid Guid";
        
        /// <summary>
        /// The invalid integer identifier error message
        /// </summary>
        public const string InvalidIntegerId = "Invalid Id. Id must be a valid integer";
        
        /// <summary>
        /// The invalid DateTime error message
        /// </summary>
        public const string InvalidDateTime = "Invalid parameter. Parameter must be a valid DateTime";
        
        /// <summary>
        /// The incorrect command identifier error message
        /// </summary>
        public const string IncorrectCommandId = "The command Id does not match the route Id";
        
        /// <summary>
        /// The command not bound error message
        /// </summary>
        public const string CommandNotBound = "The command could not be bound. Please check that it consists of valid json with the correct properties";

        /// <summary>
        /// The complex query required error message
        /// </summary>
        public const string ComplexQueryRequired = "A complex query is required for this endpoint. Either the caller did not provide one or it could not be bound";
        
        /// <summary>
        /// Invalid query parameters were supplied
        /// </summary>
        private const string _forbiddenResource = "You do not have permission to access this {0} resource";

        /// <summary>
        /// ForbiddenResource.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static string ForbiddenResource(string typeName)
        {
            return string.Format(_forbiddenResource, typeName);
        }

        /// <summary>
        /// ForbiddenResource.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static string ForbiddenResource()
        {
            return _forbiddenResource.Replace(" {0}", "");
        }

        /// <summary>
        /// ForbiddenResource.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        public static string ForbiddenResource(Type entityType)
        {
            return string.Format(_forbiddenResource, entityType.Name);
        }

        /// <summary>
        /// The unexpected error, error message
        /// </summary>
        public const string UnexpectedError = "The API has encountered an unexpected error and cannot process this request.";
    }
}
