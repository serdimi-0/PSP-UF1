using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        const int PORT = 9999;
        string username;
        List<Missatge> missatges;
        Socket socket;
        byte[] usernameBytes;
        Thread listener;

        public ChatWindow(string username)
        {
            InitializeComponent();
            this.username = username;
            missatges = new List<Missatge>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lsbMsg.ItemsSource = missatges;

            // Connexio
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("localhost", PORT);

            // Enviem l'usuari
            usernameBytes = Encoding.UTF8.GetBytes("username");
            socket.Send(usernameBytes);

            // Creem el fil per escoltar
            listener = new Thread(() => listenerFunctionAsync(socket, missatges));
            listener.Start();

        }

        private void sendMessage()
        {

            if (txbMsg.Text == "")
            {
                return;
            }

            byte[] msgBytes = Encoding.UTF8.GetBytes(txbMsg.Text);

            // enviem l'user i el missatge
            socket.Send(usernameBytes);
            socket.Send(msgBytes);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void txbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                sendMessage();
            }
        }

        async Task listenerFunctionAsync(Socket socket, List<Missatge> missatges)
        {
            byte[] usernameBytes;
            string username;
            byte[] msgBytes;
            string msg;

            // Infinitament
            while (true)
            {
                // Esperem a rebre un username del remitent
                usernameBytes = new byte[1024];
                int bytesRec = socket.Receive(usernameBytes);
                username = Encoding.UTF8.GetString(usernameBytes, 0, bytesRec);

                // Rebem el missatge
                msgBytes = new byte[1024];
                bytesRec = socket.Receive(msgBytes);
                msg = Encoding.UTF8.GetString(msgBytes, 0, bytesRec);

                // Afegim el missatge a la llista
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Missatge missatge = new Missatge(username, msg);
                    missatges.Add(missatge);
                    lsbMsg.Items.Refresh();
                }));


            }
        }
    }
}
