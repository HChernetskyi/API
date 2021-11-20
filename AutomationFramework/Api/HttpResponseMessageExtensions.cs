using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Api
{
    // <summary>
    /// Contains helper methods for working with HttpResponseMessages.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Returns string representation of content from httpResponseMessage.
        /// </summary>
        /// <param name="httpResponseMessage">The response of the http request.</param>
        /// <returns></returns>
        public static string GetStringResponse(this HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Gets the response content from HttpResponseMessage and deserializes it to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="httpResponseMessage">The response of the http request.</param>
        /// <returns>The deserialized object from the response JSON.</returns>
        public static T GetDeserializedResponse<T>(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                throw new Exception("HttpStatusCode.NoContent");
            }

            var stringResponse = GetStringResponse(httpResponseMessage);
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
    }
}
