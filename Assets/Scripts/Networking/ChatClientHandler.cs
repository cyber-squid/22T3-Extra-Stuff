using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Public facing class that takes care of interfacing with the chat client, and keeps messages internal.
    /// </summary>
    public class ChatClientHandler
    {
        ChatClient client;

        public ChatClientHandler(int port, string serverIP)
        {
            client = new ChatClient(port, serverIP);
            client.OnMessageRecieved += OnMessage;
        }

        public void SendGlobalChatMessage(string playerName, string messageText)
        {
            client.SendGlobalChatMessage(playerName, messageText);
        }

        void OnMessage(Message message)
        {
            // do stuff like displaying client message here.
        }

        public void Update()
        {
            client.Update();
        }
    }
}
