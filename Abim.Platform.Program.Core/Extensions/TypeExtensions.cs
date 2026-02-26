using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    //Updated from the original shared version
    public static class TypeExtensions
    {
        /// <summary>
        /// Searches types by predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static List<Type> SearchTypes(Func<Type, bool> predicate)
        {
            var list = new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.SafeGetTypes();
                    list.AddRange(types.Where(predicate));
                }
                #pragma warning disable 0168
                catch(Exception ex1)
                {
                    //doesn't matter
                }
            }
            return list;
        }
        
        /// <summary>
        /// Gest all types in an assembly that doesn't throw a load exception.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static IEnumerable<Type> SafeGetTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().Where(t => t != null);
            }
            catch(ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
            catch(Exception ex)
            {
                return new List<Type>();
            }
        }
        
        /// <summary>
        /// Determines whether this type is an attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAttribute(this Type type)
        {
            return typeof(Attribute).IsAssignableFrom(type);
        }
        
        /// <summary>
        /// Gets a better readable Type name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string ReadableName(this Type type)
        {
            if(!type.IsGenericType) return type.Name;
            else return string.Format("{0}<{1}>", type.Name, string.Join(", ", type.GetGenericArguments().Select(t => t.ReadableName()).ToArray()));
        }
        
        /// <summary>
        /// Gets a type by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="InputException">
        /// No class of that name was found
        /// or
        /// More than one class of that name was found
        /// </exception>
        public static Type GetType(string name)
        {
            List<Type> exactTypeMatches = SearchTypes(t => t.Name.ToLower() == name.ToLower());
            if(exactTypeMatches.Count == 1)
                return exactTypeMatches.Single();
            List<Type> typeMatches = SearchTypes(t => t.Name.ToLower().Contains(name.ToLower()));
            if(typeMatches.Count == 0)
                throw new InputException("No class of that name was found");
            if(typeMatches.Count > 1)
                throw new InputException("More than one class of that name was found");
            var type = typeMatches.Single();
            return type;
        }
        
        /// <summary>
        /// Returns a expression of a property get
        /// </summary>
        /// <example>
        /// <code language="C#" title="Example Usage">
        /// <![CDATA[
        ///
        ///    var func = ExpressionToGetProperty<Foo, string>("Bar");
        ///    //this would return the expression f => f.Bar
        ///
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns></returns>
        public static Expression<Func<TClass, TProperty>> ExpressionToGetProperty<TClass, TProperty>(string propertyName)
        {
            ParameterExpression value = Expression.Parameter(typeof(TClass), "value");
            Expression setupProperty = Expression.Property(value, propertyName);
            var func = Expression.Lambda<Func<TClass, TProperty>>(setupProperty, value);
            return func;
        }
        
        /// <summary>
        /// Returns a expression of a method invocation
        /// </summary>
        /// <example>
        /// <code language="C#" title="Example Usage">
        /// <![CDATA[
        ///
        ///    var func = ExpressionToCallMethod<Foo, string>("Bar", new Type[]{ typeof(int), typeof(bool) });
        ///    //this would return f => f.Bar(i, b)
        ///
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <typeparam name="TReturnType">The type of the return type.</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public static Expression<Func<TClass, TReturnType>> ExpressionToCallMethod<TClass, TReturnType>(string methodName, Type[] parameterTypes,
            Object[] parameterValues)
        {
            ParameterExpression value = Expression.Parameter(typeof(TClass), "value");
            var methodInfo = typeof(TClass).GetMethod(methodName, parameterTypes);
            if (methodInfo != null)
            {
                Expression setupCall = Expression.Call(value, methodInfo, parameterValues.Select(p => Expression.Constant(p)).ToArray<Expression>());
                var func = Expression.Lambda<Func<TClass, TReturnType>>(setupCall, value);
                return func;
            }
            return null;
        }
    }
}
