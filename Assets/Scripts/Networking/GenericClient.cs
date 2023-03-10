using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Client frame for other client types, such as the chat client, to inherit from and determine how to parse data.
    /// </summary>
    internal abstract class GenericClient
    {
        protected Telepathy.Client TPClient;
        protected int clientID;
        public delegate void MessageRecieved(Message message);
        public event MessageRecieved OnMessageRecieved;

        public GenericClient(int port, string serverIP) //Server serverToConnect)
        {
            TPClient = new Telepathy.Client(1024);
            TPClient.Connect(serverIP, port);

            //TPClient.OnDisconnected = () => serverToConnect.Broadcast += OnData;
            TPClient.OnData += OnData;
            // servr.Broadcast += OnData;
        }

        protected virtual void Send(Message message)
        {
            TPClient.Send(new System.ArraySegment<byte>(message.dataArray));
        }

        // inheriting classes can determine how to interpret incoming data. 
        protected abstract void OnData(System.ArraySegment<byte> data);


        void Subscribe()
        {
            //TPClient.
        }

        // processes messages and what type they are to invoke stuff like OnMessage. be sure to call in Update 
        public void Update()
        {
            TPClient.Tick(100);
        }

    }
}
