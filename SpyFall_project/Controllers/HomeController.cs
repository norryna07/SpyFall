using Microsoft.AspNetCore.Mvc;
using SpyFall_project.Models;
using System.Diagnostics;
using SpyFall_project.Services;
using System.Net;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace SpyFall_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Scanning _scanning;
        private readonly SpyFallDBcontext _dbcontext;
        private readonly ExploitMethods _exploitMethods;

        public HomeController(ILogger<HomeController> logger, SpyFallDBcontext context)
        {
            _logger = logger;
            _scanning = new Scanning();
            _dbcontext = context;
            _exploitMethods = new ExploitMethods();

        }

        public IActionResult Index()
        {
            //Console.WriteLine(model.Count);
            return View(new List<PortScanResult>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DNS()
        {
            return View(new DNSPair());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Scan(string targetIp, int startPort, int endPort)
        {
            if (!IPAddress.TryParse(targetIp, out IPAddress ipadd))
            {
                return View(new ErrorViewModel { RequestId = "IP address Invalid" });
            }
            var results = await _scanning.ScanPortsAsync(targetIp, startPort, endPort);
            using (StreamWriter writer = new StreamWriter("./Data/ip.txt"))
            {
                writer.WriteLine(targetIp);
            }
            using (StreamWriter writer = new StreamWriter("./Data/Scan.txt"))
            {
                writer.WriteLine(targetIp);
                writer.WriteLine(startPort);
                writer.WriteLine(endPort);
            }
            return View("Index",results);
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            string host;
            int start, end;
            using (StreamReader reader = new StreamReader("./Data/Scan.txt"))
            {
                host = reader.ReadLine();
                start = int.Parse(reader.ReadLine());
                end = int.Parse(reader.ReadLine());
            }
            var results = await _scanning.ScanPortsAsync(host, start, end);
            return View("Index", results);
        }

        [HttpPost]
        public async Task<IActionResult> GetServices(string targetIp, int startPort, int endPort)
        {
            if (!IPAddress.TryParse(targetIp, out IPAddress ipadd))
            {
                return View(new ErrorViewModel { RequestId = "IP address Invalid" });
            }
            var results = await _scanning.ScanPortsAsync(targetIp, startPort, endPort);
            //we need to get the service name from the CommonService table from the database
            var services = _dbcontext.CommonServices
                            .Where(serv => serv.PortNumber >= startPort && serv.PortNumber <= endPort)
                            .ToList();
            foreach (var service in services)
            {
                if (results[(int)service.PortNumber - startPort].Status != "close")
                {
                    results[(int)service.PortNumber - startPort].Service = service.ServiceName;
                }
                else
                {
                    results[(int)service.PortNumber - startPort].Service = "-";
                }
            }
            using (StreamWriter writer = new StreamWriter("./Data/ip.txt"))
            {
                writer.WriteLine(targetIp);
            }
            using (StreamWriter writer = new StreamWriter("./Data/GetServices.txt"))
            {
                writer.WriteLine(targetIp);
                writer.WriteLine(startPort);
                writer.WriteLine(endPort);
            }    
            return View("Index", results);
        }

        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            string targetIp;
            int startPort, endPort;
            using (StreamReader reader = new StreamReader("./Data/Scan.txt"))
            {
                targetIp = reader.ReadLine();
                startPort = int.Parse(reader.ReadLine());
                endPort = int.Parse(reader.ReadLine());
            }
            var results = await _scanning.ScanPortsAsync(targetIp, startPort, endPort);
            //we need to get the service name from the CommonService table from the database
            var services = _dbcontext.CommonServices
                            .Where(serv => serv.PortNumber >= startPort && serv.PortNumber <= endPort)
                            .ToList();
            foreach (var service in services)
            {
                if (results[(int)service.PortNumber - startPort].Status != "close")
                {
                    results[(int)service.PortNumber - startPort].Service = service.ServiceName;
                }
                else
                {
                    results[(int)service.PortNumber - startPort].Service = "-";
                }
            }
            return View("Index", results);

        }

        [HttpPost]
        public async Task<IActionResult> DeepScan(int PortNumber)
        {
            string host;
            using (StreamReader reader = new StreamReader("./Data/ip.txt"))
            {
                host = reader.ReadLine();
            }
            using (StreamWriter writer = new StreamWriter("./Data/DeepScan.txt"))
            {
                writer.WriteLine(host);
                writer.WriteLine(PortNumber);
            }
            var results = await _scanning.DeepScanAsync(host, PortNumber, _dbcontext);
            return View("DeepScan", results);
        }

        [HttpGet]
        public async Task<IActionResult> DeepScan()
        {
            string host;
            int port;
            using (StreamReader reader = new StreamReader("./Data/DeepScan.txt"))
            {
                host = reader.ReadLine();
                port = int.Parse(reader.ReadLine());
            }
            var results = await _scanning.DeepScanAsync(host, port, _dbcontext);
            return View("DeepScan", results);
        }

        [HttpPost]
        public async Task<IActionResult> Exploit(int PortNumber, string Service)
        {
            string host;
            using (StreamReader reader = new StreamReader("./Data/ip.txt"))
            {
                host = reader.ReadLine();
            }
            if (Service.StartsWith("HTTP"))
            {
                return Redirect(await _exploitMethods.ExploitHTTP(host, PortNumber));
            }
            else if (Service.StartsWith("DNS"))
            {
                using (StreamWriter writer = new StreamWriter("./Data/ip.txt"))
                {
                    writer.WriteLine(host);
                }
                using (StreamWriter writer = new StreamWriter("./Data/port.txt"))
                {
                    writer.WriteLine(PortNumber);
                }
                return View("DNS");
            }
            return Error();
        }

        [HttpPost]
        public async Task<IActionResult> GetAddressByNameDNS(string inpName)
        {
            Console.WriteLine(inpName);
            var result = await _exploitMethods.ExploitDNS(inpName);
           // Console.WriteLine(result.IpAddress);
           // Console.WriteLine(result.Name);
            return View("DNS", result);
        }
        

    }
}
