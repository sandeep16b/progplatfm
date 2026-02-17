using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Testing
{
    /// <summary>
    /// Responses class
    /// </summary>
    public static class Responses
    {
        /// <summary>
        /// Reads the response text from an HttpResponseMessage object
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="allowNullResponse">if set to <c>true</c> [allow null response].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Response Content was null; status code was " + response.StatusCode</exception>
        public static string ResponseText(HttpResponseMessage response, bool allowNullResponse = false)
        {
            if(response.Content == null)
            {
                if(allowNullResponse) return string.Empty;
                throw new Exception("Response Content was null; status code was " + response.StatusCode);
            }
            Task<string> task = response.Content.ReadAsStringAsync();
            task.Wait();
            return task.Result;
        }
    }
}
