
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestAPI
{
    public class APITestSuite
    {
        private HttpClient client;
        private string? createdProductId;
        private Product? product;
        private ProductData? proData;

        [SetUp]
        public void Setup()
        {
            client = new HttpClient
            {
                BaseAddress = new System.Uri("https://api.restful-api.dev")
            };
        }

        [TearDown]
        public void Teardown()
        {
            client?.Dispose();
        }

        [Test]
        public async Task VerifyUserCanGetList()
        {

            var response = await client.GetAsync("/objects");

            // Print the JSON response content
            var jsonString = await response.Content.ReadAsStringAsync();
            //TestContext.Out.WriteLine("Response JSON: " + jsonString);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 data retrieve status.");
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
                Assert.That(!string.IsNullOrEmpty(jsonString), "Expected a non-empty response body.");
            });
            var objects = await response.Content.ReadFromJsonAsync<List<Product>>();
            Assert.That(objects, Is.Not.Null, "Expected a JSON array but received null.");
            Assert.That(objects, Is.Not.Empty, "Expected a non-empty list of objects.");
            Assert.That(objects, Has.Count.GreaterThan(0), "Expected at least one object in the response.");
        }

        [Test]
        public async Task VerifyUserCanAddToList()
        {
            // Initialize the Product object using the POST constructor without Id
            proData = new ProductData(2019,1849.99M,"Intel Core i9","1 TB");
            product = new Product("Apple MacBook Pro 16") {Data = proData};

            // Create custom serialization options to ignore null values
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camel case for property names
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // Serialize product to JSON content
            var jsonContent = JsonContent.Create(product, options: jsonOptions);
            var jsonString = await jsonContent.ReadAsStringAsync();
            //TestContext.Out.WriteLine("Post Json Request Body " + jsonString);

            // Send the POST request
            var response = await client.PostAsync("/objects", jsonContent);

            // Optional: Add assertions to verify response
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 Created status.");
            var responseData = await response.Content.ReadAsStringAsync();
           // TestContext.Out.WriteLine("Response JSON: " + responseData);

            // Deserialize the response to check specific properties
            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();

            // Assert the properties of the created product
            Assert.Multiple(() =>
            {
                // Check if Data is not null before accessing its properties
                if (createdProduct?.Data != null)
                {
                    Assert.That(createdProduct, Is.Not.Null, "Expected a non-null product in response.");
                    Assert.That(createdProduct.Id, Is.Not.Null.Or.Empty, "Expected a non-empty Id.");
                    Assert.That(createdProduct.Name, Is.EqualTo(product.Name), "Expected name to match.");
                    Assert.That(createdProduct.Data, Is.Not.Null, "Expected data to be present.");
                    Assert.That(createdProduct.Data.Year, Is.EqualTo(proData.Year), "Expected the year to match.");
                    Assert.That(createdProduct.Data.Price, Is.EqualTo(proData.Price), "Expected the price to match.");
                    Assert.That(createdProduct.Data.CPUModel, Is.EqualTo(proData.CPUModel), "Expected CPU model to match.");
                    Assert.That(createdProduct.Data.HardDiskSize, Is.EqualTo(proData.HardDiskSize), "Expected hard disk size to match.");
                    // Check that CreatedAt has a valid DateTime value
                    Assert.That(createdProduct.CreatedAt, Is.GreaterThan(DateTime.MinValue), "Expected createdAt timestamp to have a valid value.");


                    // Store the created product's ID
                    createdProductId = createdProduct.Id;

                }
                else
                {
                    Assert.Fail("Post Responce Data Not shown");
                }
            });

           
        }

        [Test]
        public async Task VerifyUserCanGetOneProduct()
        {
            // Call VerifyUserCanAddToList to add a product
            await VerifyUserCanAddToList();

            // Now use the created product ID to get the product
            var response = await client.GetAsync($"/objects/{createdProductId}");

            // Assert that the response is successful
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 OK status.");
            var responseData = await response.Content.ReadAsStringAsync();
            // TestContext.Out.WriteLine("Response JSON: " + responseData);

            // Optional: Validate the returned product's details
            var retrievedProduct = await response.Content.ReadFromJsonAsync<Product>();

            Assert.Multiple(() =>
            {
                // Check if Data is not null before accessing its properties
                if (retrievedProduct?.Data != null)
                {
                    Assert.That(retrievedProduct, Is.Not.Null, "Expected a non-null product.");
                    Assert.That(retrievedProduct.Id, Is.EqualTo(createdProductId), "Expected the retrieved product ID to match the created product ID.");
                    Assert.That(retrievedProduct.Name, Is.EqualTo(product?.Name), "Expected name to match.");
                    Assert.That(retrievedProduct.Data, Is.Not.Null, "Expected data to be present.");
                    Assert.That(retrievedProduct.Data.Year, Is.EqualTo(proData?.Year), "Expected the year to match.");
                    Assert.That(retrievedProduct.Data.Price, Is.EqualTo(proData?.Price), "Expected the price to match.");
                    Assert.That(retrievedProduct.Data.CPUModel, Is.EqualTo(proData?.CPUModel), "Expected CPU model to match.");
                    Assert.That(retrievedProduct.Data.HardDiskSize, Is.EqualTo(proData?.HardDiskSize), "Expected hard disk size to match.");
                }
                else
                {
                    Assert.Fail("Get One Product Responce Data Not shown");
                }
            });
        }


        [Test]
        public async Task VerifyUserCanUpdateOneProduct()
        {
            // Call VerifyUserCanAddToList to add a product
            await VerifyUserCanAddToList();

            // Initialize an empty ProductData object with all properties as null
            proData = new ProductData(2049.99M, "silver");
            product = new Product("Apple MacBook Pro 16") { Data = proData };

            // Create custom serialization options to ignore null values
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camel case for property names
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // Serialize product to JSON content
            var jsonContent = JsonContent.Create(product, options: jsonOptions);
            var jsonString = await jsonContent.ReadAsStringAsync();
            //TestContext.Out.WriteLine("Put Json Request Body " + jsonString);


            // Send the POST request
            var response = await client.PutAsync($"/objects/{createdProductId}", jsonContent);

            // Optional: Add assertions to verify response
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 Updated status.");
            var responseData = await response.Content.ReadAsStringAsync();
            //TestContext.Out.WriteLine("Response JSON: " + responseData);

            // Deserialize the response to check specific properties
            var updatedProduct = await response.Content.ReadFromJsonAsync<Product>();

            // Assert the properties of the created product
            Assert.Multiple(() =>
            {
                // Check if Data is not null before accessing its properties
                if (updatedProduct?.Data != null)
                {
                    Assert.That(updatedProduct, Is.Not.Null, "Expected a non-null product in response.");
                    Assert.That(updatedProduct.Id, Is.Not.Null.Or.Empty, "Expected a non-empty Id.");
                    Assert.That(updatedProduct.Name, Is.EqualTo(product?.Name), "Expected name to match.");
                    Assert.That(updatedProduct.Data, Is.Not.Null, "Expected data to be present.");
                    Assert.That(updatedProduct.Data.Year, Is.EqualTo(proData?.Year), "Expected the year to match.");
                    Assert.That(updatedProduct.Data.Price, Is.EqualTo(proData?.Price), "Expected the price to match.");
                    Assert.That(updatedProduct.Data.CPUModel, Is.EqualTo(proData?.CPUModel), "Expected CPU model to match.");
                    Assert.That(updatedProduct.Data.HardDiskSize, Is.EqualTo(proData?.HardDiskSize), "Expected hard disk size to match.");
                    Assert.That(updatedProduct.Data.Color, Is.EqualTo(proData?.Color), "Expected CPU model to match.");
                    // Check that UpdatedAt has a valid DateTime value
                    Assert.That(updatedProduct.UpdatedAt, Is.GreaterThan(DateTime.MinValue), "Expected updatedAt timestamp to have a valid value.");

                }
                else
                {
                    Assert.Fail("Update Responce Data Not shown " + updatedProduct);
                }
            });
        

        }



        [Test]
        public async Task VerifyUserCanDeleteOneProduct()
        {
            // Call VerifyUserCanUpdateOneProduct to add and update a product
            await VerifyUserCanUpdateOneProduct();


            // Send the POST request
            var response = await client.DeleteAsync($"/objects/{createdProductId}");

            // Optional: Add assertions to verify response
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 Delete status.");
            var responseData = await response.Content.ReadAsStringAsync();
            //TestContext.Out.WriteLine("Delete Response JSON: " + responseData);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 OK status for successful deletion.");
                Assert.That(responseData, Is.EqualTo("{\"message\":\"Object with id = " + createdProductId + " has been deleted.\"}"), "Expected the deletion confirmation message.");
            });

        }

    }
}