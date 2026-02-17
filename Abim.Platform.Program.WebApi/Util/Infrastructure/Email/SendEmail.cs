using Abim.Platform.Program.Relational.Validation.Regex;
using Abim.Platform.Program.Util.Extensions;
using NLog;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Abim.Platform.Program.WebApi.Util.Infrastructure.Email
{
    /// <summary>
    /// Email-sending static class, using SMTP
    /// </summary>
    public static class SendEmail
    {
        #region Fields
        
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Gets the error email address.
        /// </summary>
        /// <value>
        /// The error email address.
        /// </value>
        public static string ErrorEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["ErrorEmailAddress"] ?? "dmarino@abim.org";
            }
        }
        
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toList">To list.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="ccList">The cc list.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="attachmentFilename">The attachment filename.</param>
        /// <returns></returns>
        public static void Send(List<string> toList, string from, string subject, string body, List<string> ccList = null, Stream attachment = null,
            string attachmentFilename = null)
        {
            if(toList == null)
                throw new ArgumentNullException("toList was passed in as null to SendEmail.Send()");
            if(from == null)
                throw new ArgumentNullException("from was passed in as null to SendEmail.Send()");
            if(body == null)
                throw new ArgumentNullException("body was passed in as null to SendEmail.Send()");
            
            if(ConfigurationManager.AppSettings["DisableEmail"] == "true") return;
            
            MailMessage message = new MailMessage();
            if(ccList == null) ccList = new List<string>();
            
            List<string> newTo = new List<string>();
            foreach(var val in toList)
            {
                if(val == null) continue;
                string[] split = val.Replace(",", ";").Split(';');
                foreach(var val2 in split)
                {
                    if(!string.IsNullOrEmpty(val2)) newTo.Add(val2);
                }
            }
            toList = newTo;
            
            List<string> newCc = new List<string>();
            foreach(var val in ccList)
            {
                if(val == null) continue;
                string[] split = val.Replace(",", ";").Split(';');
                foreach(var val2 in split)
                {
                    if(!string.IsNullOrEmpty(val2)) newCc.Add(val2);
                }
            }
            ccList = newCc;
            
            var invalidToEmails = toList.Where(e => !RegexValidation.IsEmail(e)).ToList();
            var invalidCcEmails = ccList.Where(e => !RegexValidation.IsEmail(e)).ToList();
            if(invalidToEmails.Any() || invalidCcEmails.Any())
                throw new ArgumentException("Invalid email address(es) were specified: " + StringExtensions.JoinWithAnd(invalidToEmails.Union(invalidCcEmails)));
            
            foreach(var to in toList.Where(val => !string.IsNullOrEmpty(val)))
                message.To.Add(to);
            foreach(var cc in ccList.Where(val => !string.IsNullOrEmpty(val)))
                message.CC.Add(cc);
            message.Subject = subject;
            message.From = new MailAddress(from);
            message.Body = body;
            if(attachment != null)
            {
                if(string.IsNullOrEmpty(attachmentFilename))
                    throw new ArgumentException("attachmentFilename must be specified when attachment is specified, in SendEmail.Send()");
                var attachmentObject = new Attachment(attachment, attachmentFilename);
                message.Attachments.Add(attachmentObject);
            }
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"] ?? "email.abim.org";
            SmtpClient smtp = new SmtpClient(smtpHost);
            
            if(ConfigurationManager.AppSettings["UsePollyForSMTP"] == "true")
                SendUsingPolly(() => { smtp.Send(message); });
            else smtp.Send(message);
            
            try
            {
                List<string> lines = new List<string>()
                {
                    "Sent email: {",
                    "\"To\": " + "\"" + string.Join(", ", toList.Where(t => t != null).Select(t => "\"" + t + "\"")) + "\"",
                    "\"From\": " + "\"" + from + "\"",
                    "\"Subject\": " + "\"" + subject + "\"",
                    "\"Body\": " +"\"" +  body + "\"",
                    "\"Cc\": " + "\"" + string.Join(", ", ccList.Where(t => t != null).Select(t => "\"" + t + "\"")) + "\"",
                    "\"Attachment\": " + (attachment != null),
                    "\"AttachmentFilename\": " + (attachmentFilename != null ? ("\"" + attachmentFilename + "\"") : "null"),
                    "}"
                };
                Log.Debug(string.Join("\r\n", lines));
            }
            catch(Exception ex)
            {
                Log.Error("Logging is failing in SendEmail, though emails are still being sent: " + ex.Stringify());
            }
        }

        /// <summary>
        /// Sends an email based on specified config value keys.
        /// </summary>
        /// <param name="toConfigKey">To configuration key.</param>
        /// <param name="fromConfigKey">From configuration key.</param>
        /// <param name="subjectConfigKey">The subject configuration key.</param>
        /// <param name="bodyConfigKey">The body configuration key.</param>
        /// <param name="ccConfigKey">The cc configuration key.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="attachmentFilename">The attachment filename.</param>
        /// <param name="bodyStringReplacements">The string replacements for the body (index 0 is for {0}, index 1 is for {1}, etc).</param>
        /// <param name="subjectStringReplacements">The string replacements for the subject (index 0 is for {0}, index 1 is for {1}, etc).</param>
        /// <param name="respectNewlineCodes">Whether to turn "\n" into a real newline.</param>
        /// <returns></returns>
        public static string SendFromConfigValues(string toConfigKey, string fromConfigKey, string subjectConfigKey, string bodyConfigKey,
            string ccConfigKey, Stream attachment = null, string attachmentFilename = null, string[] bodyStringReplacements = null,
            string[] subjectStringReplacements = null, bool respectNewlineCodes = false)
        {
            var to = ConfigurationManager.AppSettings[toConfigKey];
            if(to == null)
                return string.Format("Config value {0} not found", toConfigKey);
            var toList = to.Split(';').ToList();
            
            var from = ConfigurationManager.AppSettings[fromConfigKey];
            if(from == null)
                return string.Format("Config value {0} not found", fromConfigKey);
            
            var subject = ConfigurationManager.AppSettings[subjectConfigKey];
            if(subject == null)
                return string.Format("Config value {0} not found", subjectConfigKey);
            if(subjectStringReplacements != null)
                subject = string.Format(subject, subjectStringReplacements);
            
            var body = ConfigurationManager.AppSettings[bodyConfigKey];
            if(body == null)
                return string.Format("Config value {0} not found", bodyConfigKey);
            if(bodyStringReplacements != null)
                body = string.Format(body, bodyStringReplacements);
            if(respectNewlineCodes)
                body = body.Replace("\\r", "").Replace("\\n", "\r\n");
            
            var ccList = new List<string>();
            if(ccConfigKey != null)
            {
                var cc = ConfigurationManager.AppSettings[ccConfigKey];
                if(cc == null)
                    throw new Exception(string.Format("Config value {0} not found", ccConfigKey));
                ccList = cc.Split(';').ToList();
            }
            
            Send(toList, from, subject, body, ccList, attachment, attachmentFilename);
            return null;
        }
        
        /// <summary>
        /// Sends an error email.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void ErrorEmail(string message)
        {
            message = (message ?? "(No message)") + "\r\n\r\nPlease do not reply to this email.";
            Send(new List<string>(){ ErrorEmailAddress }, "Errors@abim.org", "Error", message);
        }

        /// <summary>
        /// Sends a fatal error email.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="environment">The environment, if known.</param>
        public static void FatalErrorEmail(string message, string environment = null)
        {
            message = (message ?? "(No message)") + "\r\n\r\nPlease do not reply to this email.";
            string subject = "Fatal Error";
            if(!string.IsNullOrEmpty(environment)) subject += $" ({environment})";
            Send(new List<string>(){ ErrorEmailAddress }, "FatalErrors@abim.org", subject, message);
        }

        /// <summary>
        /// Executes a Send using Polly.
        /// </summary>
        /// <param name="sendAction">The send action.</param>
        /// <returns></returns>
        private static void SendUsingPolly(Action sendAction)
        {
            var policy = Policy.Handle<SmtpException>(ex => { Log.Warn(ex); return true; })
                .WaitAndRetry(2, i => TimeSpan.FromSeconds(60));
            policy.Execute(() => { sendAction(); });
        }
    }
}
