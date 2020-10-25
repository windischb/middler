using System.Net;

namespace middler.Common.SharedModels.Models
{
    public class SimpleCredentials
    {
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }

        public static implicit operator NetworkCredential(SimpleCredentials simpleCredentials)
        {
            return new NetworkCredential(simpleCredentials.UserName, simpleCredentials.Password, simpleCredentials.Domain);
        }

    }
}
