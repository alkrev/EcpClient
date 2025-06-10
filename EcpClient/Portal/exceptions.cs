using System;

namespace Ecp.Portal
{
    public class NotLoggedInException : Exception
    {
        public NotLoggedInException(string message) : base(message) { }
    }
}
