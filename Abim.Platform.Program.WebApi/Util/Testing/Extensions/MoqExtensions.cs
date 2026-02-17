using Abim.Platform.Program.Util.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abim.Platform.Program.WebApi.Util.Testing.Extensions
{
    public static class MoqExtensions
    {
        /// <summary>
        /// It's often annoying to have to mock every single called IEnumerable and List method on your objects, even when you don't care at all what they return,
        /// if anything. Having this method makes unit tests much shorter without having to have 5 or 6 boilerplate ".Setup()" calls to return empty lists yourself.
        /// </summary>
        /// <param name="mockObject">The mock object.</param>
        public static void AutoMockAllEnumerableMethodsToReturnEmptyLists<TInterface>(Mock<TInterface> mock)
            where TInterface : class
        {   
            //collect the methods on the interface that we're trying to mock
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var methodsToMock = typeof(TInterface).GetMethods(flags).Where(m => m.ReturnType.Name.Contains("List`1") || m.ReturnType.Name.Contains("IEnumerable`1"));
            
            foreach(var method in methodsToMock)
            {
                Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                var methodToGenerateTheExpression = typeof(TypeExtensions).GetMethod("ExpressionToCallMethod", BindingFlags.Static)
                    .MakeGenericMethod(typeof(TInterface), method.ReturnType);
                
                //this is now an Expression<Func<TClass, TReturnType>> which is what Mock.Setup() wants
                dynamic callExpression = methodToGenerateTheExpression.Invoke(null, new object[]{ method.Name, parameterTypes });
                
                dynamic iSetup = mock.Setup(callExpression);
                iSetup.Returns((typeof(List<>).MakeGenericType(method.ReturnType.GetGenericArguments()[0]).GetConstructor(new Type[0]).Invoke(new object[0])));
            }
        }
    }
}
