using AutomationFramework.Api;
using AutomationFramework.Api.RequestObject;
using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Tests
{
    public class SimpleApiTests
    {
        string url = "https://car-fleet-management.herokuapp.com/cars";
        [Test, Order(1)]
        public void GetCars()
        {
            string expectedResponseBody = @"
[
	{
		'id': 1,
		'manufacturer': 'Ford',
		'model': 'Model T',
		'build': 1927
	},
	{
		'id': 2,
		'manufacturer': 'Tesla',
		'model': 'Model 3',
		'build': 2017
	},
	{
		'id': 3,
		'manufacturer': 'Tesla',
		'model': 'Cybertruck',
		'build': 2019
	}
]
".Replace("'", "\"")
.Replace(" ", "")
.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.GetAsync(url).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode");
            string responseBody = response.GetStringResponse().Replace(" ", "");
            Assert.AreEqual(expectedResponseBody, responseBody, "Incorrect ResponseBody");
        }
        [Test, Order(2)]
        public void AddCar()
        {
            Car newCar = new Car() { id = 4, build = 2020, manufacturer = "Tesla", model = "-1" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.PostAsync(url, newCar).Result;
            var x = response.GetStringResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode");
            Car responseBody = response.GetDeserializedResponse<Car>();
            Assert.Multiple(() =>
                {
                    //Assert.AreEqual(newCar.id, responseBody.id, "Incorrect ResponseBody id");
                    Assert.AreEqual(newCar.build, responseBody.build, "Incorrect ResponseBody build");
                    Assert.AreEqual(newCar.manufacturer, responseBody.manufacturer, "Incorrect ResponseBody manufacturer");
                    Assert.AreEqual(newCar.model, responseBody.model, "Incorrect ResponseBody model");
                });
        }
    }
}