using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.Api
{
    public class RequestHelper
    {
        public enum ContentType
        {
            Xml,
            Json,
            XwwwFormUrlencoded
        }

        #region Private Properties

        private readonly Dictionary<ContentType, string> _contentTypes = new Dictionary<ContentType, string>
        {
            { ContentType.Xml, "application/xml" },
            { ContentType.Json, "application/json" },
            { ContentType.XwwwFormUrlencoded, "application/x-www-form-urlencoded" }
        };

        private readonly HttpClientHandler _httpClientHandler;

        #endregion

        #region Ctor

        public RequestHelper()
        {
            _httpClientHandler = new HttpClientHandler { UseDefaultCredentials = true };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends a Get http request.
        /// </summary>
        /// <param name="url">A string that represents the request  <see cref="T:System.Uri" />.</param>
        /// <param name="timeOut">Sets the time (in seconds) to wait before the request times out.</param>
        /// <param name="retryCount">Sets the number of attempts that will be made in case of unsuccessful http request.</param>
        /// <param name="retryTimeout">The duration (in seconds) to wait for for a particular retry attempt.</param>
        /// <returns>Object that represents http response.</returns>
        public HttpResponseMessage Get(string url, int timeOut = 60, int retryCount = 0, int retryTimeout = 5)
        {
            var httpClient = new HttpClient(_httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(timeOut)
            };

            HttpRequestMessage CreateHttpRequestMessage()
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                return requestMessage;
            }

            var response = Policy
                .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .WaitAndRetryAsync(retryCount, i => TimeSpan.FromSeconds(retryTimeout), (result, timeSpan, rCount, context) =>{})
                .ExecuteAsync(() => httpClient.SendAsync(CreateHttpRequestMessage()));
            return response.Result;
        }

        /// <summary>
        /// Sends a Post http request. Takes request body as an object and serializes it to json before sending request.
        /// </summary>
        /// <param name="url">A string that represents the request <see cref="T:System.Uri" />.</param>
        /// <param name="data">An object that represents http request body.</param>
        /// <param name="timeOut">Sets the time (in seconds) to wait before the request times out.</param>
        /// <param name="retryCount">Sets the number of attempts that will be made in case of unsuccessful http request.</param>
        /// <param name="retryTimeout">The duration (in seconds) to wait for for a particular retry attempt.</param>
        /// <param name="ignoreSslValidations">Indicates whether the ssl certificate validation will be ignored.</param>
        /// <returns>Object that represents http response.</returns>
        public HttpResponseMessage Post(string url, object data, int timeOut = 60, int retryCount = 0, int retryTimeout = 5, bool ignoreSslValidations = false)
        {
            var content = JsonConvert.SerializeObject(data);

            return SendRequestAsync(HttpMethod.Post, url, content, ContentType.Json, timeOut, retryCount,
                retryTimeout, ignoreSslValidations).Result;
        }

        /// <summary>
        /// Sends a Put http request. Takes request body as an object and serializes it to json before sending request.
        /// </summary>
        /// <param name="url">A string that represents the request  <see cref="T:System.Uri" />.</param>
        /// <param name="data">An object that represents http request body.</param>
        /// <param name="timeOut">Sets the time (in seconds) to wait before the request times out.</param>
        /// <param name="retryCount">Sets the number of attempts that will be made in case of unsuccessful http request.</param>
        /// <param name="retryTimeout">The duration (in seconds) to wait for for a particular retry attempt.</param>
        /// <param name="ignoreSslValidations">Indicates whether the ssl certificate validation will be ignored.</param>
        /// <returns>Object that represents http response.</returns>
        public HttpResponseMessage Put(string url, object data, int timeOut = 60, int retryCount = 0, int retryTimeout = 5, bool ignoreSslValidations = false)
        {
            var content = JsonConvert.SerializeObject(data);

            return SendRequestAsync(HttpMethod.Put, url, content, ContentType.Json, timeOut, retryCount,
                retryTimeout, ignoreSslValidations).Result;
        }

        /// <summary>
        /// Sends a Delete http request.
        /// </summary>
        /// <param name="url">A string that represents the request <see cref="T:System.Uri" />.</param>
        /// <param name="data">A string that represents http request body.</param>
        /// <param name="timeOut">Sets the time (in seconds) to wait before the request times out.</param>
        /// <param name="retryCount">Sets the number of attempts that will be made in case of unsuccessful http request.</param>
        /// <param name="retryTimeout">The duration (in seconds) to wait for for a particular retry attempt.</param>
        /// <param name="ignoreSslValidations">Indicates whether the ssl certificate validation will be ignored.</param>
        /// <returns>Object that represents http response.</returns>
        public HttpResponseMessage Delete(string url, string data, int timeOut = 60, int retryCount = 0, int retryTimeout = 5, bool ignoreSslValidations = false)
        {
            return SendRequestAsync(HttpMethod.Delete, url, data, ContentType.Json, timeOut, retryCount,
                retryTimeout, ignoreSslValidations).Result;
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Sends a specific http request.
        /// </summary>
        /// <param name="httpMethod">Indicates a http method type.</param>
        /// <param name="url">A string that represents the request <see cref="T:System.Uri" />.</param>
        /// <param name="data">A string that represents http request body.</param>
        /// <param name="contentType">Indicates the media type of the resource.</param>
        /// <param name="timeOut">Sets the time (in seconds) to wait before the request times out.</param>
        /// <param name="retryCount">Sets the number of attempts that will be made in case of unsuccessful http request.</param>
        /// <param name="retryTimeout">The duration (in seconds) to wait for for a particular retry attempt.</param>
        /// <param name="ignoreSslValidations">Indicates whether the ssl certificate validation will be ignored.</param>
        /// <returns>Object that represents http response.</returns>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string url, string data, ContentType contentType, int timeOut = 60, int retryCount = 0, int retryTimeout = 5, bool ignoreSslValidations = false)
        {
            if (ignoreSslValidations)
            {
                _httpClientHandler.ServerCertificateCustomValidationCallback += (s, certificate, chain, sslPolicyErrors) => true;
            }

            var httpClient = new HttpClient(_httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(timeOut)
            };

            var response = await Policy
                .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .WaitAndRetryAsync(retryCount, i => TimeSpan.FromSeconds(retryTimeout), (result, timeSpan, rCount, context) =>
                {})
                .ExecuteAsync(() => httpClient.SendAsync(CreateHttpRequestMessage(httpMethod, url, data, contentType)));
            return response;
        }

        /// <summary>
        /// Returns new instance of HttpRequestMessage.
        /// </summary>
        /// <param name="httpMethod">Indicates a http method type.</param>
        /// <param name="url">A string that represents the request <see cref="T:System.Uri" />.</param>
        /// <param name="data">A string that represents http request body.</param>
        /// <param name="contentType">Indicates the media type of the resource.</param>
        /// <returns>Object that represents http response.</returns>
        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string url, string data,
            ContentType contentType)
        {
            var httpRequestMessage = new HttpRequestMessage(httpMethod, url)
            {
                Content = CreateRequestContent(data, contentType)
            };

            return httpRequestMessage;
        }

        /// <summary>
        /// Returns content type header value.
        /// </summary>
        /// <param name="contentType">Indicates the media type of the resource.</param>
        /// <returns>The media type to use for the content.</returns>
        private string GetContentTypeValue(ContentType contentType)
        {
            return _contentTypes[contentType];
        }

        /// <summary>
        /// Creates new instance of StringContent based on a specified content type.
        /// </summary>
        /// <param name="stringContent">The content used to initialize the <see cref="T:System.Net.Http.StringContent" />.</param>
        /// <param name="contentType">Indicates the media type of the resource.</param>
        /// <returns>Returns HTTP content based on a string.</returns>
        private StringContent CreateRequestContent(string stringContent, ContentType contentType)
        {
            return string.IsNullOrEmpty(stringContent)
                ? null
                : new StringContent(stringContent, Encoding.UTF8, GetContentTypeValue(contentType));
        }

        #endregion
    }
}
