using System;
using System.Net;
using System.Net.Sockets;

namespace WebSocketsTest.WebSockets;

public class TcpListenerWebSocketHandler
{
    public async Task StartWebSocketServerAsync(string ip, int port)
    {
        var listener = new TcpListener(IPAddress.Parse(ip), port); 
        listener.Start();
        Console.WriteLine($"WebSocket Server started on {ip}:{port}.");

        var client = await listener.AcceptTcpClientAsync();
        var webSocket = await listener.AcceptSocketAsync();

        Console.WriteLine("Client connected.");
    }
}
