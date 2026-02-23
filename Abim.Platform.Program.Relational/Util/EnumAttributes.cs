using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Abim.Platform.Program.Relational.Classes
{
    /// <summary>
    /// Library of useful enum methods
    /// </summary>
    public static class EnumAttributes
    {
        /// <summary>
        /// The Random
        /// </summary>
        private static readonly Random Random = new Random();

        private static object _getEnumByNameLock = new object();

        /// <summary>
        /// An object to lock when calling GetEnumByName() for thread safety
        /// </summary>
        private static object GetEnumByNameLock = new object();

        /// <summary>
        /// Reads the integer or character code of the enum, and returns it as a string
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumCode(Object enumValue)
        {
            if(EnumHasCharValues(enumValue.GetType()))
                return ReadEnumChar(enumValue).ToString();
            else return ReadEnumInteger(enumValue).ToString();
        }

        /// <summary>
        /// Determines whether or not an enum type is defined with chars instead of ints. Note Enum.GetUnderlyingType() will return
        /// typeof(int) in both cases. There is no better way to do this
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static bool EnumHasCharValues(Type enumType)
        {
            if(!enumType.IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", enumType.Name));
            return GetEnumValues(enumType).All(e => Char.IsLetter((char)(Convert.ToInt32(e))) || Convert.ToInt32(e) == 0);
        }

        /// <summary>
        /// Reads the integer code of the enum
        /// </summary>
        /// <remarks>
        /// The enum value is required to have an integer code, not a char one, for this to work
        /// </remarks>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static int ReadEnumInteger(Object enumValue)
        {
            Enum e = (Enum)(enumValue);
            int i = Convert.ToInt32(e);
            return i;
        }

        /// <summary>
        /// Reads the character code of the enum, and returns it in the form of a single-character string.
        /// </summary>
        /// <remarks>
        /// The enum value is required to have a char code, not an int one, for this to work. Technically it
        /// will still work if the value has an int code, the string it returns will just contain the int
        /// cast as a char rather than the int itself, making the result of this method unhelpful to the caller
        /// </remarks>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumChar(Object enumValue)
        {
            Enum e = (Enum)(enumValue);
            int i = Convert.ToInt32(e);
            char c = (char)i;
            return c.ToString();
        }
        
        /// <summary>
        /// Gets a particular Attribute of the enum value, if there is one
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(Object enumValue)
            where TAttribute : Attribute
        {
            string enumValueName = ReadEnumValue(enumValue);
            if(enumValueName == null) return null;
            var valueMember = enumValue.GetType().GetMember(enumValueName)[0];
            var attr = valueMember.GetCustomAttribute<TAttribute>();
            return attr;
        }
        
        /// <summary>
        /// Gets the DisplayAttribute of the enum value, if there is one
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static DisplayAttribute GetDisplayAttribute(Object enumValue)
        {
            return GetAttribute<DisplayAttribute>(enumValue);
        }

        /// <summary>
        /// Reads the DisplayAttribute.Name of the enum, if there is one
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumName(Object enumValue)
        {
            var attr = GetDisplayAttribute(enumValue);
           if(attr == null) return null;
            return attr.Name;
        }
        
        /// <summary>
        /// Reads the DisplayAttribute.Description of the enum, if there is one
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumDescription(Object enumValue)
        {
            var attr = GetDisplayAttribute(enumValue);
           if(attr == null) return null;
            return attr.Description;
        }
         
        /// <summary>
        /// Reads the DisplayAttribute.ShortName of the enum, if there is one
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumShortName(Object enumValue)
        {
            var attr = GetDisplayAttribute(enumValue);
           if(attr == null) return null;
            return attr.ShortName;
        }

        /// <summary>
        /// Finds the name of the enum value, e.g. "General" for CertificationType.General = 'G', that matches a given value (e.g. the char 'G')
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ReadEnumValue(Object enumValue)
        {
            try
            {
                return Enum.GetName(enumValue.GetType(), enumValue);
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Converts from a string that represents the code (e.g. "G" or "5") (single-character if it's non-numeric), back to an enum object
        /// </summary>
        /// <param name="codeString">The code string.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T ToEnum<T>(string codeString)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            if(codeString == null)
                throw new Exception("Cannot pass a null code string to ToEnum()");
            
            if(EnumHasCharValues(typeof(T)))
            {
                if(codeString.Length != 1)
                    throw new Exception(string.Format("The code string for enum {0} must be 1 character in length", typeof(T).Name));
                return FromCharCode<T>(codeString[0]);
            }
            else
            {
                int intCode = 0;
                if(!int.TryParse(codeString, out intCode))
                    throw new Exception(string.Format("The code string for enum {0} must be numeric", typeof(T).Name));
                return FromIntCode<T>(intCode);
            }
        }

        /// <summary>
        /// Cache for GetEnumByName, because reflection is slow
        /// </summary>
        private static Dictionary<Type, Dictionary<string, Enum>> _byNameCache = new Dictionary<Type, Dictionary<string, Enum>>();
        
        /// <summary>
        /// Converts from DisplayAttribute.Name, to enum. Throws an exception if not found, or if there's a duplicate
        /// </summary>
        /// <param name="nameAttribute">The DisplayAttribute.Name</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T GetEnumByName<T>(string nameAttribute)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            /*
            Added this lock to combat the intermittent "Object reference not set to an instance of an object." 
            exception occurring at Void Insert(TKey, TValue, Boolean).
            See the following for more info: 
            https://social.msdn.microsoft.com/Forums/en-US/f7b9c610-3446-4128-bd50-76cfb73cbe0a/systemcollectionsgenericdictionary2inserttkey-key-tvalue-value-boolean-add?forum=csharpgeneral
            */
            lock (_getEnumByNameLock)
            { 
                try
                {
                    Type type = typeof(T);
                    if(!type.IsEnum)
                        throw new Exception(string.Format("Type {0} is not an enum", type.Name));

                    lock (GetEnumByNameLock)
                    {
                        if (!_byNameCache.ContainsKey(type)) _byNameCache[type] = new Dictionary<string, Enum>();
                        if (!_byNameCache[type].ContainsKey(nameAttribute))
                        {
                            var vals = GetEnumValues(type);
                            var entriesFound = new List<object>();
                            foreach (var entry in vals)
                            {
                                var thisNameAttr = ReadEnumName(entry);
                                if (thisNameAttr == nameAttribute) entriesFound.Add(entry);
                            }
                            if (entriesFound.Count == 0)
                                throw new Exception(string.Format("No enum value for type {0} has Name attribute '{1}'", type.Name, nameAttribute));
                            if (entriesFound.Count > 1)
                                throw new Exception(string.Format("Duplicate enum values for type {0} have Name attribute '{1}'", type.Name, nameAttribute));
                            _byNameCache[type][nameAttribute] = (Enum)(entriesFound.First());
                        }
                    }
                    return (T)(Convert.ChangeType(_byNameCache[type][nameAttribute], typeof(T)));
                }
                catch(Exception ex)
                {
                    string typeName = null;
                    try
                    {
                        typeName = (typeof(T)).Name;
                    }
                    #pragma warning disable 0168
                    catch(Exception ex2)
                    {
                        typeName = "(null)";
                    }
                
                    var msg = string.Format("Error parsing enum by Name (type {0}, value {1}): {2}", typeName,
                        nameAttribute ?? "(null)", ex.Dump());
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Cache for GetEnumByShortName, because reflection is slow
        /// </summary>
        private static Dictionary<Type, Dictionary<string, Enum>> _byShortNameCache = new Dictionary<Type, Dictionary<string, Enum>>();
        
        /// <summary>
        /// Converts from DisplayAttribute.ShortName, to enum. Throws an exception if not found, or if there's a duplicate
        /// </summary>
        /// <param name="shortNameAttribute">The DisplayAttribute.Name</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T GetEnumByShortName<T>(string shortNameAttribute)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            try
            {
                Type type = typeof(T);
                if(!type.IsEnum)
                    throw new Exception(string.Format("Type {0} is not an enum", type.Name));
                
                if(!_byShortNameCache.ContainsKey(type)) _byShortNameCache[type] = new Dictionary<string, Enum>();
                if(!_byShortNameCache[type].ContainsKey(shortNameAttribute))
                {
                    var vals = GetEnumValues(type);
                    var entriesFound = new List<object>();
                    foreach(var entry in vals)
                    {
                        var thisNameAttr = ReadEnumShortName(entry);
                        if(thisNameAttr == shortNameAttribute) entriesFound.Add(entry);
                    }
                    if(entriesFound.Count == 0)
                        throw new Exception(string.Format("No enum value for type {0} has ShortName attribute '{1}'", type.Name, shortNameAttribute));
                    if(entriesFound.Count > 1)
                        throw new Exception(string.Format("Duplicate enum values for type {0} have ShortName attribute '{1}'", type.Name, shortNameAttribute));
                    _byShortNameCache[type][shortNameAttribute] = (Enum)(entriesFound.First());
                }
                return (T)(Convert.ChangeType(_byShortNameCache[type][shortNameAttribute], typeof(T)));
            }
            catch(Exception ex)
            {
                string typeName = null;
                try
                {
                    typeName = (typeof(T)).Name;
                }
                catch(Exception ex2)
                {
                    typeName = "(null)";
                }
                
                var msg = string.Format("Error parsing enum by ShortName (type {0}, value {1}): {2}", typeName,
                    shortNameAttribute ?? "(null)", ex.Dump());
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Parses from text that represents the name of the enum, e.g. "AAA" if the enum entry is "AAA = 5".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <param name="caseInsensitive">if set to <c>true</c> it will parse case insensitively.</param>
        /// <param name="returnDefaultOnError">if set to <c>true</c> it will return default() if the entry isn't found or an error occurs.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T ParseFromText<T>(string text, bool caseInsensitive = false, bool returnDefaultOnError = false)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            try
            {
                return (T)(Enum.Parse(typeof(T), text, caseInsensitive));
            }
            catch(Exception ex)
            {
                if(returnDefaultOnError)
                {
                    return default(T);
                }   
                else
                {
                    throw new ArgumentException(string.Format("Enum {0} does not contain entry '{1}'", text ?? "null"));
                }
            }
        }

        /// <summary>
        /// Gets an enum object from its character code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T FromCharCode<T>(char code)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            if(code != 0 && !Enum.IsDefined(typeof(T), (int)code))
                throw new Exception(string.Format("Char code {0} is not valid for enum {1}", code, typeof(T).Name));
            return (T)(Enum.ToObject(typeof(T), code));
        }

        /// <summary>
        /// Gets an enum object from its int code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T FromIntCode<T>(int code)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            if(code != 0 && !Enum.IsDefined(typeof(T), code))
                throw new Exception(string.Format("Int code {0} is not valid for enum {1}", code, typeof(T).Name));
            return (T)(Enum.ToObject(typeof(T), code));
        }

        /// <summary>
        /// Selects an enum entry at random from a given enum type, for unit testing purposes (e.g. a CancellationReasonType)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptThese">Don't allow these entries.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T RandomEntry<T>()
            where T : struct, IConvertible, IComparable, IFormattable
        {
            return RandomEntry<T>(null);
        }

        /// <summary>
        /// Selects an enum entry at random from a given enum type, for unit testing purposes (e.g. a CancellationReasonType)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptThese">Don't allow these entries.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T RandomEntry<T>(params T[] exceptThese)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            
            var list = GetEnumValues(typeof(T));
            if(exceptThese != null)
            {
                list = list.Where(e => !exceptThese.Contains((T)e)).ToList();
                if(!list.Any()) throw new Exception("A random enum entry cannot be selected because all valid entries have been excluded");
            }
            return (T)(list[Random.Next(0, list.Count)]);
        }
        
        /// <summary>
        /// Selects a nullable enum entry at random from a given enum type, for unit testing purposes (e.g. a CancellationReasonType). Is given
        /// a 50% chance of being null
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static T? RandomOrNull<T>()
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            
            T? retValue = null;
            if(Random.Next() % 2 == 0)
                return retValue;
            var list = GetEnumValues(typeof(T));
            retValue = (T)(list[Random.Next(0, list.Count)]);
            return retValue;
        }
        
        /// <summary>
        /// Selects an enum entry at random from a given enum type, for unit testing purposes (e.g. a CancellationReasonType)
        /// </summary>
        /// <remarks>
        /// This overload is needed for when the type is not known at compile time
        /// </remarks>
        /// <param name="type">Type of the enum.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Object RandomEntry(Type type)
        {
            return RandomEntry(type, (string[])null);
        }
        
        /// <summary>
        /// Selects an enum entry at random from a given enum type, for unit testing purposes (e.g. a CancellationReasonType)
        /// </summary>
        /// <remarks>
        /// This overload is needed for when the type is not known at compile time
        /// </remarks>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="exceptThese">Don't allow these entries.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Object RandomEntry(Type type, params string[] exceptThese)
        {
            if(!type.IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", type.Name));
            
            var list = GetEnumValues(type);
            if(exceptThese != null)
            {
                list = list.Where(e => !exceptThese.Contains(e.ToString())).ToList();
                if(!list.Any()) throw new Exception("A random enum entry cannot be selected because all valid entries have been excluded");
            }
            return list[Random.Next(0, list.Count)];
        }

        /// <summary>
        /// Gets the values for an enum as a List<Object>
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="excludeHiddenValues">if set to <c>true</c> [exclude hidden values].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static List<Object> GetEnumValues(Type enumType, bool excludeHiddenValues = false)
        {
            if(!enumType.IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", enumType.Name));
            
            Array values = Enum.GetValues(enumType);
            var list = new List<Object>();
            for(int i = 0; i < values.Length; i++)
                list.Add(values.GetValue(i));
            
            if(excludeHiddenValues)
                list = list.Where(i => !IsHiddenEnumValue(i)).ToList();
            
            return list;
        }

        /// <summary>
        /// Gets the values for an enum as a generic List. Requires the type to be known at compile time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excludeHiddenValues">if set to <c>true</c> [exclude hidden values].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static List<T> GetEnumValues<T>(bool excludeHiddenValues = false)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            
            Array values = Enum.GetValues(typeof(T));
            var list = new List<T>();
            for(int i = 0; i < values.Length; i++)
                list.Add((T)(values.GetValue(i)));
            
            if(excludeHiddenValues)
                list = list.Where(i => !IsHiddenEnumValue(i)).ToList();
            
            return list;
        }

        /// <summary>
        /// Determines whether an enum value is hidden, as defined by having a [JsonIgnore] attribute on it
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        ///   <c>true</c> or <c>false</c>.
        /// </returns>
        public static bool IsHiddenEnumValue(Object enumValue)
        {
            return (GetAttribute<JsonIgnoreAttribute>(enumValue) != null);
        }
        
        /// <summary>
        /// Turns an Enum object into its string representation (e.g. "AAA" for AAA='A')
        /// </summary>
        /// <remarks>
        /// The advantage of having this method, which appears to just be a wrapper, is that
        /// all objects inherit ToString(). Calling this ensures that you're calling it on a valid
        /// (enum type) object, otherwise a compilation error or exception will result
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string Value<T>(T enumEntry)
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(T).IsEnum)
                throw new Exception(string.Format("Type {0} is not an enum", typeof(T).Name));
            return enumEntry.ToString();
        }
        
        /// <summary>
        /// Determines whether the specified type T is enum.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///   <c>true</c> if the specified t is enum; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static bool IsEnum(Type t)
        {
            if(t == null) throw new Exception("Null type given to EnumAttributes.IsEnum()");
            return t.IsEnum;
        }
    }
}
