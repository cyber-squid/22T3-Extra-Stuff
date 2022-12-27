using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    internal interface ICommunicator
    {
        public void Send(Message message);

        public void Receive(System.ArraySegment<byte> data);
    }
}
