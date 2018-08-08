using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ExpCoinP2P
{
    public class P2PServer : WebSocketBehavior
    {
        bool chainSynched = false;
        WebSocketServer wss = null;

        public void Start()
        {
            var ipAddress = GetLocalIPAddress();
            wss = new WebSocketServer("ws://"+ ipAddress + ":"+Program.Port);//$"ws://127.0.0.1:{Program.Port}"
            wss.AddWebSocketService<P2PServer>("/Blockchain");
            wss.Start();
            Console.WriteLine($"Started server at ws://" + ipAddress + ":" + Program.Port);//ws://127.0.0.1:{Program.Port}"
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "Hi Server")
            {
                Console.WriteLine(e.Data);
                Send("Hi Client");
            }
            else
            {
                Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);

                if (newChain.IsValid() && newChain.Chain.Count > Program.ExpCoin.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    newTransactions.AddRange(Program.ExpCoin.PendingTransactions);

                    newChain.PendingTransactions = newTransactions;
                    Program.ExpCoin = newChain;
                }

                if (!chainSynched)
                {
                    Send(JsonConvert.SerializeObject(Program.ExpCoin));
                    chainSynched = true;
                }
            }
        }
    }
}
