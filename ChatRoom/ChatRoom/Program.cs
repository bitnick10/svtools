using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom
{
    class Program
    {
        static void Main(string[] args)
        {
            //FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:8170");
            // server.
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine(socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort + " Opend");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine(socket.ConnectionInfo.ClientIpAddress +":" + socket.ConnectionInfo.ClientPort + " Closed");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send(message));
                };
            });
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
