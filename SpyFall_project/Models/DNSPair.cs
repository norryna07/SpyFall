using System.Net;

namespace SpyFall_project.Models
{
    /// <summary>
    /// A class that contains a DNS pair with the name of the domain and the IP address.
    /// </summary>
    public class DNSPair
    {
        public string Name {  get; set; }
        public IPAddress IpAddress { get; set; }
    }
}
