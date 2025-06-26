using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ecp.Web
{
    public class Client : IClient
    {
        Uri uri;
        HttpClient client;
        CookieContainer cookieContainer;
        /// <summary>
        /// Простой веб-клиент для работы с ЕЦП.МИС
        /// </summary>

        private void AddDefaulRequestHeaders(HttpRequestHeaders headers, string userAgent)
        {
            headers.UserAgent.ParseAdd(userAgent);
            headers.Accept.ParseAdd("*/*");
            headers.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");
            headers.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            headers.Connection.ParseAdd("keep-alive");
            headers.Pragma.ParseAdd("no-cache");
        }
        public Client(string url, string userAgent)
        {
            cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { 
                CookieContainer = cookieContainer, 
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate 
            };
            client = new HttpClient(handler);
            AddDefaulRequestHeaders(client.DefaultRequestHeaders, userAgent);
            this.uri = new Uri(url);
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
        private void AddPostRequestHeaders(HttpRequestHeaders headers, Uri re)
        {
            headers.Referrer = re;
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Origin", re.ToString());
            headers.Add("DNT", "1");
            headers.Add("Priority", "u=0");
            headers.Add("Sec-Fetch-Dest", "empty");
            headers.Add("Sec-Fetch-Mode", "cors");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-GPC", "1");
            headers.Add("TE", "trailers");
            headers.Add("X-Requested-With", "XMLHttpRequest");
        }
        public async Task<string> Post(string query, Dictionary<string, string> parameters, string referer)
        {
            string responseString;
            try
            {
                Uri path = new Uri(this.uri, query);
                Uri re = new Uri(this.uri, referer);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                AddPostRequestHeaders(requestMessage.Headers, re);
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
        private void AddGetRequestHeaders(HttpRequestHeaders headers)
        {
            headers.Add("DNT", "1");
            headers.Add("Priority", "u=0, i");
            headers.Add("Sec-Fetch-Dest", "document");
            headers.Add("Sec-Fetch-Mode", "navigate");
            headers.Add("Sec-Fetch-Site", "none");
            headers.Add("Sec-Fetch-User", "?1");
            headers.Add("Upgrade-Insecure-Requests", "1");
        }
        public async Task<string> Get(string query)
        {
            string responseString;
            try
            {
                Uri path = new Uri(this.uri, query);
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
                AddGetRequestHeaders(requestMessage.Headers);
                var response = await client.SendAsync(requestMessage);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                string err = "Get: " + e.InnerException.Message ?? e.Message ?? "ошибка";
                throw new NetworkException(err);
            }
            catch (Exception e)
            {
                string err = "Get: " + e.Message ?? "ошибка";
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
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого объекта для десериализации</typeparam>
        /// <param name="query">query часть запроса</param>
        /// <param name="parameters">параметры</param>
        /// <param name="referer">referer запроса</param>
        /// <returns>Возвращает десериализованный объект. Либо возвращает NetworkException, DeserializeException</returns>
        public async Task<T> PostJson<T>(string query, Dictionary<string, string> parameters, string referer)
        {
            T res;
            string responseString = await Post(query, parameters, referer);
            res = JsonDeserialize<T>(responseString);
            return res;
        }
    }
    public class NetworkException : Exception
    {
        /// <summary>
        /// Любая ошибка при выполнении сетевого запроса
        /// </summary>
        /// <param name="message"></param>
        public NetworkException(string message) : base(message) { }
    }
    public class DeserializeException : Exception
    {
        /// <summary>
        /// Ошибка десериализации. Почти всегда означает, что вход не выполнен. 
        /// </summary>
        /// <param name="message"></param>
        public DeserializeException(string message) : base(message) { }
    }
}
