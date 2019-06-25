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
    public class TestPaymentType
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/PaymentType");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Single_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/PaymentType/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(12345, paymentType.AcctNumber);
                Assert.Equal("Visa", paymentType.Name);
                Assert.NotNull(paymentType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_PaymentType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/PaymentType/99999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_and_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType payment = new PaymentType
                {
                    AcctNumber = 11111,
                    Name = "Diner's Club",
                    CustomerId = 1
                };
                var paymentAsJSON = JsonConvert.SerializeObject(payment);


                var response = await client.PostAsync(
                    "/api/paymentType",
                    new StringContent(paymentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newPayment = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(11111, newPayment.AcctNumber);
                Assert.Equal("Diner's Club", newPayment.Name);
                Assert.Equal(1, newPayment.CustomerId);

                var deleteResponse = await client.DeleteAsync($"/api/paymentType/{newPayment.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_PaymentType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/paymentType/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
    }
}