using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Quic;
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
                if (Console.KeyAvailable)
                {  
                    ConsoleKeyInfo key = System.Console.ReadKey(true);//true don't print char on console
                    if (key.Key == ConsoleKey.Q)
                    {
                        return;
                    }
                }
                if (stopwatch.Elapsed.TotalSeconds > 1)
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                    if (receivedData != null)
                    {
                        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData);
                        Console.WriteLine($"Received {receivedMessage}");
                    }
                    stopwatch.Stop();
                    stopwatch.Restart();
                }
                
            }
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
        }
    }
}