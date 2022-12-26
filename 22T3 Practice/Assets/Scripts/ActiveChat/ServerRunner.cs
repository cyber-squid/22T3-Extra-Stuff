using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveChat
{
    public class ServerRunner : MonoBehaviour
    {
        public Networking.ServerHandler server;

        void OnEnable()
        {
            server = new Networking.ServerHandler(3001, "localhost");
        }

        void Update()
        {
            server.Update();

           /* if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                server.BroadcastOut(new System.ArraySegment<byte> { });
            }*/
        }
    }
}