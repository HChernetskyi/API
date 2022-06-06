using AutomationFramework.Api;
using AutomationFramework.Api.RequestObject;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
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
			string responseBody = response.GetStringResponse().Replace(" ", "");
			var existsResult = JsonConvert.DeserializeObject<List<Car>>(expectedResponseBody);
			var responseResult = JsonConvert.DeserializeObject<List<Car>>(responseBody);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseResult.Count.Should().BeGreaterOrEqualTo(existsResult.Count);
        }

		[Test, Order(2)]
		public void AddCar()
		{
			Car newCar = new Car() { id = 4, build = 2020, manufacturer = "Tesla", model = "-1" };
			RequestHelper api = new RequestHelper();
			HttpResponseMessage response = api.PostAsync(url, newCar).Result;
			var x = response.GetStringResponse();
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode wasn't 200 OK!");
			Car responseBody = response.GetDeserializedResponse<Car>();
			Assert.Multiple(() =>
				{
					//Assert.AreEqual(newCar.id, responseBody.id, $"Incorrect ResponseBody id: expected {newCar.id}, but actual is: {responseBody.id}");
					Assert.AreEqual(newCar.build, responseBody.build, $"Incorrect ResponseBody.build: expected {newCar.build}, but actual is: {responseBody.build}");
					Assert.AreEqual(newCar.manufacturer, responseBody.manufacturer, $"Incorrect ResponseBody manufacturer: expected {newCar.manufacturer}, but actual is: {responseBody.manufacturer}");
					Assert.AreEqual(newCar.model, responseBody.model, $"Incorrect ResponseBody model: expected {newCar.model}, but actual result is: {responseBody.model}");
				});
		}
	}
}