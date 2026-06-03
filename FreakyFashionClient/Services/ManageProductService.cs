using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;
using Newtonsoft.Json;
using System.Text;

namespace FreakyFashionClient.Services
{
    public class ManageProductService : IManageEntityService<ProductModel>
    {
        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();
        private readonly ILogger<ManageProductService> logger;
        private readonly IConfiguration configuration;

        public ManageProductService(IConfiguration configuration, ILogger<ManageProductService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task<IEnumerable<ProductModel>> ListEnitity()
        {
            //Get uri safely as a string
            string baseUri = configuration["FreakyFashion:URL"];

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUri}api/Products/GetAllProducts");

            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent content = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<IEnumerable<ProductModel>>();
                    logger.LogInformation($"The products list is loaded.");
                    return responeData;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                return new List<ProductModel>();
            }
        }

        public async Task<ProductModel> AddEntity(string token, string name, string description, int price, string image, string urlSlug, string category)
        {
            string baseUri = configuration["FreakyFashion:URL"];
            Header.Add("Authorization", $"Bearer {token}");
            Header.Add("name", name);
            Header.Add("description", description);
            Header.Add("image", image);
            Header.Add("urlslug", urlSlug);
            Header.Add("category", name);
            Header.Add("price", price.ToString());

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}api/Products/AddProduct");

            foreach (var head in Header)
            {
                httpRequest.Headers.Add(head.Key, head.Value);
                httpRequest.Headers.Add("Accept", "application/json");
            }

            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent content = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<ProductModel>();
                    logger.LogInformation($"The product {responeData.Name} with {responeData.Id} is added.");
                    return responeData;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                logger.LogInformation($"Add product with {ex.Message} failed.");
                return new ProductModel();
            }
        }

        public async Task<ProductModel?> UpdateEntity(int id, string token, ProductModel productModel)
        {
            //Get uri safely as a string
            string baseUri = configuration["FreakyFashion:URL"];
            Header.Add("Authorization", $"Bearer {token}");
            Header.Add("Accept", "application/json");
            Header.Add("id", id.ToString());

            // 1. Create the patch Array
            var patchPayload = new[]
            {
                new { op = "replace", path = "/price", value = (object)productModel.Price },
                new { op = "replace", path = "/name", value = (object)productModel.Name },
                new { op = "replace", path = "/description", value = (object)productModel.Description },
                new { op = "replace", path = "/urlSlug", value = (object)productModel.UrlSlug },
            };

            // 3. Serialize to JSON
            var json = JsonConvert.SerializeObject(patchPayload);
            var patchContent = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"{baseUri}api/Products/UpdateProduct");

            httpRequest.Content = patchContent;

            foreach (var head in Header)
            {
                httpRequest.Headers.Add(head.Key, head.Value);
            }
            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent responseContent = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<ProductModel>();
                    logger.LogInformation($"The product {responeData.Name} with {responeData.Id} is updated.");
                    return responeData;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                logger.LogInformation($"Update the product {productModel.Name} with {id} failed.");
                return new ProductModel();
            }
        }

        public async Task<bool> DeleteEntity(int id, string token)
        {
            //Get uri safely as a string
            string baseUri = configuration["FreakyFashion:URL"];
            Header.Add("Authorization", $"Bearer {token}");
            Header.Add("id", id.ToString());

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}api/Products/DeleteProduct");

            foreach (var head in Header)
            {
                httpRequest.Headers.Add(head.Key, head.Value);
                httpRequest.Headers.Add("Accept", "application/json");
            }

            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent content = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<string>();

                    if(responeData == "200")
                    return true;

                    return false;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                logger.LogInformation($"Delete product with {id} failed.");
                return false;
            }
        }

        public async Task<ProductModel> GetProductById(int id) 
        {
            string baseUri = configuration["FreakyFashion:URL"];
            Header.Add("id", id.ToString());

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}api/Products/GetProductById");

            foreach (var head in Header)
            {
                httpRequest.Headers.Add(head.Key, head.Value);
                httpRequest.Headers.Add("Accept", "application/json");
            }

            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent content = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<ProductModel>();
                    if (responeData.Id > 0)
                        logger.LogInformation($"The product {responeData.Name} with {responeData.Id} is loaded.");

                        return responeData;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                logger.LogInformation($"Get product with {ex.Message} failed.");
                return new ProductModel();
            }
        }
        public async Task<IEnumerable<ProductModel>> GetProductByUrlSlug(string urlSlug)
        {
            string baseUri = configuration["FreakyFashion:URL"];
            Header.Add("slug", urlSlug);

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}api/Products/GetAllProductBySlug");

            foreach (var head in Header)
            {
                httpRequest.Headers.Add(head.Key, head.Value);
                httpRequest.Headers.Add("Accept", "application/json");
            }

            List<ProductModel> list = new List<ProductModel>();
            try
            {
                HttpResponseMessage response = await client.SendAsync(httpRequest);
                using (HttpContent content = response.Content)
                {
                    var responeData = await response.Content.ReadFromJsonAsync<IEnumerable<ProductModel>>();

                    if (responeData.Count() == 1 && responeData.FirstOrDefault().Id == 0)
                    {
                        var empty = new ProductModel { StatusMessage = responeData.FirstOrDefault().StatusMessage };
                        list.Add(empty);
                        return list;
                    }
                    logger.LogInformation($"The products {responeData.FirstOrDefault().Name} with {responeData.FirstOrDefault().Id} are loaded.");

                        return responeData;
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                logger.LogInformation($"Get product with {ex.Message} failed.");
                return list;
            }
        }
    }
}
