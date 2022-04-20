using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace Example
{
    class Client
    {
        static void Main(string[] args)
        {
            IPHostEntry HostInfo = Dns.GetHostEntry("pym.ipdisk.co.kr");
            var ipep = new IPEndPoint(HostInfo.AddressList[0], 7777);
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(ipep);
                new Task(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var binary = new Byte[1024];
                            client.Receive(binary);
                            var data = Encoding.ASCII.GetString(binary).Trim('\0');
                            if (String.IsNullOrWhiteSpace(data))
                            {
                                continue;
                            }

                            Console.Write(data);
                        }
                    }
                    catch (SocketException)
                    {
                        // 접속 끝김이 발생하면 Exception이 발생
                    }
                }).Start();

                while (true)
                {
                    var msg = Console.ReadLine();
                    client.Send(Encoding.ASCII.GetBytes(msg + "\r\n"));
                    if ("EXIT".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }

                Console.WriteLine($"Disconnected");
            }

            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }
    }
}