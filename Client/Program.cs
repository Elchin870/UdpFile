using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Loopback;
            var port = 27001;
            var ep = new IPEndPoint(ip, port);
            var udpClient = new UdpClient();
            Console.WriteLine("Client");

            try
            {
                Console.Write("Enter file path: ");
                var filePath = Console.ReadLine()!;
                var fileName = Path.GetFileName(filePath);
                var fileNameBytes = Encoding.UTF8.GetBytes(fileName + "\n");
                udpClient.Send(fileNameBytes, fileNameBytes.Length, ep); 

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    var bytes = new byte[1024];
                    int len;
                    while ((len = fs.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        udpClient.Send(bytes, len, ep);
                    }
                }

                udpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
