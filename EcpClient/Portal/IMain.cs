using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecp.Portal
{
    public interface IMain
    {
        Task<loginReply> Login(string login, string password);
    }
}
