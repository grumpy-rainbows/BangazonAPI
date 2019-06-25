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
    }
}
