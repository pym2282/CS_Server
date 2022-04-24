using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    public class Data : Server
    {
        public Dictionary<int, Player> players = new Dictionary<int, Player>();

        public Player GetPlayer(int id) { return players[id]; }

        public Data(Server root) : base(root)
        {

        }

        public void ConnectPlayer(int playerid, Socket socket)
        {
            Player player = new Player(socket);
            if(players.ContainsKey(playerid) == false)
             players.Add(playerid, player);
        }

        public void Disconnect(int playerid)
        {
            players.Remove(playerid);
        }



    }

    public class Player
    {
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
                Console.WriteLine($"User Already Disconnect.");
            }
        }

        Socket connect;
    }
};

