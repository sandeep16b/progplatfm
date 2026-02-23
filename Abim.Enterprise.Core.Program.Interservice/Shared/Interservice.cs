using Abim.Platform.Program.Util;
using Abim.Platform.Program.Utils.Exceptions;
using Newtonsoft.Json;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Interservice.Shared
{
    /// <summary>
    /// Interservice main class
    /// </summary>
    //Updated from the original shared version
    public class Interservice : IInterservice
    {
        #region Fields

        #region Constants

        /// <summary>
        /// The default content type
        /// </summary>
        public const string DefaultContentType = "application/json";
        
        /// <summary>
        /// The Protocol Buffer content type
        /// </summary>
        public const string ProtobufContentType = "application/x-protobuf";

        /// <summary>
        /// The error prefix
        /// </summary>
        public const string ErrorPrefix = "Interservice Error: ";

        #endregion
        
        /// <summary>
        /// Whether Dispose() was already called.
        /// </summary>
        /// <value>
        /// true or false.
        /// </value>
        protected bool DisposeCalled = false;

        #endregion

        #region Properties

        /// <summary>
        /// The client, for reuse. This is according to Microsoft's recommendation to keep HttpClients static throughout the application lifecycle (this property isn't static, but the Interservice
        /// itself is a named instance in our dependency container) as seen at https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        /// 
        /// See also https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        /// </summary>
        /// <![CDATA[
        /// 
        /// From Microsoft:
        /// 
        ///     HttpClient is intended to be instantiated once and reused throughout the life of an application. The following conditions can result in SocketException errors:
        ///     
        ///     * Creating a new HttpClient instance per request.
        ///     * Server under heavy load.
        ///     
        ///     Creating a new HttpClient instance per request can exhaust the available sockets.
        /// 
        /// ]]>
        /// <![CDATA[
        /// 
        /// From the site cited above, on why disposing per-request HttpClients (whether manually or through a using statement) is not sufficient:
        /// 
        ///     Windows will hold a connection in this state for 240 seconds (It is set by [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\TcpTimedWaitDelay]). There is a
        ///     limit to how quickly Windows can open new sockets so if you exhaust the connection pool then you’re likely to see error like:
        ///     
        ///     Unable to connect to the remote server
        ///     System.Net.Sockets.SocketException: Only one usage of each socket address (protocol/network address/port) is normally permitted.
        /// 
        /// ABIM received the above error numerous times in August 2019 due to concurrent interservice calls from Registration to Program which were invoked by simultaneously-running
        /// CalculateBoardEligibilityJob child jobs
        /// 
        /// ]]>
        protected HttpClient HttpClient { get; set; }

        /// <summary>
        /// Gets or sets the request content type.
        /// </summary>
        /// <value>
        /// The type of the request content.
        /// </value>
        protected string RequestContentType { get; set; }

        /// <summary>
        /// Gets or sets the response data type.
        /// </summary>
        /// <value>
        /// The type of the response data.
        /// </value>
        protected string ResponseDataType { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Interservice"/> class.
        /// </summary>
        /// <param name="hostUrl">The host url.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        public Interservice(string hostUrl, string requestContentType, string responseDataType)
        {
            HttpClient = CreateHttpClient(hostUrl, responseDataType);
            if(string.IsNullOrEmpty(requestContentType)) requestContentType = DefaultContentType;
            if(string.IsNullOrEmpty(responseDataType)) responseDataType = DefaultContentType;
            RequestContentType = requestContentType;
            ResponseDataType = responseDataType;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Performs a GET.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        /// <param name="httpVersion">The http version.</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string url, string accessToken, RetryPolicy retryPolicy = null, Version httpVersion = null)
        {
            return await Send<T>(url, HttpVerbs.Get, accessToken, null, retryPolicy, httpVersion);
        }
        
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
        public async Task<T> Post<T>(string url, string accessToken, object body = null, RetryPolicy retryPolicy = null, Version httpVersion = null)
        {
            return await Send<T>(url, HttpVerbs.Post, accessToken, body, retryPolicy, httpVersion);
        }
        
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
        public async Task<T> Put<T>(string url, string accessToken, object body = null, RetryPolicy retryPolicy = null, Version httpVersion = null)
        {
            return await Send<T>(url, HttpVerbs.Put, accessToken, body, retryPolicy, httpVersion);
        }
       
        /// <summary>
        /// Send method to call an Http REST endpoint asynchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="verb"></param>
        /// <param name="accessToken"></param>
        /// <param name="body"></param>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        /// <param name="httpVersion">The http version.</param>
        /// <returns></returns>
        protected async Task<T> Send<T>(string url, HttpVerbs verb, string accessToken, object body, RetryPolicy retryPolicy, Version httpVersion)
        {
            try
            {
                //set the request message
                using(HttpRequestMessage requestMessage = CreateHttpRequestMessage(verb, url, body, RequestContentType, httpVersion, accessToken))
                {
                    //get and read the response. SendAsync is threadsafe (see https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?redirectedfrom=MSDN&view=netframework-4.8#Anchor_5)
                    using(HttpResponseMessage response = await ExecuteCall(() => HttpClient.SendAsync(requestMessage, default(CancellationToken)), retryPolicy)
                        .ConfigureAwait(continueOnCapturedContext: false))
                    {
                        if(!response.IsSuccessStatusCode)
                        {
                            var errorMessage = await ParseError(response, HttpClient, url, verb, accessToken, body);
                            if((int)(response.StatusCode) >= 500)
                                throw new Exception(errorMessage);
                            else
                            {
                                throw new UnsuccessfulStatusException(errorMessage)
                                {
                                    StatusCode = response.StatusCode,
                                    ResponseMessage = await TryReadResponse(response)
                                };
                            }
                        }
                        
                        //Read the response content
                        string content = null;
                        try
                        {
                            content = await response.Content.ReadAsStringAsync();
                        }
                        catch(JsonException ex)
                        {
                            throw new Exception($"{ErrorPrefix}Error reading response from {url}", ex);
                        }
                        
                        //Deserialize the response content (unless the type T requested is typeof(string))
                        T deserialized = default(T);
                        if(typeof(T) == typeof(string))
                        {
                            string cleanedContent = content;
                            if(content != null && content.Length > 2 && content.StartsWith("\"") && content.EndsWith("\""))
                            {
                                cleanedContent = content.Substring(1, content.Length - 2);
                            }
                            
                            deserialized = (T)((object)cleanedContent);
                        }
                        else
                        {
                            try
                            {
                                deserialized = JsonConvert.DeserializeObject<T>(content);
                            }
                            catch(JsonException ex)
                            {
                                throw new Exception($"{ErrorPrefix}Error deserializing response from {url}", ex);
                            }
                        }
                        return deserialized;
                    }
                }
            }
            catch(JsonException ex)
            {
                throw new Exception($"{ErrorPrefix}json exception from {url}", ex);
            }
            catch(UnsuccessfulStatusException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new Exception($"{ErrorPrefix}Error sending request to {url}", ex);
            }
        }
        
        /// <summary>
        /// Disposes all HttpClients.
        /// </summary>
        public void Dispose()
        {
            if(DisposeCalled) return;
            DisposeCalled = true;
            try
            {
                HttpClient.Dispose();
            }
            catch(Exception ex)
            {
            }
        }
        
        /// <summary>
        /// Executes an interservice call.
        /// </summary>
        /// <returns></returns>
        /// <param name="retryPolicy">The retry policy for Polly, if Polly should be used.</param>
        protected static TResponse ExecuteCall<TResponse>(Func<TResponse> interserviceCall, RetryPolicy retryPolicy)
        {
            if(retryPolicy != null)
            {
                //use Polly. The PollyCall class isn't available here
                TResponse response = default(TResponse);
                retryPolicy.Execute(() => { response = interserviceCall(); });
                return response;
            }
            else
            {
                //don't use Polly because no retryPolicy was supplied by the calling application
                return interserviceCall();
            }
        }
        
        /// <summary>
        /// Creates an HTTP request message.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <param name="url">The URL.</param>
        /// <param name="httpVersion">The HTTP version.</param>
        /// <param name="bodyContent">Content of the body.</param>
        /// <returns></returns>
        protected static HttpRequestMessage CreateHttpRequestMessage(HttpVerbs verb, string url, object body, string requestContentType, Version httpVersion, string accessToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(verb.ToString().ToUpper()), url);
            if((verb == HttpVerbs.Post || verb == HttpVerbs.Put || verb == HttpVerbs.Patch) && body != null)
            {
                ObjectContent bodyContent = new ObjectContent(body.GetType(), body, new JsonMediaTypeFormatter(), requestContentType);
                requestMessage.Content = bodyContent;
            }
            if(httpVersion != null) requestMessage.Version = httpVersion;        //otherwise the default used is version 1.1 (except in .Net Core 2.0 and higher, in which the default is 2.0)
            if(!string.IsNullOrEmpty(accessToken)) requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            return requestMessage;
        }
        
        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected static async Task<string> TryReadResponse(HttpResponseMessage response)
        {
            string responseContent = null;
            try
            {
                responseContent =  await response.Content.ReadAsStringAsync();
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
            }
            return responseContent;
        }
        
        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="client">The client.</param>
        /// <param name="url">The URL.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        protected static async Task<string> ParseError(HttpResponseMessage response, HttpClient client, string url, HttpVerbs verb, string accessToken,
            object body = null)
        {
            //PBI 164331: new string formatting
            var returnStringPart1 = $"{ErrorPrefix}Status code {{0}} was returned from a {{1}} call to {{2}}";
            string returnStringPart2 = "";
            
            //request body
            string bodyJson = null;
            if(body != null)
            {
                try
                {
                    bodyJson = JsonConvert.ToString(body);
                }
                #pragma warning disable 0168
                catch(Exception ex)
                {
                    returnStringPart2 += ". The body may have failed to serialize during the send";
                }
            }
            if(bodyJson != null) returnStringPart2 += " with body " + bodyJson;
            
            //response content
            string responseContent = null;
            try
            {
                responseContent = string.Format(": '{0}'", await response.Content.ReadAsStringAsync());
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                responseContent = " unable to be read";
            }
            returnStringPart2 += string.Format(". The response content was{0}", responseContent.Replace("\n", "").Replace("\r", "").Replace("\t", ""));
            
            //access token
            string tokenString = accessToken;
            if(tokenString == null) tokenString = "null";
            else if(tokenString == "") tokenString = "an empty string";
            returnStringPart2 += $". The access token used was {tokenString}";
            
            //other data
            try
            {
                var fullUri = new Uri(client.BaseAddress, url);
                return string.Format(returnStringPart1, response.StatusCode.ToString(), verb.ToString().ToUpper(), fullUri.AbsoluteUri) + returnStringPart2;
            }
            catch(Exception ex)
            {
                var fullMsg = $"{ErrorPrefix}call to {url ?? ""} did not return success. Additionally, an internal exception was thrown: {ex.Message}";
                return fullMsg;
            }
        }
        
        /// <summary>
        /// Creates a new HTTP client.
        /// </summary>
        /// <param name="hostUrl">The host URL.</param>
        /// <param name="responseDataType">Type of the response data.</param>
        /// <returns></returns>
        public static HttpClient CreateHttpClient(string hostUrl, string responseDataType)
        {
            HttpClient httpClient = null;
            
            if(string.IsNullOrEmpty(responseDataType) || responseDataType == DefaultContentType)
                httpClient = new HttpClient();
            else
                httpClient = new HttpClient(new SpecifiedMediaTypeDelegatingHandler(responseDataType));
            
            httpClient.BaseAddress = new Uri(hostUrl);
            
            return httpClient;
        }
        
        #endregion
    }
    
    /// <summary>
    /// A delegating handler to specify any media type
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class SpecifiedMediaTypeDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        /// Gets or sets the media type string.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        public string MediaType { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecifiedMediaTypeDelegatingHandler" /> class.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        public SpecifiedMediaTypeDelegatingHandler(string mediaType)
        {
            MediaType = mediaType;
            InnerHandler = new HttpClientHandler();
        }
        
        /// <summary>
        /// Sends a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var httpResponseMessage = task.Result;
                    httpResponseMessage.Content.Headers.ContentType.MediaType = MediaType;
                    return httpResponseMessage;
                });
        }
    }
}
