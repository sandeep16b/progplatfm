using Abim.Enterprise.Core.Product.Core.Helpers;
using Abim.Platform.Program.Util.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Util
{
    /// <summary>
    /// Extension class
    /// </summary>
    //Updated from the original shared version
    public static class JsonExtensions
    {
        /// <summary>
        /// The error text
        /// </summary>
        public const string ErrorText = "[Serialization Error]";

        /// <summary>
        /// Determines whether this instance is serializable by JsonConvert.SerializeObject(). If this returns false, methods within this JsonExtensions class can still serialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified instance is serializable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSerializable<T>(this T instance)
        {
            try
            {
                string testForSerializationErrors = JsonConvert.SerializeObject(instance);
                return true;
            }
#pragma warning disable 0168
            catch (Exception ex)
            {
                //often means the object is cyclic
                return false;
            }
        }

        /// <summary>
        /// Does a safe serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static string SafeSerialize<T>(this T instance)
        {
            try
            {
                return JsonConvert.SerializeObject(instance);
            }
#pragma warning disable 0168
            catch (Exception ex)
            {
                return FlatJson(instance);
            }
        }

        /// <summary>
        /// Stringifies the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private static string StringifyObject(Object obj)
        {
            try
            {
                return obj.ToString();
            }
#pragma warning disable 0168
            catch (Exception ex)
            {
                return ErrorText;
            }
        }

        /// <summary>
        /// Provides a flat json representation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="skipFailedProperties">if set to <c>true</c> [skip failed properties].</param>
        /// <returns></returns>
        public static string FlatJson<T>(this T instance, bool skipFailedProperties = true)
        {
            try
            {
                var members = new Dictionary<string, string>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    try
                    {
                        members[prop.Name] = StringifyObject(prop.GetValue(instance, null));
                    }
#pragma warning disable 0168
                    catch (Exception ex3)
                    {
                        if (!skipFailedProperties) throw;
                    }
                }
                foreach (var field in typeof(T).GetFields())
                {
                    try
                    {
                        members[field.Name] = StringifyObject(field.GetValue(instance));
                    }
                    catch (Exception ex4)
                    {
                        if (!skipFailedProperties) throw;
                    }
                }
                string json;
                try
                {
                    json = string.Format("{{{0}}}", string.Join(", ", members.Select(kvp => string.Format("\"{0}\": \"{1}\"", kvp.Key, kvp.Value)).ToArray()));
                }
#pragma warning disable 0168
                catch (Exception ex1)
                {
                    return ErrorText;
                }
                return json;
            }
#pragma warning disable 0168
            catch (Exception ex2)
            {
                return ErrorText;
            }
        }

        #region JsonFormat and Support

        /// <summary>
        /// Calls JsonFormat() with its most overzealous parameters. This is not recommended for use in uncertain scenarios because it passes
        /// includeCalculatedProperties as true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="tabifyJson">Whether to tabify the json.</param>
        /// <returns></returns>
        public static string FullJsonFormat<T>(this T obj, bool tabifyJson = true)
        {
            return JsonFormat(obj, tabifyJson, true, false, true, true);
        }

        /// <summary>
        /// Converts something to its Json format. This ends up being far clearer and more informative than alternatives such as JsonConvert.SerializeObject(),
        /// using expected and clear json such as [] for arrays and lists. It is also safer and more consistent than ServiceStack.Text's Dump() method, in particular
        /// because it doesn't throw an exception on cyclical objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="tabifyJson">Whether to tabify the json.</param>
        /// <param name="skipErrors">Whether to skip errors</param>
        /// <param name="skipObjectsEncounteredInMoreThanOnePlace">If true, reference objects encountered more than once will only be hydrated in once place,
        /// and elsewhere replaced by a string designating them by their hash code. This will always be treated as true when includeCalculatedProperties is true.</param>
        /// <param name="includeJsonIgnoreMembers">Whether to ignore JsonIgnore attributes and include those members anyway.</param>
        /// <param name="includeCalculatedProperties">Whether to include calculated properties as well (properties without setters). This property is not recommended to
        /// be passed as true</param>
        /// <param name="replaceTabsWithSpaces">Whether to replace tabs with spaces, if the json is being tabified.</param>
        /// <returns></returns>
        public static string JsonFormat<T>(this T obj, bool tabifyJson = true, bool skipErrors = true, bool skipObjectsEncounteredInMoreThanOnePlace = false, bool includeJsonIgnoreMembers = false,
            bool includeCalculatedProperties = false, bool replaceTabsWithSpaces = false)
        {
            try
            {
                if (includeCalculatedProperties) skipObjectsEncounteredInMoreThanOnePlace = true;        //this is necessary to avoid cycles
                Dictionary<object, bool> dict = null;
                if (skipObjectsEncounteredInMoreThanOnePlace) dict = new Dictionary<object, bool>();
                return GetJsonFormat(obj, 1, tabifyJson, skipErrors, dict, includeJsonIgnoreMembers, includeCalculatedProperties, replaceTabsWithSpaces);
            }
#pragma warning disable 0168
            catch (Exception ex)
            {
                if (!skipErrors) throw;
                return "\"unknown\"";
            }
        }

        /// <summary>
        /// Converts something to its Json format
        /// </summary>
        /// <remarks>
        /// This private overload isn't generic because it doesn't need to be, the runtime type of the object is determined. Also, a type T wouldn't be determineable at compile time for
        /// the recursions which use dynamics (though a MethodInfo object could be used for that generic recursion if needed)
        /// </remarks>
        /// <param name="obj">The object.</param>
        /// <param name="depth">The current depth level, starting at 1.</param>
        /// <param name="tabifyJson">Whether to tabify the json.</param>
        /// <param name="skipErrors">Whether to skip errors.</param>
        /// <param name="redundancyDictionary">A dictionary to keep track of previously-encountered objects.</param>
        /// <param name="includeJsonIgnoreMembers">Whether to ignore JsonIgnore attributes and include those members anyway.</param>
        /// <param name="replaceTabsWithSpaces">Whether to replace tabs with spaces, if the json is being tabified.</param>
        /// <returns></returns>
        private static string GetJsonFormat(Object obj, int depth, bool tabifyJson, bool skipErrors, Dictionary<object, bool> redundancyDictionary, bool includeJsonIgnoreMembers,
            bool includeCalculatedProperties, bool replaceTabsWithSpaces = false)
        {
            if (depth > 20) return "\"(too deep)\"";
            if (obj == null) return "null";

            //Primitive
            var primitiveOutput = PrimitiveOutput.PrimitiveToJsonString(obj);
            if (primitiveOutput != null) return primitiveOutput;

            //JObject
            if (obj is Newtonsoft.Json.Linq.JObject) return $"\"{(obj as Newtonsoft.Json.Linq.JObject).ToString(Formatting.None)}\"";
            var type = obj.GetType();

            string endingTab = "";
            string tab = replaceTabsWithSpaces ? "    " : "\t";
            for (int i = 0; i < depth - 1 /*because depth starts at 1*/; i++) endingTab += tab;
            string entryTab = endingTab + tab;

            Func<object, string> recurse = (child) => GetJsonFormat(child, depth + 1, tabifyJson, skipErrors, redundancyDictionary, includeJsonIgnoreMembers, includeCalculatedProperties);

            //Nullable
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return recurse(Convert.ChangeType(obj, nullableType));

            //IDictionary (this also covers ExpandoObjects)
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                dynamic dict = obj;
                var kvps = new List<KeyValuePair<string, string>>();
                foreach (var kvp in dict) kvps.Add(new KeyValuePair<string, string>(recurse(kvp.Key), recurse(kvp.Value)));
                return CreateObjectJson(kvps, tabifyJson, entryTab, endingTab);
            }

            //IEnumerable
            var enumerable = obj as System.Collections.IEnumerable;
            if (enumerable != null && type.Name != "JObject")
            {
                if (enumerable.Count() == 0) return "[]";
                var vals = new List<string>();
                if (tabifyJson)
                {
                    foreach (var item in enumerable)
                        vals.Add($"{entryTab}{recurse(item)}");
                    return $"[\r\n{string.Join(",\r\n", vals)}\r\n{endingTab}]";
                }
                else
                {
                    foreach (var item in enumerable)
                        vals.Add(recurse(item));
                    return $"[{string.Join(", ", vals)}]";
                }
            }

            //Claim
            if (type.FullName == "System.Security.Claims.Claim")
            {
                //the reason for dynamic here is for .Net 4.0 support
                var claim = obj as dynamic;
                var kvp = new List<KeyValuePair<string, string>>();
                kvp.Add(new KeyValuePair<string, string>($"\"{claim.Type}\"", $"\"{claim.Value}\""));
                return CreateObjectJson(kvp, tabifyJson, entryTab, endingTab);
            }

            if (redundancyDictionary != null && !type.IsValueType)
            {
                if (redundancyDictionary.ContainsKey(obj))
                    return $"\"Duplicate reference to {type.ReadableName()} ({obj.GetHashCode()})\"";
                else redundancyDictionary[obj] = true;
            }

            //Object (member iteration)
            try
            {
                var kvp = new List<KeyValuePair<string, string>>();
                foreach (var field in type.GetFields().Where(f => f.IsPublic && !f.IsStatic))
                {
                    try
                    {
                        kvp.Add(new KeyValuePair<string, string>($"\"{field.Name}\"", recurse(field.GetValue(obj))));
                    }
#pragma warning disable 0168
                    catch (Exception ex1)
                    {
                        if (!skipErrors) throw;
                    }
                }
                //for properties, we usually need to make sure that a Set method exists because we don't usually care about most calculated properties. They can also cause unwanted cycles
                foreach (var prop in type.GetProperties().Where(p => p.GetGetMethod(false) != null && !p.GetGetMethod(false).IsStatic &&
                     (p.GetSetMethod(true) != null || type.Name.StartsWith("Tuple`") || includeCalculatedProperties) &&
                     (includeJsonIgnoreMembers || !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any()) && p.GetIndexParameters().Length == 0))
                {
                    try
                    {
                        //check this line if a process is hanging with high CPU usage, in which case the caller should have passed skipObjectsEncounteredInMoreThanOnePlace as true in their
                        //... call to JsonFormat() for this particular object
                        kvp.Add(new KeyValuePair<string, string>($"\"{prop.Name}\"", recurse(prop.GetValue(obj, null))));
                    }
#pragma warning disable 0168
                    catch (Exception ex2)
                    {
                        if (!skipErrors) throw;
                    }
                }
                return CreateObjectJson(kvp, tabifyJson, entryTab, endingTab);
            }
#pragma warning disable 0168
            catch (Exception ex)
            {
                if (!skipErrors) throw;
                return "\"unknown\"";
            }
        }

        /// <summary>
        /// Creates json for an object-type item.
        /// </summary>
        /// <param name="kvp">The KVP of properties. Both the keys and the values should already contain quotation marks if they need them, because they won't be added here. This is
        /// because some keys aren't strings, e.g. in Dictionaries whose Key type isn't string
        /// </param>
        /// <param name="tabifyJson">if set to <c>true</c> [tabify json].</param>
        /// <param name="entryTab">The entry tab.</param>
        /// <param name="endingTab">The ending tab.</param>
        /// <returns></returns>
        private static string CreateObjectJson(List<KeyValuePair<string, string>> kvp, bool tabifyJson, string entryTab, string endingTab)
        {
            if (!kvp.Any()) return "{}";
            if (tabifyJson) return $"{{\r\n{string.Join(",\r\n", kvp.Select(p => $"{entryTab}{p.Key}: {p.Value}"))}\r\n{endingTab}}}";
            else return $"{{{string.Join(", ", kvp.Select(p => $"{p.Key}: {p.Value}"))}}}";
        }

        #endregion
    }
}
