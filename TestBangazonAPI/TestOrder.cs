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
                Assert.NotNull(orders[0].PaymentType);
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
                Assert.NotNull(orders[0].Customer.FirstName);
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
                Assert.NotNull(orders[0].Products[0].Title);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);
                 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, order.CustomerId);
                Assert.Equal(2, order.PaymentTypeId);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Completed_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order/1/?_completed=true");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(12345, order.PaymentType.AcctNumber);
                Assert.Equal("Visa", order.PaymentType.Name);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Incomplete_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order/5/?_completed=false");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Null(order.PaymentType);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Order_With_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order/1/?_include=customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Jameka", order.Customer.FirstName);
                Assert.Equal("Echols", order.Customer.LastName);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Order_With_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order/2/?_include=products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(order.Products.Count > 0);
                Assert.Equal(21, order.Products[0].Quantity);
                Assert.Equal("Harry Potter and the Half-blood Prince", order.Products[0].Title);
            }
        }
    }
}
