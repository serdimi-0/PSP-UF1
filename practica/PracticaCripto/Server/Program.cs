using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    public class Program
    {
        const int PORT = 9999;
        const int MAX_CONNECTIONS = 10;
        static List<Socket> clients = new List<Socket>();

        public static void Main(string[] args)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress? ipAddr = ipHost.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            if(ipAddr == null)
            {
                Console.WriteLine("No s'ha trobat cap adreça IP");
                return;
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PORT);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(ipEndPoint);
                socket.Listen(MAX_CONNECTIONS);

                Console.WriteLine("Servidor iniciat en el port " + PORT +".\n" +
                    "Quin mètode d'encriptació vols fer servir?\n" +
                    "1 - DES\t\t2 - Personalitzat\n" +
                    "Opció: ");

                string? option;
                do
                {
                    option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            Console.WriteLine("Servidor iniciat amb encriptació DES");
                            break;
                        case "2":
                            Console.WriteLine("Servidor iniciat amb encriptació personalitzada");
                            break;
                        default:
                            Console.WriteLine("Opció no vàlida");
                            break;
                    }
                } while (option != "1" && option != "2" && option != "3");
                


                while (true)
                {
                    Console.WriteLine("Esperant connexions...");
                    Socket handler = socket.Accept();
                    clients.Add(handler);

                    // Fem un thread per cada client
                    Thread t = new Thread(() => clientThreadFunction(handler));
                    t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }

        private static void clientThreadFunction(Socket handler)
        {
            // Llegim el nom d'usuari
            byte[] usernameBytes = new byte[1024];
            int bytesRec = handler.Receive(usernameBytes);
            string username = Encoding.UTF8.GetString(usernameBytes, 0, bytesRec);
            Console.WriteLine("Usuari connectat: " + username);

            byte[] msgBytes;

            while (true)
            {
                Console.WriteLine($"[{username}] Esperant missatge...");

                // Esperem a rebre un username del remitent
                usernameBytes = new byte[1024];
                bytesRec = handler.Receive(usernameBytes);
                username = Encoding.UTF8.GetString(usernameBytes, 0, bytesRec);

                // Rebem el missatge
                byte[] tmp = new byte[2048];
                bytesRec = handler.Receive(tmp);

                msgBytes = new byte[bytesRec];
                Array.Copy(tmp, msgBytes, bytesRec);

                // Mostrem el missatge
                Console.WriteLine(username + ": " + BitConverter.ToString(msgBytes));

                // Enviem el missatge a tots els clients
                foreach (Socket client in clients)
                {
                    client.Send(usernameBytes);
                    client.Send(msgBytes);
                }

            }
        }
    }
}