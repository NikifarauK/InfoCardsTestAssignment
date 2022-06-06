using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    internal class RestConnectionServise
    {
        private HttpClient HttpClient { get; set; }

        private string ServerAdress { get; set; }

        public RestConnectionServise(string serverUrl)
        {
            ServerAdress = serverUrl;
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders
                .Add(HttpRequestHeader.ContentType.ToString(), "application/json");
        }

        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                return await HttpClient.GetAsync(ServerAdress);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Server is not avaible", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                return await HttpClient.GetAsync(ServerAdress + $"/{id}");
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Server is not avaible", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        public async Task<HttpResponseMessage> Insert(string body)
        {
            try
            {
                return await HttpClient.PostAsync(ServerAdress, new StringContent(body, Encoding.UTF8, "application/json"));
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Server is not avaible", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        public async Task<HttpResponseMessage> Update(int id, string body)
        {
            try
            {
                return await HttpClient.PutAsync(ServerAdress + $"/{id}", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Server is not avaible", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                return await HttpClient.DeleteAsync(ServerAdress + $"/{id}");
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Server is not avaible", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

    }
}
