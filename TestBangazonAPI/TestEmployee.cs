using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestEmployee
    {
        [Fact]
        public async Task Test_Get_All_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync("/api/Employee");
                string responseBody = await response.Content.ReadAsStringAsync();

                var employeeList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employeeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Employee_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Employee/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var employeeList = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Andy", employeeList.FirstName);
                Assert.Equal("Collins", employeeList.LastName);

                Assert.NotNull(employeeList);

            }
        }

        //Post and Delete Employee
        [Fact]
        public async Task Test_Create_And_Delete_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                Employee local = new Employee
                {
                    FirstName = "David",
                    LastName = "Thomes"
                //    IsSuperVisor = true,
                //    DepartmentId = 1,
                //    Department = new Department
                //    {
                //        Id = 4,
                //        Name = "Logistics",
                //    }
                };
                var localAsJson = JsonConvert.SerializeObject(local);

                var response = await client.PostAsync(
                   "/api/Employee/",
                new StringContent(localAsJson, Encoding.UTF8, "application/json")

                   );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newLocal = JsonConvert.DeserializeObject<Employee>(responseBody);



                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                Assert.Equal("David", newLocal.FirstName);

                Assert.Equal("Thomes", newLocal.LastName);

               // Assert.Equal(1, local.DepartmentId);





                var deleteResponse = await client.DeleteAsync($"api/Employee/{newLocal.Id}");

                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }

        }
        // Test if the employee is not existence 
        [Fact]
        public async Task Test__Nonexistant_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync("api/Employee/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }
    

        //Pt and Delete Employee
        [Fact]
        public async Task Test_Put_Employee()
        {
            string newChangeLocal = "Coco";

            using (var client = new APIClientProvider().Client)
            {
                Employee ChangeName = new Employee
                {
                    FirstName = newChangeLocal,
                    LastName = "Thomes",
                    IsSuperVisor = true,
                    DepartmentId = 1,
                    Department = new Department
                    {
                        Id = 4,
                        Name = "Logistics"
                    }
                };
                var ChangeNameAsJson = JsonConvert.SerializeObject(ChangeName);

                var response = await client.PostAsync(
                   "/api/Employee/1",
                new StringContent(ChangeNameAsJson, Encoding.UTF8, "application/json")

                   );

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);


                var getEmployee = await client.GetAsync("/api/Employee/1");
                getEmployee.EnsureSuccessStatusCode();

                var getEmployeeBody = await getEmployee.Content.ReadAsStringAsync();
                Employee newEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);
                

                Assert.Equal(HttpStatusCode.OK, getEmployee.StatusCode);
                Assert.Equal(newChangeLocal, newEmployee.FirstName);
            }

        }
    }
}
