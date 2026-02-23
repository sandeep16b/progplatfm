using Moq;
using NLog;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// Tests the logging
    /// </summary>
    public class LogTest
    {
        /// <summary>
        /// Gets or sets the traces.
        /// </summary>
        /// <value>
        /// The traces.
        /// </value>
        public List<string> Traces { get; set; }

        /// <summary>
        /// Gets or sets the infos.
        /// </summary>
        /// <value>
        /// The infos.
        /// </value>
        public List<string> Infos { get; set; }

        /// <summary>
        /// Gets or sets the debugs.
        /// </summary>
        /// <value>
        /// The debugs.
        /// </value>
        public List<string> Debugs { get; set; }

        /// <summary>
        /// Gets or sets the warns.
        /// </summary>
        /// <value>
        /// The warns.
        /// </value>
        public List<string> Warns { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [output logs].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [output logs]; otherwise, <c>false</c>.
        /// </value>
        public bool OutputLogs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTest"/> class.
        /// </summary>
        public LogTest()
        {
            Traces = new List<string>();
            Infos = new List<string>();
            Debugs = new List<string>();
            Warns = new List<string>();
            Errors = new List<string>();
        }

        /// <summary>
        /// Watches the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        public void Watch(Mock<ILogger> log)
        {
            //Traces
            log.Setup(o => o.Trace(It.IsAny<string>()))
                .Callback<string>((message) =>
                {
                    Traces.Add(message);
                    if(OutputLogs) Console.WriteLine(message);
                })
                .Verifiable();
            
            //Infos
            log.Setup(o => o.Info(It.IsAny<string>()))
                .Callback<string>((message) =>
                {
                    Infos.Add(message);
                    if(OutputLogs) Console.WriteLine(message);
                })
                .Verifiable();
            
            //Debugs
            log.Setup(o => o.Debug(It.IsAny<string>()))
                .Callback<string>((message) =>
                {
                    Debugs.Add(message);
                    if(OutputLogs) Console.WriteLine(message);
                })
                .Verifiable();
            log.Setup(o => o.Debug(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((message, arg) =>
                {
                    try
                    {
                        Debugs.Add(string.Format(message, arg));
                        if(OutputLogs) Console.WriteLine(string.Format(message, arg));
                    }
                    catch(Exception ex){ /*ignore*/ }
                })
                .Verifiable();
            log.Setup(o => o.Debug(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, args) =>
                {
                    try
                    {
                        Debugs.Add(string.Format(message, args));
                        if(OutputLogs) Console.WriteLine(string.Format(message, args));
                    }
                    catch(Exception ex){ /*ignore*/ }
                })
               .Verifiable();
            log.Setup(o => o.Debug(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<string, Guid, string>((message, arg1, arg2) =>
                {
                    try
                    {
                        var debugMessage = string.Format(message, arg1, arg2);
                        Debugs.Add(debugMessage);
                        if (OutputLogs) Console.WriteLine(debugMessage);
                    }
                    catch (Exception ex) { /*ignore*/ }
                })
               .Verifiable();

            //Warns
            log.Setup(o => o.Warn(It.IsAny<string>()))
                .Callback<string>((message) =>
                {
                    Warns.Add(message);
                    if(OutputLogs) Console.WriteLine(message);
                })
                .Verifiable();
            log.Setup(o => o.Warn(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((message, arg) =>
                {
                    try
                    {
                        Warns.Add(string.Format(message, arg));
                        if(OutputLogs) Console.WriteLine(string.Format(message, arg));
                    }
                    catch(Exception ex){ /*ignore*/ }
                })
                .Verifiable();
            log.Setup(o => o.Warn(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, args) =>
                {
                    try
                    {
                        Warns.Add(string.Format(message, args));
                        if(OutputLogs) Console.WriteLine(string.Format(message, args));
                    }
                    catch(Exception ex){ /*ignore*/ }
                })
                .Verifiable();
            
            //Errors
            log.Setup(o => o.Error(It.IsAny<Exception>()))
                .Callback<Exception>((ex) =>
                {
                    if(ex != null) Errors.Add(ex.Message);
                    if(OutputLogs) Console.WriteLine(ex.Message);
                })
               .Verifiable();
            //Errors
            log.Setup(o => o.Error(It.IsAny<string>()))
                .Callback<string>((message) =>
                {
                    Errors.Add(message);
                    if(OutputLogs) Console.WriteLine(message);
                })
                .Verifiable();
            //Errors
            log.Setup(o => o.Error(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((message, arg1, arg2) =>
                {
                    var errorMessage = string.Format(message, arg1, arg2);
                    Errors.Add(errorMessage);
                    if (OutputLogs) Console.WriteLine(errorMessage);
                })
                .Verifiable();
        }
    }
}
