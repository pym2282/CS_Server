using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Server
{
    public class Recv : Server
    {
        public delegate void OnRecvPacket(string json);

        public OnRecvPacket[] packets = new OnRecvPacket[100];

        enum PacketID
        {
            SendMessage = 2,
        }

        public Recv(Server root) : base(root)
        {
            packets[(int)PacketID.SendMessage] = R_SendMessage;
        }

       public void R_Signin(string json, Socket client)
       {
           Packet.T_Packet packet = JsonConvert.DeserializeObject<Packet.T_Packet>(json);

           data.ConnectPlayer(packet.value["iplayerid"], client);

           TestAllSend(json);
       }
       public void R_SendMessage(string json)
       {
           Packet.S_SendMessage packet = JsonConvert.DeserializeObject<Packet.S_SendMessage>(json);
       }
       public void TestAllSend(string json)
       {
            Packet.T_Packet packet = JsonConvert.DeserializeObject<Packet.T_Packet>(json);

            Byte[] bytes = new Byte[4];

            int size = json.Length + 4;
            int id = 0;
            bytes[0] = (Byte)size;
            bytes[1] |= (Byte)(size << 8);
            bytes[2] = (Byte)id;
            bytes[3] |= (Byte)(id << 8);
            bytes = bytes.Concat<Byte>(Encoding.ASCII.GetBytes(json)).ToArray();

            foreach (KeyValuePair<string, Player> info in data.players)
            {
                data.GetPlayer(info.Key).SendBytes(bytes);
            }
       }
    }
}
