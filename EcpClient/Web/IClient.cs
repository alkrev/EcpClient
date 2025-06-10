using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecp.Web
{
    public interface IClient
    {
        Task<T> PostJson<T>(string url, Dictionary<string, string> parameters, string referer);
    }
}
