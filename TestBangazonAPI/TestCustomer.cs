using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestCustomer
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customer");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync("/api/customer/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Ali", customer.FirstName);
                Assert.Equal("Abdulle", customer.LastName);
                Assert.NotNull(customer);

            }
        }
        [Fact]
        public async Task Test_Create_And_Delete_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Customer john = new Customer
                {
                    FirstName = "John",
                    LastName = "Chalmers"
                };
                var johnAsJSON = JsonConvert.SerializeObject(john);


                var response = await client.PostAsync(
                    "/customer",
                    new StringContent(johnAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var newJohn = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("John", newJohn.FirstName);
                Assert.Equal("Chalmers", newJohn.LastName);

                var deleteResponse = await client.DeleteAsync($"/student/{newJohn.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newLastName = "Anderson";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Customer modifiedDave = new Customer
                {
                    FirstName = "Dave",
                    LastName = newLastName,

                };
                var modifiedDaveAsJSON = JsonConvert.SerializeObject(modifiedDave);

                var response = await client.PutAsync(
                    "/student/1",
                    new StringContent(modifiedDaveAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getDave = await client.GetAsync("/student/1");
                getDave.EnsureSuccessStatusCode();

                string getDaveBody = await getDave.Content.ReadAsStringAsync();
                Customer newDave = JsonConvert.DeserializeObject<Customer>(getDaveBody);

                Assert.Equal(HttpStatusCode.OK, getDave.StatusCode);
                Assert.Equal(newLastName, newDave.LastName);
            }
        }

    }
}
