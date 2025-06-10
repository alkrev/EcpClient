using System.Threading.Tasks;

namespace Ecp.Portal
{
    public interface IMain
    {
        Task<loginReply> Login(string login, string password);
    }
}
