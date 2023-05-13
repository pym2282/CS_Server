using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Numerics;
using System.Drawing;
using System.Collections.Concurrent;

namespace Server
{
    public class Data : Server
    {
        //Dictionary<int, Player> players = new Dictionary<int, Player>();
        public ConcurrentDictionary<string, Player> players = new ConcurrentDictionary<string, Player>();

        public Player GetPlayer(string id)
        {
            if (players.ContainsKey(id))
            {
                if(players[id] == null)
                {
                    return null;
                }

                return players[id];
            }
            else
            {
                return null;
            }
        }

        public Data(Server root) : base(root)
        {

        }

        public void ConnectPlayer(string playerid, Socket socket)
        {
            Player player = new Player(socket);
            if(players.ContainsKey(playerid) == false)
             players.TryAdd(playerid, player);
        }
        public void ConnectPlayer(Socket socket)
        {
            Random  rand = new Random();
            string playerid = rand.Next().ToString();
            Player player = new Player(socket);
            if (players.ContainsKey(playerid) == false)
                players.TryAdd(playerid, player);
        }

        public void Disconnect(string playerid)
        {
            Player player = new Player();
            players.TryRemove(playerid, out player);
        }



    }

    public class Player
    {
        public Player()
        {
        }

        public Player(Socket socket)
        {
            connect = socket;
        }

        public void SendMessage(string msg)
        {
            try
            {
                connect.Send(Encoding.ASCII.GetBytes(msg));
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine($"User Already Disconnect. \n");
            }
        }
        public void SendBytes(Byte[] msg)
        {
            try
            {
                connect.Send(msg);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine($"User Already Disconnect. \n");
            }
        }


        Socket connect;

        public string id;

        public Vector3 position;
        public Vector3 rotation;
    }
};

