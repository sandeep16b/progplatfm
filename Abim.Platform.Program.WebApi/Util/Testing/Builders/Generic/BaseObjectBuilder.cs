using Abim.Platform.Program.Relational.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Base class for ObjectBuilders
    /// </summary>
    public abstract class BaseObjectBuilder<T, TOwnType> : ObjectBuilderType
        where T : new()
        where TOwnType : BaseObjectBuilder<T, TOwnType>, new()
    {
        #region Properties
        
        /// <summary>
        /// The custom sets
        /// </summary>
        protected List<Action<T>> CustomSets = new List<Action<T>>();

        #endregion

        /// <summary>
        /// Starts with all types planned to be invalid once the object is built.
        /// </summary>
        /// <returns></returns>
        public static TOwnType Invalid()
        {
            return new TOwnType()
            {
                ValidGuids = false,
                ValidEnums = false,
                ValidStrings = false,
                ValidIntegers = false,
                ValidDateTimes = false,
                ValidLists = false
            };
        }
        
        /// <summary>
        /// Starts with all types planned to be valid once the object is built.
        /// </summary>
        /// <returns></returns>
        public static TOwnType Valid()
        {
            return new TOwnType();
        }

        /// <summary>
        /// Specifies that guids should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidGuids()
        {
            ValidGuids = true;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that enums should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidEnums()
        {
            ValidEnums = true;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that strings should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidStrings()
        {
            ValidStrings = true;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that ints should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidIntegers(bool allowZero = true)
        {
            ValidIntegers = true;
            AllowZeroInIntegers = allowZero;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that DateTimes should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidDateTimes()
        {
            ValidDateTimes = true;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that lists should be valid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithValidLists()
        {
            ValidLists = true;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that guids should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidGuids()
        {
            ValidGuids = false;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that enums should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidEnums()
        {
            ValidEnums = false;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that strings should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidStrings()
        {
            ValidStrings = false;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that ints should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidIntegers()
        {
            ValidIntegers = false;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that DateTimes should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidDateTimes()
        {
            ValidDateTimes = false;
            return (TOwnType)(this);
        }
        
        /// <summary>
        /// Specifies that lists should be invalid.
        /// </summary>
        /// <returns></returns>
        public TOwnType WithInvalidLists()
        {
            ValidLists = false;
            return (TOwnType)(this);
        }

        /// <summary>
        /// The same as WithValidIntegers(), but with a method name emphasizing what it is doing for use in a context in which there
        /// are no calls to .WithValid__
        /// </summary>
        /// <returns></returns>
        public TOwnType WithNoZeroIntegers()
        {
            return WithValidIntegers(false);
        }

        /// <summary>
        /// Adds an action to set something on the resultant object.
        /// </summary>
        /// <example>
        /// <code language="C#" title="Example Usage">
        /// <![CDATA[   
        ///       
        ///     //Note: this example is using the child class, CommandBuilder<T>
        ///     var command = CommandBuilder<UpdateNonAccmeProductCommand>
        ///                     .Valid()
        ///                     .With(cmd => cmd.EffectiveStartDate = DateTime.Now.AddDays(fromNow))
        ///                     .With(cmd => cmd.EffectiveEndDate = DateTime.Now.AddDays(fromNow).AddDays(-1 * offset))
        ///                     .Build();
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public TOwnType With(Action<T> action)
        {
            CustomSets.Add(action);
            return (TOwnType)(this);
        }

        /// <summary>
        /// Builds the object.
        /// </summary>
        /// <returns></returns>
        public T Build()
        {
            T targetObject = new T();
            foreach(var prop in typeof(T).GetProperties().Where(p => p.CanWrite && p.GetSetMethod(true).IsPublic))
            {
                switch(prop.PropertyType.Name)
                {
                    case "Int32":       prop.SetMethod.Invoke(targetObject, new object[]{ GetInteger() });
                                        break;
                    case "String":      prop.SetMethod.Invoke(targetObject, new object[]{ GetString() });
                                        break;
                    case "DateTime":    prop.SetMethod.Invoke(targetObject, new object[]{ GetDateTime() });
                                        break;
                    case "Guid":        prop.SetMethod.Invoke(targetObject, new object[]{ GetGuid() });
                                        break;
                    case "Boolean":     prop.SetMethod.Invoke(targetObject, new object[]{ RandomBool.IsTrue });
                                        break;
                    default:            if(prop.PropertyType.IsEnum)
                                        {
                                            prop.SetMethod.Invoke(targetObject, new[]{ EnumAttributes.RandomEntry(prop.PropertyType) });
                                        }
                                        else if(prop.PropertyType.Name.Contains("IReadOnlyList`1"))
                                        {
                                            var type = prop.PropertyType.GetGenericArguments()[0];
                                            var list = GetList(type);
                                            if(list != null)
                                            {
                                                prop.SetMethod.Invoke(targetObject, new object[]{ (list as dynamic).AsReadOnly() });
                                            }
                                        }
                                        else if(prop.PropertyType.Name.Contains("List`1"))
                                        {
                                            var type = prop.PropertyType.GetGenericArguments()[0];
                                            var list = GetList(type);
                                            if(list != null)
                                            {
                                                prop.SetMethod.Invoke(targetObject, new[]{ list });
                                            }
                                        }
                                        break;
                }
            }
            foreach(var action in CustomSets)
                action(targetObject);
            return targetObject;
        }
    }
}
