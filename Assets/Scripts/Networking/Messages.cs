using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{

    internal enum MessageType
    {
        globalChatMessage,
        privateChatMessage,
        /* shooterPlayerCrosshairMove,
        shooterPlayerBeginCharge,
        shooterPlayerAimingCharge,
        shooterPlayerFireCharge,
        /* arrowReset,
        arrowAim, // set arrow rotation
        arrowFire, // set animation trigger, arrow physics and velocity
        targetSpawned,
        targetBroken, // destroy target with particles
        playerScoreUpdate, */
        nullMessage
    }

    internal enum CreationMode
    {
        sending,
        receiving
    }


    /// <summary>
    /// Class for other messages to inherit from, to be instantiated when sending or recieving a message (use the appropriate constructor).
    /// </summary>
    internal abstract class Message
    {
        // determines how the message should be handled (constructor use, who to send to).
        protected MessageType typeCode = MessageType.nullMessage; 

        protected int sender; // ID of the machine / player sending the message
        protected int reciever; // ID of the reciving machine / player

        protected int typeCodeArrayPos = 0;
        protected int senderArrayPos = 4;
        protected int senderRecieverPos = 8;

        // array of bytes to put the relevant data into on instantiation. this is the data sent by Telepathy's service 
        public byte[] dataArray { get; protected set; }

        public Message() { }
        //public Message(int senderID, int recieverID) { /* enter data here to convert to byte array. */ }

        Message(byte[] messageData) { /* convert data in given byte array to the data the message previously represents. */ }
    }

    
    /// <summary>
    /// Message type instantiated by a client when inputting a chat message for all clients connected to the server to read.
    /// </summary>
    internal class GlobalChatMessage : Message
    {
        public string playerName { get; protected set; }
        
        public string chatMessage { get; protected set; }

        int msgTerminator = 0; // signifies end of message so we don't read more than needed

        // convert a string passed in with the sender's ID into a byte array, to be sent via Telepathy
        public GlobalChatMessage(int senderID, string playerName, string chatMessage)
        {
            typeCode = MessageType.globalChatMessage;
            dataArray = new byte[typeCodeArrayPos + senderArrayPos 
                                + playerName.Length + 1    
                                + chatMessage.Length + 1
                                + 1]; // add space for terminator

            dataArray[typeCodeArrayPos] = (byte)typeCode;
            dataArray[senderArrayPos] = (byte)senderID;


            char[] nameChars = playerName.ToCharArray();
            int msgPosInDataArray = senderArrayPos + 1;

            for (int i = 0; i < nameChars.Length; i++)
            {
                dataArray[msgPosInDataArray] = (byte)nameChars[i];
                msgPosInDataArray++;
            }

            dataArray[msgPosInDataArray] = (byte)'\0';
            msgPosInDataArray++;

            char[] chatMsgChars = chatMessage.ToCharArray();
            for (int i = 0; i < chatMessage.Length; i++)
            {
                dataArray[msgPosInDataArray] = (byte)chatMsgChars[i];
                msgPosInDataArray++;
            }

            dataArray[msgPosInDataArray] = (byte)msgTerminator;
        }

        // convert the given byte array into the sender ID and the string containing the message
        public GlobalChatMessage(byte[] chatMessageData)
        {
            dataArray = chatMessageData;

            sender = dataArray[senderArrayPos];
            int msgPosInDataArray = senderArrayPos + 1;


            //char[] nameChars = new char[dataArray.Length];
            List<char> nameChars = new List<char>();
            for (int i = 0; (char)dataArray[msgPosInDataArray] != '\0'; i++)
            {
                nameChars.Add((char)dataArray[msgPosInDataArray]);
                //nameChars[i] = (char)dataArray[msgPosInDataArray];
                msgPosInDataArray++;
            }

            playerName = new string(nameChars.ToArray());
            msgPosInDataArray++;

            //char[] chatMsgChars = new char[dataArray.Length];
            List<char> chatMsgChars = new List<char>();
            for (int i = 0; (char)dataArray[msgPosInDataArray] != '\0'; i++)
            {
                chatMsgChars.Add((char)dataArray[msgPosInDataArray]);
                //chatMsgChars[i] = (char)dataArray[msgPosInDataArray];
                msgPosInDataArray++;
            }

            chatMessage = new string(chatMsgChars.ToArray());
        }
    }


    /// <summary>
    /// Empty message used to signify invalid or placeholder values.
    /// </summary>
    internal class NullMessage : Message
    {
        public NullMessage()
        {
            //return new System.NotImplementedException();
        }
    }
}