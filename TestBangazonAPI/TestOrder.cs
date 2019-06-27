/* Author: Billy Mathison
 * Purpose: Creating tests for the Order controller methods of GET, POST, PUT, and DELETE. 
 * Methods: Test_Get_All_orders, 
 */

using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestOrder
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Completed_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_completed=true");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
                Assert.True(orders[0].PaymentType != null);
            }
        }

        [Fact]
        public async Task Test_Get_All_Incomplete_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_completed=false");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
                Assert.True(orders[0].PaymentType == null);
            }
        }

        [Fact]
        public async Task Test_Get_All_Orders_With_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_include=customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
                Assert.True(orders[0].Customer.FirstName != null);
            }
        }

        [Fact]
        public async Task Test_Get_All_Orders_With_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_include=products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
                Assert.True(orders[0].Products[0].Title != null);
            }
        }
    }
}
