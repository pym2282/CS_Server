﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    public class Core : Server
    {
        public Core(int port, Server root) : base(root)
        {
            RunServer(port).Wait();

            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }

        async Task RunServer(int port)
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
                            int playerid = 0;
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            Console.WriteLine($"Client : (From: {ip?.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            client.Send(Encoding.ASCII.GetBytes("Welcome YoungMin's Server!\r\n>"));

                            using (client)
                            {
                                try
                                {
                                    RecvPacket(client);
                                }
                                 catch (SocketException)
                                {
                                    Console.WriteLine($"{ip.Address.ToString()}'s Process Crush");
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

        void RecvPacket(Socket client)
        {
            var sb = new StringBuilder();
            while (true)
            {
                 string str = null;
                 var binary = new Byte[1024];
                 client.Receive(binary);
                 var packet = Encoding.ASCII.GetString(binary);
                 sb.Append(packet.Trim('\0'));
                 if (sb.Length <= 2)
                     continue;
                
                 if (sb[0] != '{')
                 {
                     int size = sb[0];
                     size |= sb[1];
                
                     if (sb.Length == size)
                     {
                         int id = sb[2];
                         id |= sb[3];
                
                         if (id == 1)
                         {
                             recv.R_Signin(sb.Remove(0, 4).ToString(), client);
                         }
                         else
                         {
                            recv.TestAllSend(sb.ToString());
                           // recv.packets[id](sb.Remove(0, 4).ToString());
                         }

                        Console.WriteLine($"{sb} + \n");
                       // str = "[Prefect] " + sb + "\n";
                     }
                     else
                     {
                         str = "[Just Word] " + sb + "\n";
                     }
                 }
                 else if (sb.Length > 2)
                 {
                     packet = sb.ToString().Replace("\n", "").Replace("\r", "");
                     if (String.IsNullOrWhiteSpace(packet))
                     {
                         continue;
                     }
                     if ("EXIT".Equals(packet, StringComparison.OrdinalIgnoreCase))
                     {
                         break;
                     }
                     str = "[PPRK version] " + sb + "\n";
                 }
                
                 sb.Length = 0;
                 var sendMsg = Encoding.ASCII.GetBytes(str);
               // Console.WriteLine($"{str}");
                //client.Send(sendMsg);
                
            }
        }
    }

    public class Server
    {
        protected Start start {
            get
            {
                return (Start)_root;
            }
        }
        protected Network net
        {
            get
            {
                return start.netInstance;
            }
        }
        protected Data data
        {
            get
            {
                return start.dataInstance;
            }
        }
        protected Recv recv
        {
            get
            {
                return start.recvInstance;
            }
        }
        public Server(Server root)
        {
            _root = root;
        }

        public Server _root;
    }
}