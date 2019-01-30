using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;


namespace ServerApp
{
    class Program
    {
        static Process client;
        const string EXE_PATH = @"D:\Labs\3 курс\lab_С#\Practice\The_True_Lab1_Part5\ClientApp\ClientApp\bin\Debug\ClientApp.exe";

        static void Main(string[] args)
        {
            using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable))
            {
                StartClient(pipeServer.GetClientHandleAsString());
                pipeServer.DisposeLocalCopyOfClientHandle();

                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    string temp;
                    int num = 0;
                    // Wait for 'sync message' from the client.
                    do
                    {
                        Console.WriteLine("[SERVER] Wait for sync...");
                        temp = sr.ReadLine();
                    }
                    while (!temp.StartsWith("SYNC"));
                    Console.WriteLine("[SERVER] Client has connected");

                    do
                    {
                        temp = sr.ReadLine();
                        if (string.IsNullOrEmpty(temp))
                        {
                            Console.WriteLine("[SERVER] No response from client");
                        }
                        else
                        {
                            if(IsNumber(temp))
                            {
                                num = Convert.ToInt32(temp);
                                Console.WriteLine("[SERVER] Received: " + temp);
                            }
                            else
                            {
                                Console.WriteLine("[SERVER] Received: " + temp);
                                for(int i = 0; i < num; i++)
                                {
                                    Console.WriteLine(temp);
                                }
                            }
                        }
                        
                    }
                    while (!client.HasExited);
                }
                client.Close();
                Console.WriteLine("[SERVER] Client quit. Server terminating.");
                Console.ReadKey();
            }
        }

        static void StartClient(string clientHandle)
        {
            ProcessStartInfo info = new ProcessStartInfo(EXE_PATH);
            info.Arguments = clientHandle;
            info.UseShellExecute = false;
            client = Process.Start(info);
        }

        static bool IsNumber(string s)
        {
            char a = s[0];
            return Char.IsNumber(a);
        }

    }
}
