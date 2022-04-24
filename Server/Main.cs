using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Server;

namespace Server
{
    public class Start : Server
    {
        public Data dataInstance;
        public Network netInstance;
        public Recv recvInstance;
        void Init()
        {
            dataInstance = new Data(this);
            recvInstance = new Recv(this);


            netInstance = new Network(this);
        }
        Start() : base(null)
        {
            Init();
        }

        static void Main(string[] args)
        {
            Start main = new Start();
        }
    }
}
