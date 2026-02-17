using Abim.Platform.Program.Relational;
using StructureMap;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// SmServiceActivator Class.
    /// </summary>
    public class SmServiceActivator
        : IHttpControllerActivator
    {
        private IContainer _container;
        
        /// <summary>
        /// SmServiceActivator Constructor
        /// </summary>
        /// <param name="container"></param>
        public SmServiceActivator(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="request"></param>
        /// <param name="controllerDescriptor"></param>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if(_container == null)
            {
                if(DependencyResolver.Container == null) throw new Exception("Container is null in SmServiceActivator");
                else _container = DependencyResolver.Container;
            }
            var controller = _container.GetInstance(controllerType) as IHttpController;
            if(controller == null)
                throw new InvalidOperationException("{0} could not be retrieved from the object factory. Please insure");
            return controller;
        }
    }
}
