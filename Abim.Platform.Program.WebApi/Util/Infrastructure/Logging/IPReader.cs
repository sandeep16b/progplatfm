using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// IPReader
    /// </summary>
    public static class IPReader
    {
        /// <summary>
        /// Gets the server IP.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetServerIP(HttpRequestMessage request)
        {
            const string httpContext = "MS_HttpContext";
            const string remoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
            const string msOwinContext = "MS_OwinContext";
            
            string ip = null;
            if(request.Properties.ContainsKey(httpContext))
            {
                HttpContextWrapper contextWrapper = (HttpContextWrapper)request.Properties[httpContext];
                if(contextWrapper != null) ip = contextWrapper.Request.UserHostAddress;
            }
            if(request.Properties.ContainsKey(remoteEndpointMessage))
            {
                RemoteEndpointMessageProperty remoteEndpoint =
                    (RemoteEndpointMessageProperty)request.Properties[remoteEndpointMessage];
                if(remoteEndpoint != null) ip = remoteEndpoint.Address;
            }
            if(request.Properties.ContainsKey(msOwinContext))
            {
                OwinContext owinContext = (OwinContext)request.Properties[msOwinContext];
                if(owinContext != null) ip = owinContext.Request.RemoteIpAddress;
            }
            if(ip == null || ip.Length < "0.0.0.0".Length)
            {
                //if all else failed for some reason, run ipconfig
                var outputText = "";
                try
                {
                    ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c ipconfig.exe");
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    Process process = new Process();
                    process.StartInfo = procStartInfo;
                    process.Start();
                    outputText = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    ip = ParseIPFromOutput(outputText);
                }
                catch(Exception ex)
                {
                    ip = null;
                }
            }
            return ip;
        }

        /// <summary>
        /// Gets the client IP.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetClientIP(HttpRequestMessage request)
        {
            if(request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper) request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if(request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            return null;
        }

        /// <summary>
        /// Parses the IP from output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        private static string ParseIPFromOutput(string output)
        {
            if(output == null) return null;
            string[] lines = output.Replace("\r", "").Split('\n');
            string connection = "";
            List<string> resultIPs = new List<string>();
            List<string> resultConnections = new List<string>();
            for(int l = 0; l < lines.Length; l++)
            {
                if(!lines[l].StartsWith(" ") && lines[l].Trim().Length > 0)
                    connection = lines[l].Trim().TrimEnd(':');
                else if(lines[l].Contains("IPv4 Address"))
                {
                    resultIPs.Add(lines[l].Substring(lines[l].IndexOf(':') + 1).Trim());
                    resultConnections.Add(connection);
                }
            }
            if(resultIPs.Count == 0) return null;
            if(resultIPs.Count == 1) return resultIPs.First();
            List<string> combinedResults = new List<string>();
            for(int i = 0; i < resultIPs.Count; i++)
                combinedResults.Add(string.Format("{0} ({1})", resultIPs[i], resultConnections[i]));
            return string.Join(", ", combinedResults.ToArray());
        }
    }
}
