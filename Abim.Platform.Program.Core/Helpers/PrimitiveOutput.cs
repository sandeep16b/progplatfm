using System;

namespace Abim.Enterprise.Core.Product.Core.Helpers
{
    /// <summary>
    /// Converts primitives to strings
    /// </summary>
    public static class PrimitiveOutput
    {
        /// <summary>
        /// The C# date tag used in string output if specified by the caller
        /// </summary>
        public const string CSharpDateTag = "/*date*/";
        
        /// <summary>
        /// Converts a primitive to a string used in C# or json.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string PrimitiveToJsonString(object obj)
        {
            if(obj == null) return "null";
            
            if(obj is bool) return obj.ToString().ToLower();
            if(obj is char) return $"\"{obj}\""; //json doesn't allow single quotes
            if(obj is Enum || obj is Guid) return $"\"{obj}\"";
            if(obj is DateTime) return $"\"{obj}\"";
            if(obj.GetType().IsPrimitive) return obj.ToString();
            if(obj is string) return $"\"{obj.ToString().Replace("\"", "\"\"")}\"";
            
            return null;
        }
        
        /// <summary>
        /// Converts a primitive to a string used in C# or json.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="printDatesAsStrings">Whether to print dates as if they were strings, with a comment.</param>
        /// <returns></returns>
        public static string PrimitiveToCSharpString(object obj, bool printDatesAsStrings)
        {
            if(obj == null) return "null";
            
            if(obj is bool) return obj.ToString().ToLower();
            if(obj is char) return $"'{obj}'";
            if(obj is Enum || obj is Guid) return $"\"{obj}\"";
            if(obj is DateTime)
            {
                if(printDatesAsStrings) return $"{CSharpDateTag}\"{obj}\"";
                return $"DateTime.Parse(\"{obj}\")";
            }
            if(obj.GetType().IsPrimitive) return obj.ToString();
            if(obj is string) return $"\"{obj.ToString().Replace("\"", "\\\"")}\"";
            
            return null;
        }
    }
}
