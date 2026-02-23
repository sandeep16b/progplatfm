using Abim.Platform.Program.Util.Extensions;
using System;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Objects.Mapping
{
    /// <summary>
    /// Chris's MultiMapper class. This class maps a series of objects onto a single object of a supplied type (TOutput).
    /// For example, we can map multiple objects sequentially onto a cddRequestType object. See usage in Registration,
    /// where this class was originally located
    /// </summary>
    public static class MultiMapper
    {
        /// <summary>
        /// AutoMapper backing field
        /// </summary>
        private static Type _autoMapper;

        /// <summary>
        /// AutoMapper Type.
        /// </summary>
        /// <value>
        /// The automatic mapper.
        /// </value>
        private static Type AutoMapper
        {
            get
            {
                if(_autoMapper == null)
                    _autoMapper = TypeExtensions.SearchTypes(t => t.Name == "Mapper" && t.AssemblyQualifiedName.Contains("AutoMapper")).First();
                return _autoMapper;
            }
        }

        /// <summary>
        /// Does a multi-step map.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="sources">The sources.</param>
        /// <returns></returns>
        public static TOutput Map<TOutput>(params object[] sources)
        {
            return Map<TOutput>(AutoMapper, sources);
        }

        /// <summary>
        /// Does a map with the AutoMapper type specified by the caller.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="sources">The sources.</param>
        /// <returns></returns>
        public static TOutput Map<TOutput>(Type autoMapper, params object[] sources)
        {
            var methods = autoMapper.GetMethods().Where(m => m.Name == "Map").ToList();
            var createMethod = methods.First(m => m.GetParameters().Count() == 1 && m.GetParameters().First().ParameterType == typeof(object));
            var updateMethod = methods.First(m => m.GetParameters().Count() == 2 && !m.GetParameters().ToList()[1].ParameterType.Name.Contains("Action"));
            Func<object, TOutput> createFunc =
                (src) => (TOutput)(createMethod.MakeGenericMethod(typeof(TOutput)).Invoke(null, new[]{ src }));
            Func<object, TOutput, TOutput> updateFunc =
                (src, dest) => (TOutput)(updateMethod.MakeGenericMethod(src.GetType(), typeof(TOutput)).Invoke(null, new[]{ src, dest }));
            return Map<TOutput>(createFunc, updateFunc, sources);
        }

        /// <summary>
        /// Does a map with functions specified by the caller.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="mapCreate">The map create.</param>
        /// <param name="mapUpdate">The map update.</param>
        /// <param name="sources">The sources.</param>
        /// <returns></returns>
        public static TOutput Map<TOutput>(Func<object, TOutput> mapCreate, Func<object, TOutput, TOutput> mapUpdate,
            params object[] sources)
        {
            if(!sources.Any())
                return default(TOutput);
            
            var destination = mapCreate(sources[0]);
            
            if(sources.Length <= 1)
                return destination;
            
            foreach (var source in sources.Skip(1))
            {
                if(source == null)
                    continue;
                
                mapUpdate(source, destination);
            }
            
            return destination;
        }
    }
}
