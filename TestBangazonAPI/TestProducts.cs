/*
    Author: Jameka Echols 
    Purpose: 
    Methods: 
*/

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
    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/product");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Product()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/product/4");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(21, product.Quantity);
                Assert.Equal("Harry Potter and the Half-blood Prince", product.Title);
                Assert.NotNull(product);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Product_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/product/9999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product product1 = new Product
                {
                   ProductTypeId = 2,
                   CustomerId = 1,
                   Price = 9,
                   Title = "Apple iPhone X",
                   Description = "With the 10th Anniversary of the iPhone, we have created this exceptional phone.",
                   Quantity = 11
                };
                var product1AsJSON = JsonConvert.SerializeObject(product1);


                var response = await client.PostAsync(
                    "/api/product",
                    new StringContent(product1AsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(9, newProduct.Price);
                Assert.Equal("Apple iPhone X", newProduct.Title);
                Assert.Equal(11, newProduct.Quantity);
                

                var deleteResponse = await client.DeleteAsync($"/api/product/{newProduct.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

    }
}
