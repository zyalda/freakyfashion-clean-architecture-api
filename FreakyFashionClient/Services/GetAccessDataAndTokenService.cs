using FreakyFashionClient.Enums;
using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;

namespace FreakyFashionClient.Services
{
    public class GetAccessDataAndTokenService : IGetAccessDataAndTokenService
    {
        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();
        private readonly IConfiguration configuration;
        private readonly ILoginResult loginResult;

        public GetAccessDataAndTokenService(ILoginResult loginResult, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.loginResult = loginResult;
        }
        public async Task<LoginResult> GetTokenAccesstAsync(string userName, string passWord)
        {
            //Get uri safely as a string
            string baseUri  = configuration["FreakyFashion:URL"];
            Header.Add("userName", userName);
            Header.Add("passWord", passWord);

            //Skapa upp en instans av httpclient
            HttpClient client = new HttpClient();
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}api/Login");

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
                    var responeData = await response.Content.ReadFromJsonAsync<UserManagerResponseModel>();

                    return new LoginResult { Status = ResponseStatus.Succeed, UserManagerResponseModelData = responeData };
                }
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException socketEx)
            {
                // Specifically handle the network/socket error
                var result = new UserManagerResponseModel{AccessToken = $"Socket Error: {socketEx.Message} (Code: {socketEx.ErrorCode})"};
                // Specifically handle the network/socket error
                return new LoginResult {Status = ResponseStatus.BadRequest, UserManagerResponseModelData = result};
            }
            catch (HttpRequestException ex)
            {
                var result = new UserManagerResponseModel {AccessToken = $"HTTP Error: {ex.Message}"};
                // Handle other HTTP errors (e.g., 404, 500)
                return new LoginResult {Status = ResponseStatus.BadRequest, UserManagerResponseModelData = result };
            }
            catch (Exception ex)
            {
                var result = new UserManagerResponseModel {AccessToken = $"General Error: {ex.Message}"};
                // General fallback
                return new LoginResult {Status = ResponseStatus.Declined, UserManagerResponseModelData = result };
            }
        }
    }
}
