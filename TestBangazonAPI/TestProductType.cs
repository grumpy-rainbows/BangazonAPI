using BangazonAPI.Models;
using TestBangazonAPI;
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
    public class TestProductType
    {
        [Fact]
        public async Task Test_Get_All_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
            
                var response = await client.GetAsync("/api/productType");
                string responseBody = await response.Content.ReadAsStringAsync();

                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_ProductType_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/productType/1");

                string responseBody = await response.Content.ReadAsStringAsync();

                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(productType.Name);

            }
        }

        //Post a new Product Type
        [Fact]
        public async Task Test_Post_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
            ProductType productType = new ProductType
            {
                Name = "Phone"
            };
                var productTypeJson = JsonConvert.SerializeObject(productType);

                var response = await client.PostAsync(
                    "/api/productType",
                    new StringContent(productTypeJson, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);


                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(productType.Name, newProductType.Name);

            }
        }

        //Put Product Type
        [Fact]
        public async Task Test_Put_ProductType()
        {
            string Name = "Mac Book Bro";

            using (var client = new APIClientProvider().Client)
            {
                ProductType changeProductType = new ProductType
                {
                    Name = Name
                };
                var productTypeJson = JsonConvert.SerializeObject(changeProductType);

                var response = await client.PutAsync(
                    "/api/productType/7",
                    new StringContent(productTypeJson, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getProducType = await client.GetAsync("/api/productType/7");
                getProducType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProducType.Content.ReadAsStringAsync();
                var newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);


                Assert.Equal(HttpStatusCode.OK, getProducType.StatusCode);
                Assert.Equal(Name, newProductType.Name);

            }
        }

        //Detete Product Type
        [Fact]
        public async Task Test_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/productType/14");

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

    }
}
