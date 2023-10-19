using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
class Program{
    static void Main(string[] args)
    {
        UdpClient udpListener = new UdpClient(8888);
        Console.WriteLine("server started");
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (stopwatch.Elapsed.TotalSeconds > 1)
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                    if (receivedData != null)
                    {
                        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData);
                        Console.WriteLine($"Received {receivedMessage}");
                    }
                    stopwatch.Reset();
                }
                
            }
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
        }
    }
}