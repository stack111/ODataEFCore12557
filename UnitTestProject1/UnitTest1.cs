using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApplication1;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly JsonSerializerOptions serializationOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private static TestServer server;
        private static HttpClient Client;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            serializationOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            
            server = new TestServer(new WebHostBuilder()
                 .UseStartup<Startup>()
                 .ConfigureTestServices(services =>
                 {
                 }));
            Client = server.CreateClient();
        }


        [AssemblyCleanup]
        public static void AssemblyTeardown()
        {
            server.Dispose();
        }


        [DataTestMethod]
        [DataRow("odata/poco?$filter=partitionKey eq 'e58ec759-cdd4-42d9-af7b-807214f4a456' and state eq 'Activated'")]
        [DataRow("odata/poco/e58ec759-cdd4-42d9-af7b-807214f4a456?$filter=state eq 'Activated'")]
        public async Task FilterByEnumAsync(string path)
        {
            var response = await Client.GetAsync(new Uri(path, UriKind.Relative));
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, responseContent);
            var accounts = Deserialize<ODataResponse<PocoSample>>(responseContent);
            Assert.AreEqual(1, accounts.Value.Count);
            PocoSample actualAccount = accounts.Value[0];

        }

        protected static string SerializeToJson<T>(T payload)
        {
            return JsonSerializer.Serialize(payload, serializationOptions);
        }

        protected static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, serializationOptions);
        }

    }
}
