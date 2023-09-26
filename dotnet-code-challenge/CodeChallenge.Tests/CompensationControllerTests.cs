
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();

            // Seed some compensation data to test with
            var comps = new List<Compensation>
            {
                new Compensation()
                {
                    EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                    Salary = 150000,
                    EffectiveDate = DateTime.Today.Subtract(TimeSpan.FromDays(90)),
                },
                new Compensation()
                {
                    EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3",
                    Salary = 50000,
                    EffectiveDate = DateTime.Today.Subtract(TimeSpan.FromDays(90)),

                },
                new Compensation() //Future date
                {
                    EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3",
                    Salary = 999999,
                    EffectiveDate = DateTime.Today.AddDays(90),

                }
            };
            var postRequestTasks = new List<Task>();
            foreach (var comp in comps)
            {
                // Post each request
                postRequestTasks.Add(_httpClient.PostAsync("api/compensation",
                   new StringContent(new JsonSerialization().ToJson(comp), 
                   Encoding.UTF8, "application/json")));
            }

            //Wait for all requests
            Task t = Task.WhenAll(postRequestTasks);
            t.Wait();
            if (t.Status != TaskStatus.RanToCompletion)
            {
                throw new Exception($"Test data failed to seed: {t.Status.ToString()}");
            }
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var comp = new Compensation()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Salary = 150000,
                EffectiveDate = DateTime.Today.Subtract(TimeSpan.FromDays(30)),
            };

            var requestContent = new JsonSerialization().ToJson(comp);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newComp = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newComp.CompensationId);
            Assert.AreEqual(comp.Salary, newComp.Salary);
            Assert.AreEqual(comp.EffectiveDate, newComp.EffectiveDate);
            Assert.AreEqual(comp.EmployeeId, newComp.EmployeeId);
        }

        [TestMethod]
        public void GetByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedSalary = 150000;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var comp = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, comp.Salary);
        }

        [TestMethod]
        public void GetByEmployeeId_Returns_Active_Salary()
        {
            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var expectedSalary = 140000;

            //   Add newer active compensation
            var comp = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = expectedSalary,
                EffectiveDate = DateTime.Today.Subtract(TimeSpan.FromDays(30)),
            };
            var requestContent = new JsonSerialization().ToJson(comp);
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;


            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseComp = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, responseComp.Salary);
        }
    }
}
