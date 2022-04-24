using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packet
{
    public class R_Signin
    {
        public int Id;
    }

    public class R_SendMessage
    {
        public int Id;
        public int targetId;
        public string message;
    }

    public class S_SendMessage
    {
        public int Id;
        public int targetId;
        public string message;
    }
    public class T_Int_Packet
    {
        public string packetname;
        public int type;
        public int value;
    }
}
