using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Packet
{
    public class BasePacket
    {
        public string packetid;
    }

    public class R_Signin : BasePacket
    {
        public string playerid;
    }

    public class S_Signin : BasePacket
    {
        public S_Signin()
        {
            packetid = "Signin";
        }
        public string playerid;

        public Vector3 Position;
        public Vector3 Rotation;
    }

    public class R_SendMessage : BasePacket
    {
        public int Id;
        public int targetId;
        public string message;
    }

    public class S_SendMessage : BasePacket
    {
        public S_SendMessage()
        {
            packetid = "SendMessage";
        }
        public int Id;
        public int targetId;
        public string message;
    }
    public class R_PlayerMove : BasePacket
    {
        public int Id;

        public string PlayerId;

        public Vector3 Position;
        public Vector3 Rotation;

        public float Speed;
    }

    public class S_PlayerMove : BasePacket
    {
        public S_PlayerMove()
        {
            packetid = "PlayerMove";
        }
        public int Id;

        public string PlayerId;

        public Vector3 Position;
        public Vector3 Rotation;

        public float Speed;
    }
    public class S_PlayerActionEvent : BasePacket
    {
        public S_PlayerActionEvent()
        {
            packetid = "PlayerActionEvent";
        }
        public string PlayerId;

        public string ActionId;
    }

    public class T_Int_Packet
    {
        public string packetname;
        public int type;
        public int value;
    }
    public class T_Vector_Packet
    {
        public string packetname;
        public int type;
        //public Vector3D value;
    }
    public class T_Packet
    {
        public string packetname;
        public int type;
        public Dictionary<string, string> value;
    }
}
