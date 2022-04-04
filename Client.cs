using System.Net;
using System.Net.Http;
using System.Text;
using Serializer;

namespace HttpClient
{
    internal class Client : IDisposable
    {
        private readonly string _url;
        private readonly int _port;
        private readonly System.Net.Http.HttpClient _httpClient;

        public Client(string url, int port)
        {
            _url = url;
            _port = port;
            _httpClient = new();
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("HttpClient/1.0");
        }

        public async Task<bool> WriteAnswer(Output oupt)
        {
            if (oupt is null)
            {
                throw new ArgumentNullException(nameof(oupt));
            }
            string outputData = Serializer.Serializer.SerializeOutput(oupt, "json");
            try
            {
                await Request("WriteAnswer", outputData);
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Connection error: {ex.StatusCode}");
            }
            return false;
        }

        public async Task<Input?> GetInputData()
        {
            string data = await Request("GetInputData");
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return Serializer.Serializer.DeserializeInputObject("json", data);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<bool> Ping()
        {
            try
            {
                await Request("Ping");
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Connection error: {ex.StatusCode}");
            }
            return false;
        }

        private async Task<string> Request(string requestType, string requsetBody = "")
        {
            string uri = $"{_url}:{_port}/{requestType}";

            var response = requsetBody switch
            {
                "" => await _httpClient.GetAsync(uri),
                _ => await _httpClient.PostAsync(uri, new StringContent(requsetBody, Encoding.UTF8, "application/json"))
            };

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
