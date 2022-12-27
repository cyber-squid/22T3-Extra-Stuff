using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Class which takes care of recieving client messages, and broadcasting them back out. Simply instantiate and give
    /// it an IP and port, and update.
    /// </summary>
    public class ServerHandler //: ICommunicator
    {
        Telepathy.Server TPServer;
        List<int> players = new List<int>();

        public int port { get; private set; }
        public string serverIP { get; private set; }

        //delegate void Broadcast(Message message);
        internal Action<Message> Broadcast;

        // set up the Telepathy server when instantiated.
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

        // when we get the ondata event called, we should send the data out to relevant subs
        void OnData(int ID, ArraySegment<byte> data)
        {
            Debug.Log("got some data from ID " + ID + ". Sending it out");
            BroadcastOut(data);
        }

        // get all the IDs subbed to this server and send them the data we just got.
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
