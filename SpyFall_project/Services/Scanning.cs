using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NuGet.DependencyResolver;
using SpyFall_project.Models;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SpyFall_project.Services
{
    internal class Scanning
    {
        /// <summary>
        /// A function that checks if a port is open.
        /// </summary>
        /// <param name="host">The IP address of the host.</param>
        /// <param name="port">The port to test.</param>
        /// <returns>Information about the port status</returns>
        private async Task<PortScanResult> ScanPortAsync(string host, int port)
        {
            var result = new PortScanResult { PortNumber = port };
            try
            {
                using (TcpClient client = new TcpClient())
                {

                    var connectTask = client.ConnectAsync(host, port);
                    // add a timeout of 5 seconds in order to not block the site much time
                    var timeoutTask = Task.Delay(2000);

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (connectTask == completedTask) {
                        if (client.Connected) {
                            result.Status = "TCP open";
                            return result;
                        }
                    }

                    throw new SocketException();
                }
            }
            catch
            {
                //test the UDP is the TCP is not open
                try
                {
                    using (UdpClient client = new UdpClient())
                    {
                        client.Connect(host, port);

                        byte[] testData = new byte[1];
                        client.Send(testData, testData.Length);

                        client.Client.ReceiveTimeout = 2000;
                        var receiveTask = client.ReceiveAsync();

                        if (await Task.WhenAny(receiveTask, Task.Delay(2000)) == receiveTask) {
                            result.Status = "UDP open";
                            return result;
                        }

                        result.Status = "close";
                        return result;

                    }
                } catch
                {
                    result.Status = "close";
                    return result;
                }
            }

        }

        /// <summary>
        /// Find out what ports are open.
        /// </summary>
        /// <param name="host">The IP address of the host.</param>
        /// <param name="startPort">The left edge of the interval of ports to check.</param>
        /// <param name="endPort">The right edge of the interval of ports to check.</param>
        /// <returns>A list with information about the status of every port.</returns>
        public async Task<List<PortScanResult>> ScanPortsAsync(string host, int startPort, int endPort)
        {
            List<Task<PortScanResult>> tasks = new List<Task<PortScanResult>>();

            for (int i = startPort; i <= endPort; i++)
            {
                tasks.Add(ScanPortAsync(host, i));
            }

            return (await Task.WhenAll(tasks)).ToList();
        }

        /// <summary>
        /// Verify is one service is open on a port sending a request and analyse the response.
        /// </summary>
        /// <param name="host">The IP address of the host.</param>
        /// <param name="port">The port to test.</param>
        /// <param name="verif">The service to verify.</param>
        /// <returns>Information about the status of service.</returns>
        private async Task<DeepScanResult> TestServiceAsync(string host, int port, ServiceVerif verif)
        {
            var result = new DeepScanResult 
            {
                PortNumber = port,
                ServiceTest = verif.Name
            };

            //connect to port, sent the message, analize the response
            try
            {
                using (TcpClient client  = new TcpClient())
                {
                    await client.ConnectAsync(host, port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        client.ReceiveTimeout = 5000;
                        if (verif.SendMessage != null && verif.SendSize != null)
                        {
                            byte[] sendPacket;
                            int dim = verif.SendSize.Value;
                            if (verif.Name.StartsWith("HTTP"))
                            {
                                var adresa = Encoding.ASCII.GetBytes("Host: " + host + "\r\n\r\n");
                                sendPacket = new byte[verif.SendSize.Value + adresa.Length];
                                Array.Copy(verif.SendMessage, 0, sendPacket, 0, verif.SendSize.Value);
                                Array.Copy(adresa, 0, sendPacket, verif.SendSize.Value, adresa.Length);
                                dim = sendPacket.Length;
                            }
                            else
                            {
                                sendPacket = verif.SendMessage;
                            }
                            
                            stream.Write(sendPacket, 0, dim);
                        }
                        

                        //reading the response
                        byte[] buffer = new byte[4096];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        //Console.WriteLine(verif.Name);
                        Console.WriteLine(response);
                        //Console.WriteLine();
                        if (bytesRead >= verif.MinLengthResponse)
                        {
                            //test is the response contain the string from the database
                            bool open = false;
                            if (verif.ContainResponse != null)
                            {
                                open = open | ContainsBytes(buffer, bytesRead, verif.ContainResponse);
                            }
                            if (verif.StartResponse != null)
                            {
                                open = open | StartsBytes(buffer, verif.StartResponse);
                            }
                            if (open)
                            {
                                result.ServiceStatus = "open";
                            }
                            else
                            {
                                result.ServiceStatus = "close";
                            }
                        }
                        else
                        {
                            result.ServiceStatus = "close";
                        }
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result.ServiceStatus = "close";
            }
            return result;
        }

        /// <summary>
        /// Test is the port has DNS activated.
        /// </summary>
        /// <param name="host">The IP address of the host.</param>
        /// <param name="port">The port to be checked.</param>
        /// <param name="verif">The DNS service.</param>
        /// <returns>Information about the service.</returns>
        private async Task<DeepScanResult> TestDNS(string host, int port, ServiceVerif verif)
        {
            var result = new DeepScanResult
            {
                PortNumber = port,
                ServiceTest = verif.Name
            };
            //connect to port, sent the message, analize the response
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(host, port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        client.ReceiveTimeout = 5000;
                        Console.WriteLine("size " + verif.SendSize);
                        if (verif.SendMessage != null && verif.SendSize != null)
                        {
                            byte[] sendPacket;
                            int dim = verif.SendSize.Value;
                            
                            sendPacket = verif.SendMessage;

                            Console.WriteLine(dim);
                            
                            foreach (var b in sendPacket)
                            {
                                Console.Write($"{b:X2} "); // Display each byte in hex format
                            }
                            Console.WriteLine(); // New line after printing all bytes
                                                        
                            byte[] queryLength = BitConverter.GetBytes((ushort)dim);
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(queryLength);
                            }
                            stream.Write(queryLength, 0, queryLength.Length);
                            Console.WriteLine(Encoding.ASCII.GetString(sendPacket));
                            Console.WriteLine(dim);
                            
                        
                            stream.Write(sendPacket, 0, dim);
                        }
                        byte[] responseLengthBuffer = new byte[2];
                        stream.Read(responseLengthBuffer, 0, responseLengthBuffer.Length);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(responseLengthBuffer);
                        }

                        int responseLength = BitConverter.ToUInt16(responseLengthBuffer, 0);
                        Console.Write("Lungimea raspunsului");
                        Console.WriteLine(responseLength);

                        //reading the response
                        byte[] buffer = new byte[4096];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine(bytesRead);


                        if (bytesRead >= verif.MinLengthResponse)
                        {
                            //test is the response contain the string from the database
                            bool open = CheckDNS(buffer);
                            if (open)
                            {
                                result.ServiceStatus = "open";
                            }
                            else
                            {
                                result.ServiceStatus = "close";
                            }
                        }
                        else
                        {
                            result.ServiceStatus = "close";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result.ServiceStatus = "close";
            }
            Console.WriteLine(result.ServiceStatus);
            return result;
        }

        private async Task<DeepScanResult> CheckHTTPS(string host, int port, ServiceVerif verif)
        {
            var result = new DeepScanResult
            {
                PortNumber = port,
                ServiceTest = verif.Name
            };
            try
            {
                using (TcpClient client =  new TcpClient(host, port)) 
                {
                    using (SslStream sslStream = new SslStream(client.GetStream(), false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                    {
                        sslStream.AuthenticateAsClient(host);
                    }
                }
            }
            catch
            {
                result.ServiceStatus = "close";
            }
            Console.WriteLine(result.ServiceStatus);
            return result;
        }

        /// <summary>
        /// Make a deep scan on a port to find out if what service is open.
        /// </summary>
        /// <param name="host">The IP address of the host.</param>
        /// <param name="port">The port to be checked.</param>
        /// <param name="context">The DB Context for taking the Service.</param>
        /// <returns>A list of information about the services status.</returns>
        public async Task<List<DeepScanResult>> DeepScanAsync(string host, int port, SpyFallDBcontext context)
        {
            var services = context.ServiceVerifs.ToList();
            List<Task<DeepScanResult>> tasks = new List<Task<DeepScanResult>>();

            foreach (var service in services)
            {
                Console.WriteLine(service.Name);
                //if (service.Name.StartsWith("HTTPS")) tasks.Add(CheckHTTPS(host, port, service));
                if (service.Name.StartsWith("DNS"))
                {
                    tasks.Add(TestDNS(host, port, service));
                }
                else tasks.Add(TestServiceAsync(host, port, service));
            }

            return (await Task.WhenAll(tasks)).ToList();


        }

        /// <summary>
        /// Verify if buffer contains bytes.
        /// </summary>
        /// <param name="buffer">The string to search in.</param>
        /// <param name="bufferLength">The length of the first string.</param>
        /// <param name="bytes">The string to be search.</param>
        /// <returns>if buffer contains bytes</returns>
        private bool ContainsBytes(byte[] buffer, int bufferLength,  byte[] bytes)
        {
            for (int i = 0; i < bufferLength; i++)
            {
                int j = 0;
                int k = i;
                while (bytes[j] != 0 && buffer[k] == bytes[j])
                {
                    j++; k++;
                }
                if (bytes[j] == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Verify if the buffer starts with bytes.
        /// </summary>
        /// <param name="buffer">The string to search in.</param>
        /// <param name="bytes">The string to be searched.</param>
        /// <returns>if the buffer starts with bytes.</returns>
        private bool StartsBytes(byte[] buffer, byte[] bytes)
        {
            int i = 0, j = 0;
            while (bytes[j] != 0 && bytes[j] == buffer[i])
            {
                j++;
                i++;
            }
            if (bytes[j] == 0) return true;
            return false;
        }

        /// <summary>
        /// A function that Check if the response is a DNS response.
        /// </summary>
        /// <param name="buffer">Response from host.</param>
        /// <returns>valid DNS response</returns>
        private bool CheckDNS(byte[] buffer)
        {
            if (!(buffer[0] == 219 && buffer[1] == 66)) return false;
            ushort flags = (ushort)((buffer[2] << 8) | buffer[3]);
            // Check if the response is valid
            if ((flags & 0x8000) != 0)
            {
                Console.WriteLine(flags);
                
                return (flags & 0x000F) == 0;
            }
            return false;

        }

        public static bool ValidateServerCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true; 
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }
    }
}
