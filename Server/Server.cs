using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace Example
{
    class Server
    {
        static async Task RunServer(int port)
        {
            var ipep = new IPEndPoint(IPAddress.Any, port);
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(ipep);
                server.Listen(20);
                Console.WriteLine($"Server Start... Listen port {ipep.Port}...");
                var task = new Task(() =>
                {
                    while (true)
                    {
                        var client = server.Accept();
                        new Task(() =>
                        {
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            client.Send(Encoding.ASCII.GetBytes("Welcome YoungMin's Server!\r\n>"));

                            var sb = new StringBuilder();
                            using (client)
                            {
                                try 
                                { 
                                    while (true)
                                    {
                                        var binary = new Byte[1024];
                                        client.Receive(binary);
                                        var data = Encoding.ASCII.GetString(binary);
                                        sb.Append(data.Trim('\0'));
                                        if (sb.Length <= 2)
                                            continue;

                                        if (sb[0] != '{')
                                        {
                                            int size = sb[0];
                                            size |= sb[1];

                                            if(sb.Length == size)
                                            {
                                                int id = sb[2];
                                                id |= sb[3];

                                                Console.WriteLine("[Perfect] size = " + (size - 4) + ", id = " + id + ", data = " + sb.Remove(0,4));
                                            }
                                            else
                                            {
                                                Console.WriteLine("[Just Word] " + sb);
                                            }
                                        }
                                        else if (sb.Length > 2) //&& sb[sb.Length - 2] == '\r' && sb[sb.Length - 1] == '\n')
                                        {
                                            data = sb.ToString().Replace("\n", "").Replace("\r", "");
                                            if (String.IsNullOrWhiteSpace(data))
                                            {
                                                continue;
                                            }
                                            if ("EXIT".Equals(data, StringComparison.OrdinalIgnoreCase))
                                            {
                                                break;
                                            }
                                            Console.WriteLine("[PPRK version] " + data);
                                        }

                                        sb.Length = 0;
                                        var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n>");
                                        client.Send(sendMsg);
                                    }
                                }
                                 catch (SocketException)
                                {
                                    // 접속 끝김이 발생하면 Exception이 발생
                                }
                            Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            }
                        }).Start();
                    }
                });
                task.Start();
                await task;
            }
        }

        static void Main(string[] args)
        {
            RunServer(7777).Wait();

            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }
    }
}