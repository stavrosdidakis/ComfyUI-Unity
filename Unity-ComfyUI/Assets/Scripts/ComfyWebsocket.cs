using System.Collections.Generic;

using System;

using System.Collections;

using System.Net.WebSockets;

using System.Text;

using System.Threading;

using UnityEngine;

using UnityEngine.Networking;



public class ResponseDataWebsocket

{

    public string prompt_id;

}

public class ComfyWebsocket : MonoBehaviour

{

    private string serverAddress = "127.0.0.1:8188";

    private string clientId = Guid.NewGuid().ToString();

    private ClientWebSocket ws = new ClientWebSocket();



    public ComfyImageCtr comfyImageCtr;

    async void Start()

    {

     

        await ws.ConnectAsync(new Uri($"ws://{serverAddress}/ws?clientId={clientId}"), CancellationToken.None);

        StartListening();





    }

    public string promptID;

    private async void StartListening()

    {

        var buffer = new byte[1024 * 4];

        WebSocketReceiveResult result = null;



        while (ws.State == WebSocketState.Open)

        {

            var stringBuilder = new StringBuilder();

            do

            {

                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)

                {

                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                }

                else

                {

                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    stringBuilder.Append(str);

                }

            }

            while (!result.EndOfMessage);



            string response = stringBuilder.ToString();

            Debug.Log("Received: " + response);

        

           if (response.Contains("\"queue_remaining\": 0"))

            {

                comfyImageCtr.RequestFileName(promptID);

            }

        



        }

    }



   



    void OnDestroy()

    {

        if (ws != null && ws.State == WebSocketState.Open)

        {

            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

        }

    }

}

