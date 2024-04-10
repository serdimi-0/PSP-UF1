using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;

namespace Server
{
    public class Program
    {
        const int PORT = 9999;
        const int MAX_CONNECTIONS = 10;
        static List<Client> clients = new List<Client>();
        private static string? method;

        const string serverPublicKey = "<RSAKeyValue><Modulus>yXA14h2M/bLlMsERDyy9ErTh6VyFklaN+8GfMxQkVNcx9IS0AESWgVX49w802EcrYZib1oGJuPW1HTX1A1eyb75JXkolq2mSHr+GCjMqV7xlTljOf7u42av0kZKNOvWUQEDo/aOtYpOZH7CSmA3f4dhBGdLoh7TqhB/fGdMwnEk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string serverPrivateKey = "<RSAKeyValue><Modulus>yXA14h2M/bLlMsERDyy9ErTh6VyFklaN+8GfMxQkVNcx9IS0AESWgVX49w802EcrYZib1oGJuPW1HTX1A1eyb75JXkolq2mSHr+GCjMqV7xlTljOf7u42av0kZKNOvWUQEDo/aOtYpOZH7CSmA3f4dhBGdLoh7TqhB/fGdMwnEk=</Modulus><Exponent>AQAB</Exponent><P>904FD9XlvXN2bF4lOrEID/Coo64M5yJBDJd2XAHEzG/THadrSHag0Wt5VfCEOqJG1T0gVm+iEZI8uxN/Z8FM8w==</P><Q>0IVZRNVK+6BHQJoaTL19Nv8k1I/wEj9F57xRBxy/YcwD9LrGiFGEpqkvoEq80SNogjcrxoAnY6ktw4aIdXUQ0w==</Q><DP>FaIwlSsL9t+z21T9Ar5byzEtP2xJWqrHb/eL9g59jbi2iiCMJQGjnc4+BgONPafWdG7tdkI7tjfJsj/JZGUnew==</DP><DQ>zoDxCikNbCqrxb+Xgh46jieZyuSNRRTiXv/xYtDGe8y2sjvyd3f7na15nA3H9npRenu234t09s7JopRuOZxovw==</DQ><InverseQ>0BmUXX/99QckT3mII2mXnVbDQdWF1EE1N90mhAOZI4gIBMqliNmU5p5JekybmIreosgxpPb+uHasxAcCkHJJpg==</InverseQ><D>XMjV2Pg5mCYN0ooBfJbj5DjzEhVu5Q//SGZQGSv+7CFPPkzdyH3PQXXDPrF/atulTMgJiA2UzWBH81OGLur3IhVzFtoQd3DSVY5BR4hA9FjsLdfCWKiBpnU0l8yvV3uM6q3mov3aX4JMSo/caRRr0rVnIJ3gHGmjEtAoy3Qcty0=</D></RSAKeyValue>";


        public static void Main(string[] args)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress? ipAddr = ipHost.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            if (ipAddr == null)
            {
                Console.WriteLine("No s'ha trobat cap adreça IP");
                return;
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PORT);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEndPoint);
            socket.Listen(MAX_CONNECTIONS);

            Console.WriteLine("Servidor iniciat en el port " + PORT + ".\n" +
                "Quin mètode d'encriptació vols fer servir?\n" +
                "\t1 - DES\t\n" +
                "\t2 - DES + RSA\n" +
                "\t3 - Personalitzat\n" +
                "Opció: ");

