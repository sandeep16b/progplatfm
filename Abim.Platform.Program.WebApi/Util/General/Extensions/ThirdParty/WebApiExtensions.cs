using Abim.Platform.Program.Util;
using Abim.Platform.Program.Util.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.WebApi.Objects.Extensions
{
    /// <summary>
    /// Some of these are intended for unit tests, to check whether links coming back from system endpoints are the expected ones, etc
    /// (we must use reflection for that because a unit test cannot test whether someone ever forgets to add a link, unless we're
    /// assuming they NEVER forget to update the unit test itself when they add an endpoint, which if they're forgetting to add code
    /// to the app/host project itself is undoubtedly a faulty assumption)
    /// </summary>
    public static class WebApiExtensions
    {
        /// <summary>
        /// Gets the request body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string GetRequestBody(this HttpRequestMessage message)
        {
            //from the BodyStorageHandler:
            if(message.Properties.ContainsKey("body"))
            {
                var body = message.Properties["body"];
                if(body == string.Empty) return null;
                return null;
            }
            
            if(HttpContext.Current != null)
            {
                if(HttpContext.Current.Request.InputStream.CanSeek)
                    HttpContext.Current.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                string content = null;
                using(var reader = new System.IO.StreamReader(HttpContext.Current.Request.InputStream))
                {
                    content = reader.ReadToEnd();
                }
                return content;
            }
            
            return null;
        }

        /// <summary>
        /// Gets the route names.
        /// </summary>
        /// <param name="routeCollection">The route collection.</param>
        /// <returns></returns>
        public static IList<string> RouteNames(this HttpRouteCollection routeCollection)
        {
            object routes = routeCollection;
            var fieldName = "_dictionary";
            if(routes == null) return new List<string>();
            
            var type = routes.GetType();
            if(type.FullName == "System.Web.Http.WebHost.Routing.HostedHttpRouteCollection")
            {
                var hostedField = type.GetField("_routeCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                type = hostedField.FieldType;
                routes = hostedField.GetValue(routes);
                fieldName = "_namedMap";
            }
            
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var dictionary = field.GetValue(routes) as IDictionary;
            return dictionary == null ? new List<string>() : dictionary.Keys.Cast<string>().Where(s => s != "MS_attributerouteWebApi").ToList();
        }

        /// <summary>
        /// Checks if a route has parameters.
        /// </summary>
        /// <param name="routeCollection">The route collection.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static bool RouteHasParameters(this HttpRouteCollection routeCollection, string routeName)
        {
            var route = routeCollection[routeName];
            if(route == null) throw new Exception(string.Format("Route {0} does not exist", routeName));
            return route.RouteTemplate.Contains("{");
        }

        /// <summary>
        /// Gets the route method.
        /// </summary>
        /// <param name="routeCollection">The route collection.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <returns></returns>
        public static HttpVerbs GetRouteMethod(this HttpRouteCollection routeCollection, string routeName)
        {
            foreach(var type in ControllerTypes)
            {
                foreach(var method in type.GetMethods())
                {
                    var routeAttrs = method.GetCustomAttributes(false).Where(a => a.GetType() == typeof(RouteAttribute));
                    if(!routeAttrs.Any(a => ((RouteAttribute)(a)).Name == routeName)) continue;
                    
                    var methodAttr = method.GetCustomAttributes(false).FirstOrDefault(a => typeof(IActionHttpMethodProvider).IsAssignableFrom(a.GetType()));
                    if(methodAttr == null) continue;
                    
                    if(methodAttr is HttpGetAttribute) return HttpVerbs.Get;
                    if(methodAttr is HttpPostAttribute) return HttpVerbs.Post;
                    if(methodAttr is HttpPutAttribute) return HttpVerbs.Put;
                    if(methodAttr is HttpDeleteAttribute) return HttpVerbs.Delete;
                    if(methodAttr is HttpOptionsAttribute) return HttpVerbs.Options;
                    if(methodAttr is HttpPatchAttribute) return HttpVerbs.Patch;
                    return default(HttpVerbs);
                }
            }
            return default(HttpVerbs);
        }

        private static List<Type> _controllerTypes;

        /// <summary>
        /// Gets the controller types.
        /// </summary>
        /// <value>
        /// The controller types.
        /// </value>
        /// <exception cref="System.Exception"></exception>
        public static List<Type> ControllerTypes
        {
            get
            {
                try
                {
                    if(_controllerTypes == null)
                    {
                        _controllerTypes = new List<Type>();
                        _controllerTypes.AddRange(TypeExtensions.SearchTypes(t => typeof(ControllerBase).IsAssignableFrom(t)));
                    }
                    return _controllerTypes;
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Stringify());
                }
            }
        }

        /// <summary>
        /// Gets the main controller types.
        /// </summary>
        /// <value>
        /// The main controller types.
        /// </value>
        public static List<Type> MainControllerTypes
        {
            get
            {
                return ControllerTypes.Where(c => !c.Name.Contains("Base") && !c.Name.Contains("Mock") && !c.Name.Contains("AppInfo")).ToList();
            }
        }
 
        /// <summary>
        /// Generates the base API links.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <remarks>
        /// this will also include links to App endpoints    
        /// </remarks>
        public static List<Tuple<Link, IHttpRoute>> GenerateBaseApiLinks(this ControllerBase controller)
        {
            if(controller == null || controller.Configuration == null || controller.Configuration.Routes == null)
                return new List<Tuple<Link, IHttpRoute>>();
            var routeNames = controller.Configuration.Routes.RouteNames();
            var linkData = new List<Tuple<Link, IHttpRoute>>();
            foreach(var name in routeNames)
            {
                var route = controller.Configuration.Routes[name];
                if(name.Contains("swagger") || controller.Configuration.Routes.RouteHasParameters(name)) continue;
                var link = new Link()
                {
                    Name = name,
                    Href = controller.Url.Link(name, null),
                    Method = controller.Configuration.Routes.GetRouteMethod(name).ToString()
                };
                linkData.Add(new Tuple<Link, IHttpRoute>(link, route));
            }
            return linkData;
        }

        /// <summary>
        /// Generates the link creation code.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="constantsClass">The constants class.</param>
        /// <param name="ignoreSystemApiLinks">if set to <c>true</c> [ignore system API links].</param>
        /// <returns></returns>
        /// <example>
        /// <code language="C#" title="Example Usage">
        /// <![CDATA[
        ///
        ///    var code = WebApiExtensions.GenerateLinkCreationCode(this, typeof(ProductResourceConstants), true);
        ///
        /// ]]>
        /// </code>
        /// </example>
        public static string GenerateLinkCreationCode(this ControllerBase controller, Type constantsClass, bool ignoreSystemApiLinks)
        {
            return GenerateLinkCreationCode(controller.GenerateBaseApiLinks(), constantsClass, ignoreSystemApiLinks);
        }

        /// <summary>
        /// Generates the link creation code.
        /// </summary>
        /// <param name="linkData">The link data.</param>
        /// <param name="constantsClass">The constants class.</param>
        /// <param name="ignoreSystemApiLinks">if set to <c>true</c> [ignore system API links].</param>
        /// <returns></returns>
        public static string GenerateLinkCreationCode(this List<Tuple<Link, IHttpRoute>> linkData, Type constantsClass, bool ignoreSystemApiLinks)
        {
            var lines = new List<string>();
            var warningEndpoints = new List<string>();
            foreach(var linkItem in linkData)
            {
                var constName = FieldName(constantsClass, linkItem.Item1.Name);
                if(string.IsNullOrEmpty(constName))
                {
                    warningEndpoints.Add(linkItem.Item1.Name);
                    continue;
                }
                if(ignoreSystemApiLinks && (constName.Contains("RouteNames.System.") || constName.Contains("RouteNames.App.")))
                    continue;
                lines.Add("\t\t\tlinks.Add(new Link()");
                lines.Add("\t\t\t{");
                lines.Add("\t\t\t\tName = " + constName + ",");
                lines.Add("\t\t\t\tHref = Url.Link(" + constName + ", null),");
                lines.Add("\t\t\t\tMethod = HttpVerbs." + linkItem.Item1.Method.ToString() + ".ToString().ToUpper()");
                lines.Add("\t\t\t});");
            }
            if(warningEndpoints.Any())
            {
                lines.Add("\t\t\t");
                lines.Add("\t\t\t//Warning: the following routes do not use constants, and therefore may not be intended for exposure. Due to this they are not");
                lines.Add("\t\t\t//... included in the links collection here: " + string.Join(", ", warningEndpoints.ToArray()));
            }
            return string.Join("\r\n", lines.ToArray()).Replace("\t", "    ");
        }

        /// <summary>
        /// Gets a field name.
        /// </summary>
        /// <param name="constantsClass">The constants class.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        private static string FieldName(Type constantsClass, string fieldValue)
        {
            var types = new List<Tuple<Type, string>>();
            CollectTypes(constantsClass, "", types);
            return FieldName(types, fieldValue);
        }

        /// <summary>
        /// Gets a field name.
        /// </summary>
        /// <param name="typeData">The type data.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        private static string FieldName(List<Tuple<Type, string>> typeData, string fieldValue)
        {
            foreach(var typeItem in typeData)
            {
                var fields = typeItem.Item1.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.IsLiteral && f.FieldType == typeof(string)).ToList();
                var fieldFound = fields.FirstOrDefault(f => f.GetRawConstantValue().ToString() == fieldValue);
                if(fieldFound != null) return typeItem.Item2 + "." + fieldFound.Name;
            }
            return "";
        }

        /// <summary>
        /// Collects the types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeParentageNotation">The type parentage notation.</param>
        /// <param name="collection">The collection.</param>
        private static void CollectTypes(Type type, string typeParentageNotation, List<Tuple<Type, string>> collection)
        {
            var fullPath = typeParentageNotation + type.Name;
            collection.Add(new Tuple<Type, string>(type, fullPath));
            foreach(var t in type.GetNestedTypes().ToList())
                CollectTypes(t, fullPath + ".", collection);
        }

        /// <summary>
        /// Determines whether the specified method has the specified verb.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="verb">The verb.</param>
        /// <returns>
        ///   <c>true</c> if the specified verb has verb; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasVerb(this MethodInfo methodInfo, HttpVerbs verb)
        {
            switch(verb)
            {
                case HttpVerbs.Get:      return methodInfo.GetCustomAttribute<HttpGetAttribute>() != null;
                case HttpVerbs.Post:     return methodInfo.GetCustomAttribute<HttpPostAttribute>() != null;
                case HttpVerbs.Put:      return methodInfo.GetCustomAttribute<HttpPutAttribute>() != null;
                case HttpVerbs.Options:  return methodInfo.GetCustomAttribute<HttpOptionsAttribute>() != null;
                default:                 return false;
            }
        }
    }
}
