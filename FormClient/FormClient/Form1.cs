using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;


namespace FormClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Client client = new Client(Environment.GetCommandLineArgs()[0], richTextBox1);
            client.Start();
            Console.ReadKey();
        }

    }

    public class Client
    {
        NamedPipeClientStream clientPipe;
        int clientID;
        RichTextBox tb;
        public Client(string serverID,RichTextBox tb)
        {
            this.tb = tb;

            clientID = Process.GetCurrentProcess().Id;
            clientPipe = new NamedPipeClientStream(".", "pipe" + serverID,
                PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
        }

        public void Start()
        {
            clientPipe.Connect();
            byte[] inBytes = new byte[100];
            clientPipe.Read(inBytes, 0, 100);
            string inStr = Encoding.ASCII.GetString(inBytes);
            msg("Received from server: " + inStr);

            //send
            msg("Sending message to server...");
            byte[] outBytes = new byte[100];
            outBytes = Encoding.ASCII.GetBytes("Hello, server! I'm your client #" + clientID);
            clientPipe.Write(outBytes, 0, outBytes.Length);
            //чекаємо поки сервер отримає повідомлення
            clientPipe.WaitForPipeDrain();
            msg("The message has been received by server");
        }

        void msg(string str)
        {
            tb.Text += str + "\n";
        }
    }
}
