using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Interservice.Shared
{
    /// <summary>
    /// Interservice interface
    /// </summary>
    public interface IInterservice : IDisposable
    {
        /// <summary>
        /// Performs a GET.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        /// <param name="httpVersion">The http version.</param>
        /// <returns></returns>
        Task<T> Get<T>(string url, string accessToken, RetryPolicy retryPolicy = null, Version httpVersion = null);
        
        /// <summary>
        /// Performs a POST.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="body">The body.</param>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        /// <param name="httpVersion">The http version.</param>
        /// <returns></returns>
        Task<T> Post<T>(string url, string accessToken, object body = null, RetryPolicy retryPolicy = null, Version httpVersion = null);
        
        /// <summary>
        /// Performs a PUT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="body">The body.</param>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        /// <param name="httpVersion">The http version.</param>
        /// <returns></returns>
        Task<T> Put<T>(string url, string accessToken, object body = null, RetryPolicy retryPolicy = null, Version httpVersion = null);
    }
}
