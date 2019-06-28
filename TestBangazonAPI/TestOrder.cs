/* Author: Billy Mathison
 * Purpose: Creating tests for the Order controller methods of GET, POST, PUT, and DELETE. 
 * Methods: Test_Get_All_orders, Test_Get_All_Completed_Orders, Test_Get_All_Incomplete_Orders, Test_Get_All_Orders_With_Customers, Test_Get_All_Orders_With_Products, Test_Get_Single_Order, Test_Get_Single_Completed_Order, Test_Get_Single_Incomplete_Order, Test_Get_Single_Order_With_Customers, Test_Get_Single_Order_With_Products, Test_Get_NonExistant_Order_Fails, Test_Create_And_Delete_Order, Test_Modify_Order, Test_Modify_Order_Again
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

        [Fact]
        public async Task Test_Get_NonExistant_Order_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/order/99999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                Order order = new Order
                {
                    CustomerId = 4,
                    PaymentTypeId = 1
                };

                var orderAsJSON = JsonConvert.SerializeObject(order);

                var response = await client.PostAsync(
                    "/api/Order",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(4, newOrder.CustomerId);
                Assert.Equal(1, newOrder.PaymentTypeId);

                var deleteResponse = await client.DeleteAsync($"/api/Order/{newOrder.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Order_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/order/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*Initial GET
                 */
                var order = await client.GetAsync("/api/Order/1");
                Assert.Equal(HttpStatusCode.OK, order.StatusCode);

                /*
                    PUT section
                 */
                Order modifiedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 3
                };
                var modifiedOrderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    "/api/Order/1",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getOrder = await client.GetAsync("/api/Order/1");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(3, newOrder.PaymentTypeId);
            }
        }

        [Fact]
        public async Task Test_Modify_Order_Again()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*Initial GET
                 */
                var order = await client.GetAsync("/api/Order/1");
                Assert.Equal(HttpStatusCode.OK, order.StatusCode);

                /*
                    PUT section
                 */
                Order modifiedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 2
                };
                var modifiedOrderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    "/api/Order/1",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getOrder = await client.GetAsync("/api/Order/1");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(2, newOrder.PaymentTypeId);
            }
        }
    }
}
