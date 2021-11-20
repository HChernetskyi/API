using AutomationFramework.Api;
using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Tests
{
    public class GetCarByIdTests
    {
        internal int recordId = 2;
        [Test, Order(1)]
        public void PositiveTestUseValidId()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars/";
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.Get(url + recordId);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode for GET request");
        }
        [Test, Order(2)]
        public void PositiveTestGetRightBody()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars/";
            string expectedResponseBody = @"
	{
		'id': 2,
		'manufacturer': 'Tesla',
		'model': 'Model 3',
		'build': 2017
	}
".Replace("'", "\"")
.Replace(" ", "")
.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.Get(url + recordId);
            string responseBody = response.GetStringResponse().Replace(" ", "");
            Assert.AreEqual(expectedResponseBody, responseBody, "Incorrect Response Body");
        }
        [Test, Order(3)]
        public void NegativeTestUseInvalidId()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars/";
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.Get(url + recordId * -1);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Incorrect StatusCode for GET request");
        }
    }
}
