using AutomationFramework.Api;
using AutomationFramework.Api.RequestObject;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Tests
{
    public class UpdateCarTests
    {
        internal int carId = 2;
        readonly int dateYear = DateTime.Now.Year + 1;
        readonly string url = "https://car-fleet-management.herokuapp.com/cars/";
        [Test, Order(1)]
        public void PositiveTestWithValidBody()
        {
            Car updateCar = new Car() { id = carId, build = 2020, manufacturer = "ZAZ", model = "SENS" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage oldRecord = api.GetAsync(url + carId).Result;
            Car oldBody = oldRecord.GetDeserializedResponse<Car>();
            Assert.AreEqual(HttpStatusCode.OK, oldRecord.StatusCode, "Incorrect StatusCode for GET request");
            HttpResponseMessage response = api.PutAsync(url, updateCar).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode for PUT request");
            HttpResponseMessage responseReturn = api.PutAsync(url, oldBody).Result;
            Assert.AreEqual(HttpStatusCode.OK, responseReturn.StatusCode, "Incorrect StatusCode for PUT request");
        }
        [Test, Order(2)]
        public void NegativeTestWithValidId()
        {
            Car updateCar = new Car() { id = carId * -1, build = 2008, manufacturer = "Mitsubishi", model = "Lancer 9" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.PutAsync(url, updateCar).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Incorrect StatusCode for PUT request");
        }
        [Test, Order(3)]
        public void NegativeTestWithInvalidPath()
        {
            Car updateCar = new Car() { id = carId, build = 1980, manufacturer = "Vaz", model = "2101" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.PutAsync(url + carId, updateCar).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Incorrect StatusCode for PUT request");
        }
        [Test, Order(4)]
        public void NegativeTestWithInvalidDate()
        {
            Car updateCar = new Car() { id = carId, build = dateYear, manufacturer = "Mazda", model = "3" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.PutAsync(url, updateCar).Result;
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Incorrect StatusCode for PUT request");
        }
    }
}
