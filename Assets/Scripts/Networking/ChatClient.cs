using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// A class that handles connecting to the server, and both sending and recieving chat messages for an individual user.
    /// </summary>
    internal class ChatClient : GenericClient
    {
        public ChatClient(int port, string serverIP) : base(port, serverIP)
        {

        }

        // create a chat message to be sent to server, then use the inheriting class' send function to do so.
        public void SendGlobalChatMessage(string playerName, string messageText)
        {
            GlobalChatMessage messageObject = new GlobalChatMessage(clientID, playerName, messageText);
            Send(messageObject);
        }

        protected override void OnData(System.ArraySegment<byte> data)
        {
            //TPClient.OnData = (data) => 
            ProcessOncomingMessage(data);
        }

        // check the array segments recieved and potentially convert them to chat messages to display.
        void ProcessOncomingMessage(System.ArraySegment<byte> data)
        {
            // get first byte in the array and convert it to our enum, denoting the message type.
            byte[] dataArray = data.Array;
            MessageType messageType = (MessageType)dataArray[0];


            if (messageType == MessageType.globalChatMessage)
            {
                // convert the data into a global chat message by passing it to a constructor.
                GlobalChatMessage messageObject = new GlobalChatMessage(dataArray);

                // determine how to display the message.
                DisplayGlobalMessage(messageObject);
            }

            /*if (messageType == MessageType.privateChatMessage)
            {
                GlobalChatMessage messageObject = new PrivateChatMessage(dataArray);
                ProcessPrivateMessage(messageObject);
            }*/

        }

        // put the global chat message on the user's screen.
        void DisplayGlobalMessage(GlobalChatMessage message)
        {
            Debug.Log("[" + message.playerName + "] said: " + message.chatMessage);
        }


        /*void ProcessPrivateMessage(PrivateChatMessage message)
        {
            Debug.Log("[" + message.playerName + "] said to you: " + message.chatMessage);
        }*/
    }
}
