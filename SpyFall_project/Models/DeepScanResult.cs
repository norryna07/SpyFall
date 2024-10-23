namespace SpyFall_project.Models
{
    /// <summary>
    /// A class that contain results of deep scanning.
    /// Contain the port number, the service tested and the status of the service ("open" or "close").
    /// </summary>
    public class DeepScanResult
    {
        public int PortNumber { get; set; }
        public string ServiceTest {  get; set; }

        public string ServiceStatus { get; set; } = string.Empty;

    }
}
