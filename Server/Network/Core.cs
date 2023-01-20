﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Packet;
using System.Numerics;
using static Server.Recv;

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
                            string playerid = "";
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            Console.WriteLine($"Client : (From: {ip?.Address.ToString()}:{ip?.Port}, Connection time: {DateTime.Now})");

                            using (client)
                            {
                                try
                                {
                                    //** Recv Packet */
                                    var sb = new StringBuilder();
                                    Queue<string> buffer = new Queue<string>();
                                    while (true)
                                    {
                                        var binary = new Byte[1024];
                                        int result = client.Receive(binary);
                                        var packet = Encoding.ASCII.GetString(binary);
                                        var recvData = packet.Trim('\0');

                                        if (result == 0) break;

                                        int size = BitConverter.ToInt32(binary, 0) - 6;
                                        int type = BitConverter.ToInt16(binary, 4);
                                        var json = recvData.Substring(6, size);
                                        recvData = recvData.Remove(0, size + 6);

                                        buffer.Enqueue(json);

                                        while (buffer.Count > 0)
                                        {
                                            if(type == 0)
                                            {
                                                BasePacket jsonObject = JsonConvert.DeserializeObject<BasePacket>(json);
                                                string fieldValue = jsonObject.packetid;

                                                if (fieldValue == "PlayerID")
                                                {
                                                    R_Signin? _packet = JsonConvert.DeserializeObject<R_Signin>(json);
                                                    playerid = _packet?.playerid ?? "";
                                                }
                                                json = buffer.Dequeue();
                                                recv.packets[fieldValue](json, client);

                                                Console.WriteLine($"{json}");
                                            }
                                            else if(type == 1)
                                            {
                                                json = buffer.Dequeue();
                                                recv.SendBroadcast(json);
                                                Console.WriteLine($"BroadCast Packet : {json}");
                                            }
                                            else if(type == 2)
                                            {
                                                json = buffer.Dequeue();
                                                recv.SendOthercast(playerid, json);
                                                Console.WriteLine($"OtherCast Packet : {json}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"ErrorPacket");
                                            }
                                        }
                                    }
                                }
                                catch (SocketException)
                                {
                                    Console.WriteLine($"{ip?.Address.ToString()}'s Process Crush");
                                }

                                //** Disconnect Client */
                                //Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            }

                            //** Disconnect Client */
                            Console.WriteLine($"Disconnected And Remove Player id : {playerid}");
                            data.Disconnect(playerid);
                        }).Start();
                    }
                });
                task.Start();
                await task;
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