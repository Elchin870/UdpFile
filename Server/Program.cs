using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Client_File
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Loopback;
            var port = 27001;
            var ep = new IPEndPoint(ip, port);
            var udpServer = new UdpClient(ep);
            Console.WriteLine("Server");

            try
            {
                while (true)
                {
                    var clientEp = new IPEndPoint(IPAddress.Any, 0);
                    var fileNameData = udpServer.Receive(ref clientEp);
                    var fileName = Encoding.UTF8.GetString(fileNameData).Trim();

                    Console.WriteLine($"{clientEp} is sending file to server");

                    var directoryPath = Path.Combine(Environment.CurrentDirectory, clientEp.Address.ToString());
                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    var filePath = Path.Combine(directoryPath, fileName);


                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        udpServer.Client.ReceiveTimeout = 5000; 
                        try
                        {
                            while (true)
                            {
                                var fileData = udpServer.Receive(ref clientEp); 
                                fs.Write(fileData, 0, fileData.Length);
                            }
                        }
                        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            Console.WriteLine("File transfer completed.");
                        }
                    }

                    Console.WriteLine("File received");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
