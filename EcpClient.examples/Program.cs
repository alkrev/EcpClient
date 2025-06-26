using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecp.Web;
using Ecp.Portal;

namespace EcpClient.examples
{
    internal class Program
    {
        /// <summary>
        /// Программа демонстрирует подключение к порталу ЕЦП
        /// </summary>
        static async Task Main(string[] args)
        {
            try
            {
                var url = "https://ecp.medkirov.ru";
                var login = "user";
                var password = "password";
                var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:140.0) Gecko/20100101 Firefox/140.0";

                var wc = new Client(url, userAgent);

                var str = await wc.Get(url);
                var main = new Main(wc);

                var reply = await main.Login(login, password);
                if (reply.success == true)
                {
                    Console.WriteLine("Вход выполнен");
                }
                else
                {
                    Console.WriteLine("Вход не выполнен");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Вход не выполнен: {ex.Message}");
            }
        }
    }
}
