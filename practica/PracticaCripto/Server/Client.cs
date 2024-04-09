using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private Socket socket;
        private string? username;
        private string? ip;
        private string? clauPublica;

        public Client(Socket socket)
        {
            Socket = socket;
        }

        public Socket Socket { get => socket; set => socket = value; }
        public string? Username { get => username; set => username = value; }
        public string? Ip { get => ip; set => ip = value; }
        public string? ClauPublica { get => clauPublica; set => clauPublica = value; }
    }
}
