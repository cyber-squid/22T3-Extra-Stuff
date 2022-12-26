using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    public class ServerHandler //: ICommunicator
    {
        Telepathy.Server TPServer;
        List<int> players = new List<int>();

        public int port { get; private set; }
        public string serverIP { get; private set; }

        //delegate void Broadcast(Message message);
        internal Action<Message> Broadcast;

        public ServerHandler(int port, string serverIP)
        {
            TPServer = new Telepathy.Server(1024);
            this.port = port;
            this.serverIP = serverIP;

            TPServer.Start(this.port);
            Debug.Log("Server was made");

            TPServer.OnData += OnData;
            TPServer.OnConnected += SubPlayer;
            TPServer.OnDisconnected += UnsubPlayer;
        }

        void OnData(int ID, ArraySegment<byte> data)
        {
            Debug.Log("got some data from ID " + ID + ". Sending it out");
            BroadcastOut(data);
        }

        void BroadcastOut(ArraySegment<byte> data)//Message message)
        {
            //Broadcast(message);

            for (int i = 0; i < players.Count; i++)
            {
                TPServer.Send(players[i], data);
            }
            /*for (int i = players.Count; i < 0; i--)
            {
                TPServer.Send(players[i], data);
            }*/
        }

        void SubPlayer(int ID)
        {
            players.Add(ID);
            Debug.Log("player " + ID + " has connected");
        }

        void UnsubPlayer(int ID)
        {
            players.Remove(ID);
            Debug.Log("player " + ID + " has disconnected");
        }


        public void Update()
        {
            TPServer.Tick(100);
        }

    }
}
