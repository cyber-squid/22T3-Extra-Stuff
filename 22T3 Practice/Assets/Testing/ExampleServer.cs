using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

public class ExampleServer : MonoBehaviour
{
    Telepathy.Server server = new Server(1024);

    // Start is called before the first frame update
    void Start()
    {
        server.Start(3001);
    }

    // Update is called once per frame
    void Update()
    {
        server.Tick(100);

        server.OnConnected = (connectionId) => Debug.Log(connectionId + " Connected");
        server.OnData = (data, arrayData) => Debug.Log(data);
    }
}
