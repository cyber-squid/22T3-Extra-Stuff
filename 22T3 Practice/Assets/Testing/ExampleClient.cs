using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

public class ExampleClient : MonoBehaviour
{
    Telepathy.Client client = new Client(1024);

    // Start is called before the first frame update
    void Start()
    {
        //client.Connect("127.0.0.1", 3003);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            client.Connect("192.168.193.111", 3004);
            client.OnConnected = () => Debug.Log("Client Connected");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            byte[] dataArray = new byte[] { 255, 255, 255, 255, 255, 62 };
            client.Send(new System.ArraySegment<byte>(dataArray));

        }

        client.Tick(100);

    }
}
