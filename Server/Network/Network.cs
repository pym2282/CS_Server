﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Network : Server
    {
        Core core;
        //public Network(Server root) : base(root)
        //{
        //    //TestCore core;
        //}

        public Network(Server root) : base(root) 
        {
          // core = new Core(7777, root);
           core = new Core(8000, root);
        }


    }
}
