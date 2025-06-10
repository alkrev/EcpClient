using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecp.Web
{
    public interface IClient
    {
        Task<T> PostJson<T>(string url, Dictionary<string, string> parameters, string referer);
    }
}
