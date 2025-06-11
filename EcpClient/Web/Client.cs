using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ecp.Web
{
    public class Client : IClient
    {
        string url;
        HttpClient client;
        CookieContainer cookieContainer;
        public Client(string url)
        {
            cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.url = url;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
        public async Task<string> Post(string url, Dictionary<string, string> parameters, string referer)
        {
            string responseString;
            try
            {
                string path = Path.Combine(this.url, url);
                string re = Path.Combine(this.url, referer);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                requestMessage.Headers.Referrer = new Uri(re);
                requestMessage.Content = new FormUrlEncodedContent(parameters);
                var response = await client.SendAsync(requestMessage);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                string err = "Post: " + e.InnerException.Message ?? e.Message ?? "ошибка";
                throw new NetworkException(err);
            }
            catch (Exception e)
            {
                string err = "Post: " + e.Message ?? "ошибка";
                throw new NetworkException(err);
            }
            return responseString;
        }
        public T JsonDeserialize<T>(string responseString)
        {
            T res;
            try
            {
                res = JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception e)
            {
                string err = "JsonDeserialize: " + e.Message ?? "ошибка";
                throw new DeserializeException(err);
            }
            return res;
        }
        public async Task<T> PostJson<T>(string url, Dictionary<string, string> parameters, string referer)
        {
            T res;
            string responseString = await Post(url, parameters, referer);
            res = JsonDeserialize<T>(responseString);
            return res;
        }
    }
    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message)
        {

        }
    }
    public class DeserializeException : Exception
    {
        public DeserializeException(string message) : base(message)
        {

        }
    }
}
