using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Abim.Platform.Program.WebApi.Utilities
{
    /// <summary>
    /// ExternalAssemblyResolver
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExternalAssemblyResolver<T>
        : DefaultAssembliesResolver
    {
        /// <summary>
        /// GetAssemblies
        /// </summary>
        /// <returns></returns>
        public override ICollection<Assembly> GetAssemblies()
        {
            var baseAssemblies = base.GetAssemblies().ToList();
            var assemblies = new List<Assembly>(baseAssemblies) { typeof(T).Assembly };
            var externalControllers = typeof(T).Assembly;
            return assemblies.Distinct().ToList();
        }
    }
}