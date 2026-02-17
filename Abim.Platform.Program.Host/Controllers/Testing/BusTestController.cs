using Abim.Enterprise.Core.Resource.Program;
using Abim.Enterprise.Core.Resource.Registration;
using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Enterprise.Core.WebApi;
using Abim.Enterprise.Core.WebApi.Api.Attributes;
using Abim.Enterprise.Core.WebApi.Attributes;
using Abim.Enterprise.Core.WebApi.Exceptions;
using Abim.Enterprise.Core.WebApi.Extensions;
using Abim.Enterprise.Core.WebApi.Objects;
using Abim.Enterprise.Core.WebApi.Util.General.Json;
using MassTransit;
using System;
using System.Net;
using System.Web.Http;

namespace Abim.Platform.Registration.Host.Controllers
{
    /// <summary>
    /// Controller for AdaAccommodation data
    /// </summary>
    //[HideFromSwagger]
    //[RoutePrefix(RegistrationResourceConstants.ApiInfo.RoutePrefix)]
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.ProgramRules)]
    public class BusTestController : ControllerBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the bus control.
        /// </summary>
        /// <value>
        /// The bus control.
        /// </value>
        private IBusControl _busControl { get; set; }

        /// <summary>
        /// Enum service
        /// </summary>
        private IEnumService _enumService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BusTestController"/> class.
        /// </summary>
        /// <param name="busControl">The bus control.</param>
        /// <param name="enumService">The enum service.</param>
        public BusTestController(IBusControl busControl, IEnumService enumService)
        {
            _busControl = busControl;
            _enumService = enumService;
        }

        /// <summary>
        /// Publishes any ServiceBus library event. All properties in the posted json string are optional
        /// </summary>
        /// <param name="eventName">Class name of the event.</param>
        /// <param name="data">The event as json. All properties in it are optional. Even the Body itself is optional.</param>
        /// <returns></returns>
        [HttpPost]
        [HideFromSwagger]
        //[RequiresHttps, Authorize]
        [Route("Publish/{eventName}", Name = "Publish")]
        public IHttpActionResult Publish(string eventName, [FromBody] string data)
        {
            var @sample = new ApplicationAcceptedEvent();       //to load the assembly
            try
            {
                var @event = JsonData.Parse(eventName, data);
                _busControl.Publish(@event);
                return Ok(@event);
            }
            catch (InputException ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Stringify());
            }
        }
    }
}
