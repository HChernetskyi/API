using AutomationFramework.Api;
using AutomationFramework.Api.RequestObject;
using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace AutomationFramework.Tests
{
    public class OtherTests
    {
        public int carId = 1;
        bool findId = false;
        [Test, Order(1)]
        public void FindFreeId()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars/";
            RequestHelper api = new RequestHelper();
            while (findId == false)
            {
                HttpResponseMessage responseMessage = api.Get(url + carId);
                Car responseBody = responseMessage.GetDeserializedResponse<Car>();
                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    findId = true;
                    break;
                }
                carId = responseBody.id + 1;    //next available id
                Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode, "Incorrect StatusCode for GET request");
            }
        }
        [Test, Order(2)]
        public void AddCarByPost()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars";
            Car newCar = new Car() { id = carId, build = 2020, manufacturer = "Jac", model = "Model CW6x" };
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.Post(url, newCar);
            Car responseBody = response.GetDeserializedResponse<Car>();
            carId = responseBody.id;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode");
        }
        [Test, Order(3)]
        public void RemoveCar()
        {
            string url = "https://car-fleet-management.herokuapp.com/cars/";
            RequestHelper api = new RequestHelper();
            HttpResponseMessage response = api.Delete(url + carId, "accept: */*");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Incorrect StatusCode");
        }
    }
}
