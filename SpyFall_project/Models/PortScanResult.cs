namespace SpyFall_project.Models
{
    /// <summary>
    /// A class that contains the results of the ports scan, the port number, the status of the port,
    /// and the default service if is needed.
    /// </summary>
    public class PortScanResult
    {
        public int PortNumber { get; set; }
        public string Status { get; set; }

        public string Service { get; set; } = string.Empty;
    }
}
