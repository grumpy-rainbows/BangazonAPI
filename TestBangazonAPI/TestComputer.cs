/* Author: Brian Jobe
 * Purpose: Creating tests for the Computer controller methods of GET, POST, PUT, and DELETE. 
 * Methods: Test_Get_All_Computers, Test_Single_Computer, Test_Get_NonExistant_Computer_Fails, Test_Create_and_Delete_Computer, Test_Delete_NonExistent_Computer_Fails, Test_Modify_Computer.
 */

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
    public class TestComputer
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Computer");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Single_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Computer/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Inspiron 1500", computer.Make);
                Assert.Equal("Dell", computer.Manufacturer);
                Assert.NotNull(computer);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Computer/99999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_and_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Computer computer = new Computer
                {
                    Make = "MBP",
                    Manufacturer = "Apple",
                    PurchaseDate = new DateTime(2009, 2, 15),
                    DecomissionDate = new DateTime(2014, 6, 14)
                };

                var computerAsJSON = JsonConvert.SerializeObject(computer);
                 
                var response = await client.PostAsync(
                    "/api/computer",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json")
                );



                string responseBody = await response.Content.ReadAsStringAsync();
                var newComputer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("MBP", newComputer.Make);
                Assert.Equal("Apple", newComputer.Manufacturer);

                var deleteResponse = await client.DeleteAsync($"/api/computer/{newComputer.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/computer/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Computer()
        {
            // New last name to change to and test
            string newManufacturer = "Apple";
            string oldManufacturer = "Dell";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Computer modifiedComputer = new Computer
                {
                    Make = "Inspiron 1500",
                    Manufacturer = newManufacturer,
                    PurchaseDate = new DateTime(2000, 12, 12),
                    DecomissionDate = new DateTime(2005, 12, 12)
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    "/api/computer/1",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getComputer = await client.GetAsync("/api/computer/1");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newManufacturer, newComputer.Manufacturer);

                Computer originalComputer = new Computer
                {
                    Make = "Inspiron 1500",
                    Manufacturer = oldManufacturer,
                    PurchaseDate = new DateTime(2000, 12, 12),
                    DecomissionDate = new DateTime(2005, 12, 12)
                };
                var originalComputerAsJSON = JsonConvert.SerializeObject(originalComputer);

                var originalResponse = await client.PutAsync(
                                "/api/computer/1",
                                new StringContent(originalComputerAsJSON, Encoding.UTF8, "application/json")
                            );
                string responseBodyOriginal = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, originalResponse.StatusCode);


            }
        }
    }
}
