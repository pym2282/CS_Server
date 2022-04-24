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
           Packet.R_Signin packet = JsonConvert.DeserializeObject<Packet.R_Signin>(json);

           data.ConnectPlayer(packet.Id, client);
           data.GetPlayer(packet.Id).SendMessage("Hello " + packet.Id + " User\n");
       }
       public void R_SendMessage(string json)
       {
           Packet.S_SendMessage packet = JsonConvert.DeserializeObject<Packet.S_SendMessage>(json);

            //  msg = JsonConvert.SerializeObject(packet);

            //int size = msg.Length + 4;
            //int id = 2;
            //json[0] = (Byte)size;
            //json[1] |= (Byte)(size << 8);
            //json[2] = (Byte)id;
            //json[3] |= (Byte)(id << 8);
            //json = json.Concat<Byte>(Encoding.ASCII.GetBytes(msg)).ToArray();
            if (packet.targetId == 0) return;
            data.GetPlayer(packet.targetId).SendMessage(packet.message);
       }

    }
}
