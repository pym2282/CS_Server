using Newtonsoft.Json;
using Packet;
using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                    Byte[] json = new Byte[4];
                    if ("1".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        Packet.R_Signin packet = new Packet.R_Signin { playerid = "" };
                        msg = JsonConvert.SerializeObject(packet);

                        int size = msg.Length + 4;
                        int id = 1;
                        json[0] = (Byte)size;
                        json[1] |= (Byte)(size << 8);
                        json[2] = (Byte)id;
                        json[3] |= (Byte)(id << 8);
                        json = json.Concat<Byte>(Encoding.ASCII.GetBytes(msg)).ToArray();
                    }
                    else if ("2".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        Packet.R_Signin packet = new Packet.R_Signin { playerid = "" };
                        msg = JsonConvert.SerializeObject(packet);

                        int size = msg.Length + 4;
                        int id = 1;
                        json[0] = (Byte)size;
                        json[1] |= (Byte)(size << 8);
                        json[2] = (Byte)id;
                        json[3] |= (Byte)(id << 8);
                        json = json.Concat<Byte>(Encoding.ASCII.GetBytes(msg)).ToArray();
                    }
                    else if ("3".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        Packet.S_SendMessage packet = new Packet.S_SendMessage { Id = 1, targetId = 2, message = "Hello" };
                        msg = JsonConvert.SerializeObject(packet);

                        int size = msg.Length + 4;
                        int id = 2;
                        json[0] = (Byte)size;
                        json[1] |= (Byte)(size << 8);
                        json[2] = (Byte)id;
                        json[3] |= (Byte)(id << 8);
                        json = json.Concat<Byte>(Encoding.ASCII.GetBytes(msg)).ToArray();
                    }

                    client.Send(json);

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