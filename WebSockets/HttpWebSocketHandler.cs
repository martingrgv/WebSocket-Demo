using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketsTest.WebSockets;

public class HttpWebSocketHandler
{
    private readonly ConcurrentBag<WebSocket> webSockets;

    public HttpWebSocketHandler()
    {
        webSockets = new();
    }

    public async Task StartWebSocketServerAsync(string url)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        while (true)
        {
            var context = await listener.GetContextAsync();

            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);

                Console.WriteLine("Client connected successfully.");
                var webSocket = webSocketContext.WebSocket;

                webSockets.Add(webSocket);
                _ = Task.Run(() => HandleWebSocketAsync(webSocket));
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var response = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (response.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Client disconnected.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected.", CancellationToken.None);
                }

                if (response.MessageType == WebSocketMessageType.Text)
                {
                    var responseMessage = Encoding.UTF8.GetString(buffer, 0, response.Count);
                    responseMessage = $"Client response: {responseMessage}";
                    Console.WriteLine(responseMessage);

                    await BroadcastMessageAsync(webSocket, responseMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task BroadcastMessageAsync(WebSocket senderWebSocket, string responseMessage)
    {
        try
        {
            foreach (var webSocket in webSockets)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(responseMessage);

                    if (webSocket == senderWebSocket)
                    {
                        string senderMessage = "We have sent your message to everyone!";
                        buffer = Encoding.UTF8.GetBytes(senderMessage);
                    }

                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
