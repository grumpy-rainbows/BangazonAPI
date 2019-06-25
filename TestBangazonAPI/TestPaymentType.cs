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
        public async Task Test_Get_NonExitant_PaymentType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymentType/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
