using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        /// <param name="url"></param>
        public Client(string url)
        {
            cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.uri = new Uri(url);
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
        public async Task<string> Post(string query, Dictionary<string, string> parameters, string referer)
        {
            string responseString;
            try
            {
                Uri path = new Uri(this.uri, query);
                Uri re = new Uri(this.uri, referer);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                requestMessage.Headers.Referrer = re;
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
