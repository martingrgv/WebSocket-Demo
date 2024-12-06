using WebSocketsTest.WebSockets;

class Program
{
    static async Task Main(string[] args)
    {
        string httpServerUrl = "http://127.0.0.1:8080/";
        string tcpServerUrl = "127.0.0.1";
        int tcpServerPort = 8080;

        // var httpWebSocketHandler = new HttpWebSocketHandler(); 
        // await httpWebSocketHandler.StartWebSocketServerAsync(serverUrl);

        var tcpListenerWebSocketHandler = new TcpListenerWebSocketHandler();
        await tcpListenerWebSocketHandler.StartWebSocketServerAsync(tcpServerUrl, tcpServerPort);
    }
}