            string? option;
            do
            {
                option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        method = "des";
                        Console.WriteLine("\nServidor iniciat amb encriptació simètrica DES");
                        break;
                    case "2":
                        method = "rsa";
                        Console.WriteLine("\nServidor iniciat amb encriptació asimètrica RSA");
                        break;
                    case "3":
                        method = "inv";
                        Console.WriteLine("\nServidor iniciat amb encriptació personalitzada");
                        break;
                    default:
                        Console.WriteLine("\nOpció no vàlida");
                        break;
                }
            } while (option != "1" && option != "2" && option != "3");


            // ESPEREM CONNEXIONS
            while (true)
            {
                // Fem un thread per cada client
                // Aquest thread es quedarà escoltant els missatges del client i els reenviarà a tots els clients
                Thread t;

                // Acceptem connexions
                Console.WriteLine("Esperant connexions...");
                Socket handler = socket.Accept();

                Client client = new Client(handler);

                // Obtenim la IP del client
                string ip = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
                client.Ip = ip;

                // Llegim el nom d'usuari
                byte[] usernameBytes = new byte[1024];
                int bytesRec = handler.Receive(usernameBytes);
                string username = Encoding.UTF8.GetString(usernameBytes, 0, bytesRec);
                Console.WriteLine("Usuari connectat: " + username + " (" + ip + ")");
                client.Username = username;

                // Enviar el tipus d'encriptació
                byte[] methodBytes = Encoding.UTF8.GetBytes(method);
                handler.Send(methodBytes);

                switch (method)
                {
                    case "rsa":

                        // Enviem la clau pública del servidor
                        handler.Send(Encoding.UTF8.GetBytes(serverPublicKey));
                        Console.WriteLine($"Clau pública del servidor enviada a {username}, {ip}");

                        // Rebem la clau pública del client
                        byte[] tmp = new byte[2048];
                        int size = handler.Receive(tmp);
                        byte[] publicKeyBytes = new byte[size];
                        Array.Copy(tmp, publicKeyBytes, size);
                        client.ClauPublica = Encoding.UTF8.GetString(publicKeyBytes);

                        Console.WriteLine($"Clau pública de {username}, {ip} rebuda: {Encoding.UTF8.GetString(publicKeyBytes)}");

                        lock (clients)
                            clients.Add(client);

                        t = new Thread(() => clientRSAThreadFunction(client));
                        t.Start();
                        break;
                    case "des":
                    case "inv":
                        lock (clients)
                            clients.Add(client);

                        t = new Thread(() => clientSymmetricThreadFunction(client));
                        t.Start();
                        break;
                    default:
                        break;
                }

                lock (clients)
                    Console.WriteLine("Clients connectats: " + clients.Count);
            }
        }


        private static void clientSymmetricThreadFunction(Client client)
        {
            byte[] msgBytes;
            int bytesRec;


            while (true)
            {
                Console.WriteLine($"[{client.Username}, {client.Ip}] Esperant missatge...");

                // Rebem el missatge, que ha de tenir la longitud exacta que ens envia
                byte[] tmp = new byte[2048];
                bytesRec = client.Socket.Receive(tmp);
                msgBytes = new byte[bytesRec];
                Array.Copy(tmp, msgBytes, bytesRec);

                // Si el missatge és BYE, tanquem la connexió
                if (Encoding.ASCII.GetString(msgBytes) == "BYE")
                {
                    // protegim l'acces a la llista de clients
                    lock (clients)
                    {
                        clients.Remove(client);
                        Console.WriteLine($"[{client.Username}, {client.Ip}] Desconnectat.");
                        Console.WriteLine("Clients connectats: " + clients.Count);
                    }
                    client.Socket.Close();
                    return;
                }

                // Mostrem el missatge
                Console.WriteLine($"[{client.Username}, {client.Ip}] Missatge enviat: {BitConverter.ToString(msgBytes)}");

                // Enviem el missatge a tots els clients
                foreach (Client c in clients)
                {
                    c.Socket.Send(Encoding.ASCII.GetBytes(client.Username));
                    c.Socket.Send(msgBytes);
                }

            }

        }

        private static void clientRSAThreadFunction(Client client)
        {
            byte[] msgBytes, keyBytes;
            int bytesRec;


            while (true)
            {
                Console.WriteLine($"[{client.Username}, {client.Ip}] Esperant missatge...");

                // Rebem el missatge, que ha de tenir la longitud exacta que ens envia
                byte[] tmp = new byte[2048];
                bytesRec = client.Socket.Receive(tmp);
                msgBytes = new byte[bytesRec];
                Array.Copy(tmp, msgBytes, bytesRec);

                // Si el missatge és BYE, tanquem la connexió
                if (Encoding.ASCII.GetString(msgBytes) == "BYE")
                {
                    // protegim l'acces a la llista de clients
                    lock (clients)
                    {
                        clients.Remove(client);
                        Console.WriteLine($"[{client.Username}, {client.Ip}] Desconnectat.");
                        Console.WriteLine("Clients connectats: " + clients.Count);
                    }
                    client.Socket.Close();
                    return;
                }
                Console.WriteLine($"[{client.Username}, {client.Ip}] Missatge enviat");

                // Rebem la clau simètrica
                tmp = new byte[2048];
                bytesRec = client.Socket.Receive(tmp);
                keyBytes = new byte[bytesRec];
                Array.Copy(tmp, keyBytes, bytesRec);
                Console.WriteLine($"[{client.Username}, {client.Ip}] Clau del missatge rebuda");

                // Desencriptem la clau simètrica amb la privada del servidor
                string key = RSADecrypt(keyBytes, serverPrivateKey);
                Console.WriteLine($"[{client.Username}, {client.Ip}] Clau desencriptada: {key}");

                // Enviem el missatge a tots els clients
                Console.WriteLine($"[{client.Username}, {client.Ip}] Enviant missatges amb claus encriptades...");
                foreach (Client c in clients)
                {
                    c.Socket.Send(Encoding.ASCII.GetBytes(client.Username));
                    c.Socket.Send(msgBytes);
                    // A part del missatage, enviem la clau encriptada amb la pública del client
                    c.Socket.Send(RSAEncrypt(key, c.ClauPublica));
                }

            }

        }

        public static byte[] RSAEncrypt(string missatge, string clauXml)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(clauXml);

            byte[] missatgeBytes = Encoding.ASCII.GetBytes(missatge);
            byte[] missatgeEncriptat = rsa.Encrypt(missatgeBytes, false);

            return missatgeEncriptat;
        }

        public static string RSADecrypt(byte[] missatge, string clauXml)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(clauXml);

            byte[] missatgeDesencriptat = rsa.Decrypt(missatge, false);
            string missatgeDesencriptatString = Encoding.ASCII.GetString(missatgeDesencriptat);

            return missatgeDesencriptatString;
        }
    }
}