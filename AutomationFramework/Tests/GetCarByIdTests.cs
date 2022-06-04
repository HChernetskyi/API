using AutomationFramework.Api;
using AutomationFramework.Api.RequestObject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Tests
{
	public class GetCarByIdTests
	{
		internal int recordId = 2;
		internal const string _url = "https://car-fleet-management.herokuapp.com/cars/";

		[Test, Order(1)]
		public void PositiveTestUseValidId()
		{
			RequestHelper api = new RequestHelper();
			HttpResponseMessage response = api.GetAsync(_url + recordId).Result;
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode for GET request");
		}

		[Test, Order(2)]
		public void PositiveTestGetRightBody()
		{
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
			HttpResponseMessage response = api.GetAsync(_url + recordId).Result;
			string responseBody = response.GetStringResponse().Replace(" ", "");
			Assert.AreEqual(expectedResponseBody, responseBody, "Incorrect Response Body");
		}

		[Test, Order(3)]
		public void NegativeTestUseInvalidId()
		{
			RequestHelper api = new RequestHelper();
			HttpResponseMessage response = api.GetAsync(_url + (GetLastCarId() + 1)).Result;
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Incorrect StatusCode for GET request");
		}

		public static int GetLastCarId()
		{
			RequestHelper api = new RequestHelper();
			HttpResponseMessage response = api.GetAsync(_url).Result;
			var cars = response.GetDeserializedResponse<List<Car>>();
			return cars.Select(c => c.id).Max();
		}
	}
}
