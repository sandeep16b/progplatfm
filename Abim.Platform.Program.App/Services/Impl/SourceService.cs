using System.Linq;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Hangfire;
using MassTransit;
using NLog;


namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class SourceService
    /// </summary>
    public class SourceService : ServiceBase<Source, ISourceRepository>, ISourceService
    {
        /// <summary>
        /// The code to find the default Source
        /// </summary>
        private const string ABIMCode = "ABIM";
        
        #region Properties
        
        /// <summary>
        /// The logger is inherited from the base class. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceService"/> class.
        /// </summary>
        /// <param name="sourceRepository">The source repository.</param>
        /// <param name="bus">The bus.</param>
        /// <param name="jobClient">The job client.</param>
        /// <param name="validationFactory">The validation factory.</param>
        public SourceService(ISourceRepository sourceRepository,
                             IBusControl bus,
                             IBackgroundJobClient jobClient,
                             IValidationFactory validationFactory)
            : base(bus, sourceRepository, jobClient, validationFactory)
        {
            //Log.Trace("SourceService ~ctor :: Begin");
            
            //Log.Trace("SourceService ~ctor :: End");
        }
        
        #region Custom Get Methods

        /// <summary>
        /// Gets the abim source.
        /// </summary>
        /// <returns></returns>
        public Source GetAbimSource()
        {
            return Repository.Query(o => o.Code == ABIMCode).SingleOrDefault();
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the child services.
        /// </summary>
        protected override void DisposeChildServices()
        {
        }

        #endregion
    }
}
