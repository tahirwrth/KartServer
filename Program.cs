using System;
using System.Net;
using System.Net.Sockets;
class Program{
    static void Main(string[] args)
    {
        UdpClient udpListener = new UdpClient(8888);
        Console.WriteLine("server started");
        try
        {
            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                if (receivedData != null)
                {
                    string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData);
                    Console.WriteLine($"Received {receivedMessage}");
                    
                }
            }
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
        }
    }
}