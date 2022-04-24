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

        public Recv(Server root) : base(root)
        {
            packets[2] = R_SendMessage;
        }

       public void R_Signin(string json, Socket client)
        {
            Packet.R_Signin? packet;
            packet = JsonConvert.DeserializeObject<Packet.R_Signin>(json);

            data.ConnectPlayer(packet.Id, client);
            data.GetPlayer(packet.Id).SendMessage("Hello " + packet.Id + " User\n");
        }
        public void R_SendMessage(string json)
        {
            Packet.S_SendMessage? packet;
            packet = JsonConvert.DeserializeObject<Packet.S_SendMessage>(json);

            data.GetPlayer(packet.targetId).SendMessage(packet.message);
        }
    }
}
