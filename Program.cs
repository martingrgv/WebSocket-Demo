using WebSocketsTest.WebSockets;

class Program
{
    static async Task Main(string[] args)
    {
        string serverUrl = "http://127.0.0.1:8080/";

        var httpWebSocketHandler = new HttpWebSocketHandler(); 
        await httpWebSocketHandler.StartWebSocketServerAsync(serverUrl);
    }
}
