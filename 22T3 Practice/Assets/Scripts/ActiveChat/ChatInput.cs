using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveChat 
{ 
    public class ChatInput : MonoBehaviour
    {
        Networking.ChatClientHandler client;
        [SerializeField] string playerName;
        [SerializeField] string message;
        [SerializeField] KeyCode chatInputButton;
        [SerializeField] string serverIP = "localhost";


        void OnEnable()
        {
            //GameserverObj = GameObject.Find("ServerRunner");
            client = new Networking.ChatClientHandler(3001, serverIP);
        }

        
        void Update()
        {
            client.Update();

            if (Input.GetKeyDown(chatInputButton))
            {
                client.SendGlobalChatMessage(playerName, message);
            }
        }
    }
}