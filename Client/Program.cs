using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{

    /// <summary>
    ///  IdentityModel  nuget IdentityModel 
    /// </summary>
    class Program
    { 
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            //var disco = await DiscoveryClient.GetAsync("http://localhost:1597");
            //identityServer 端口地址
            var discoveryClient = new DiscoveryClient("http://docelot.identityservice.com");
            discoveryClient.Policy.RequireHttps = false;
            var disco = await discoveryClient.GetAsync();

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            //帐号密码
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("mqkfc123@163.com", "123456").Result;
            //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            //api地址
            //var response = await client.GetAsync("http://localhost:1624/identity");
            var response = await client.GetAsync("http://DOcelot.APIGateway.com/api/productservice/values");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("\n\n");
            var response2 = await client.GetAsync("http://DOcelot.APIGateway.com/api/orderservice/values");
            if (!response2.IsSuccessStatusCode)
            {
                Console.WriteLine(response2.StatusCode);
            }
            else
            {
                var content2 = await response2.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content2));
            }

            Console.ReadLine();
        }
    }
}
