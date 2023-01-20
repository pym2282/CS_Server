using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Packet;
using System.Numerics;
using System.Drawing;
using System.Xml.Linq;

namespace Server
{
    public class Recv : Server
    {
        public delegate void OnRecvPacket(string json, Socket client);

        public Dictionary<string, OnRecvPacket> packets = new Dictionary<string, OnRecvPacket>();

        public Recv(Server root) : base(root)
        {
            packets["PlayerID"]             = Signin;
            packets["SendMessage"]          = SendMessage;
            packets["PlayerMove"]           = PlayerMove;
            packets["PlayerActionEvent"]    = PlayerActionEvent;
        }

        public void Signin(string json, Socket client)
        {
            R_Signin? _packet = JsonConvert.DeserializeObject<R_Signin>(json);

            if (_packet != null)
            {
                if (data.players.ContainsKey(_packet.playerid) == false)
                {
                    Player player = new Player(client);
                    player.id = _packet.playerid;
                    player.position = Vector3.Zero;
                    player.rotation = Vector3.Zero;
                    data.players.TryAdd(_packet.playerid, player);
                }

                recv.SignSync(_packet.playerid);

                foreach (KeyValuePair<string, Player> info in data.players)
                {
                    if (info.Key != _packet.playerid)
                        SendPacketToPlayer(info.Key, _packet);
                }
            }
        }

        public void PlayerMove(string json, Socket client)
        {
            Packet.R_PlayerMove packet = JsonConvert.DeserializeObject<Packet.R_PlayerMove>(json);

            if (data.players.ContainsKey(packet.PlayerId))
            {
                data.players[packet.PlayerId].position = packet.Position;
            }

            foreach (KeyValuePair<string, Player> info in data.players)
            {
                if(info.Key != packet.PlayerId)
                {
                    Packet.S_PlayerMove s_packet = new Packet.S_PlayerMove();
                    s_packet.PlayerId = info.Key;
                    s_packet.Position = Vector3.Zero;
                    s_packet.Rotation = Vector3.Zero;
                    //string jsondata = JsonConvert.SerializeObject(s_packet);
                    SendPacketToPlayer(info.Key, packet);
                }
            }
        }
        public void SendMessage(string json, Socket client)
        {
            TestAllEchoSend(json);
        }

        public void PlayerActionEvent(string json, Socket client)
        {
            Packet.S_PlayerActionEvent packet = JsonConvert.DeserializeObject<Packet.S_PlayerActionEvent>(json);

            foreach (KeyValuePair<string, Player> info in data.players)
            {
                if (info.Key != packet.PlayerId)
                {
                    SendPacketToPlayer(info.Key, packet);
                    //TestSendData(info.Key, json);
                }
            }
        }

        ///

        public void SignSync(string PlayerId)
        {
            foreach (KeyValuePair<string, Player> info in data.players)
            {
                if(PlayerId != info.Key)
                {
                    if(data.GetPlayer(info.Key).id != "")
                    {
                        Packet.S_Signin s_packet = new S_Signin();
                        s_packet.playerid = info.Key;
                        s_packet.Position = data.GetPlayer(info.Key).position;
                        s_packet.Rotation = data.GetPlayer(info.Key).rotation;
                        string jsondata = JsonConvert.SerializeObject(s_packet);

                        SendPacketToPlayer(PlayerId, jsondata);
                    }
                }
            }
        }

       public void TestAllSend(string json)
       {
            Packet.T_Packet packet = JsonConvert.DeserializeObject<Packet.T_Packet>(json);

            Byte[] bytes = new Byte[6];

            int size = json.Length + 6;
            int id = 0;
            bytes[0] = (Byte)size;
            bytes[1] |= (Byte)(size << 8);
            bytes[2] |= (Byte)(size << 16);
            bytes[3] |= (Byte)(size << 24);
            bytes[4] = (Byte)id;
            bytes[5] |= (Byte)(id << 8);
            bytes = bytes.Concat<Byte>(Encoding.ASCII.GetBytes(json)).ToArray();

            foreach (KeyValuePair<string, Player> info in data.players)
            {
                data.GetPlayer(info.Key).SendBytes(bytes);
            }
       }
        public void SendPacketToAll(Packet.T_Packet packet)
        {
           // byte[] packetBytes = packet.ToBytes();
            foreach (KeyValuePair<string, Player> info in data.players)
            {
           //     data.GetPlayer(info.Key).SendBytes(packetBytes);
            }
        }

        
        public void TestSendData(string playerid, string json)
        {
            Byte[] bytes = new Byte[6];

            int size = json.Length + 6;
            bytes[0] = (Byte)size;
            bytes[1] |= (Byte)(size << 8);
            bytes[2] |= (Byte)(size << 16);
            bytes[3] |= (Byte)(size << 24);
            bytes[4] |= 0;
            bytes[5] |= 0;
            bytes = bytes.Concat<Byte>(Encoding.ASCII.GetBytes(json)).ToArray();

            data.GetPlayer(playerid).SendBytes(bytes);
        }

        public void SendBroadcast(string json)
        {
            foreach (KeyValuePair<string, Player> info in data.players)
            {
                SendJsonToPlayer(info.Key, json);
                //data.GetPlayer(info.Key).SendMessage(json);
            }
        }

        public void SendOthercast(string playerid, string json)
        {
            if (playerid == "") return;

            foreach (KeyValuePair<string, Player> info in data.players)
            {
                if(playerid != info.Key)
                {
                    SendJsonToPlayer(info.Key, json);
                    //SendPacketToPlayer(info.Key, json);
                }
            }
        }

        public void TestAllEchoSend(string json)
        {
            foreach (KeyValuePair<string, Player> info in data.players)
            {
                data.GetPlayer(info.Key).SendMessage(json);
            }
        }

        public void SendPacketToPlayer<T>(string playerid, T packet)
        {
            string  json    = JsonConvert.SerializeObject(packet);
            int     size    = json.Length + 6;
            Int16     type    = 0;
            
            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] typeBytes = BitConverter.GetBytes(type);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            byte[] bytes = sizeBytes.Concat(typeBytes).Concat(jsonBytes).ToArray(); 
       
            data.GetPlayer(playerid).SendBytes(bytes);
        }
        public void SendJsonToPlayer(string playerid, string json)
        {
            int size = json.Length + 6;
            Int16 type = 0;

            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] typeBytes = BitConverter.GetBytes(type);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            byte[] bytes = sizeBytes.Concat(typeBytes).Concat(jsonBytes).ToArray();

            data.GetPlayer(playerid).SendBytes(bytes);
        }
    }
}