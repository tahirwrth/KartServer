using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Quic;
using System.Text;
using System.Net.Mail;
class Program
{
    static void Main(string[] args)
    {
        const string serverIPAddress = "18.132.209.245"; // AWS 'Kart Server' Public IPv4
        const int serverPort = 8888;
        List<IPEndPoint> clients = new List<IPEndPoint>();
        Dictionary<IPEndPoint, List<string>> clientData = new Dictionary<IPEndPoint, List<string>>();

        UdpClient udpListener = new UdpClient(serverPort);
        Console.WriteLine($"Server started at {serverIPAddress}:{serverPort}");
        //PositionsTest(udpListener);

        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (Console.KeyAvailable)
                {  
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        return;
                    }
                }
                if (stopwatch.Elapsed.TotalSeconds > 1)
                {
                    Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    
                    byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                    if (receivedData != null)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receivedData);
                        
                        if (receivedMessage == "newPlayer" && clients.Contains(clientEndPoint) == false)
                        {
                            Console.WriteLine($"New Player at {clientEndPoint.Address} assigned to {clients.Count}");
                            UdpClient udpToClient = new UdpClient(clientEndPoint.Address.ToString(), clientEndPoint.Port);
                            string message = clients.Count.ToString();
                            byte[] encoded = Encoding.UTF8.GetBytes(message);
                            udpToClient.Send(encoded, encoded.Length);
                            clients.Add(clientEndPoint);
                        }
                        
                    }
                    stopwatch.Stop();
                    stopwatch.Restart();
                }
            }
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
            return;
        }
    }

    private static void PositionsTest(UdpClient udpListener)
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (Console.KeyAvailable)
                {  
                    ConsoleKeyInfo key = Console.ReadKey(true);
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

    private static List<IPEndPoint> HandshakeL(List<IPEndPoint> clients)
    {
        try
        {
            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                UdpClient udpClient = new UdpClient(clientEndPoint.Address.ToString(), clientEndPoint.Port);
                byte[] receivedData = udpClient.Receive(ref clientEndPoint);
                if (receivedData != null)
                {
                    string receivedMessage = Encoding.UTF8.GetString(receivedData);
                    if (receivedMessage == "newPlayer" && clients.Contains(clientEndPoint) == false)
                    {
                        string sendMessage = clients.Count.ToString();
                        byte[] sendEncoded = Encoding.UTF8.GetBytes(sendMessage);
                        udpClient.Send(sendEncoded, sendEncoded.Length);
                        clients.Add(clientEndPoint);
                        break;
                    }
                }
            }
            return clients;
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
            return clients;
        }
    }

    private static List<IPEndPoint> Handshake(UdpClient udpListener, List<IPEndPoint> clients)
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (Console.KeyAvailable)
                {  
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        return clients;
                    }
                }
                if (stopwatch.Elapsed.TotalSeconds > 1)
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    
                    byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                    if (receivedData != null)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receivedData);
                        if (receivedMessage == "newPlayer" && clients.Contains(clientEndPoint) == false)
                        {
                            Console.WriteLine($"New Player at {clientEndPoint.Address} assigned to {clients.Count}");
                            UdpClient udpToClient = new UdpClient(clientEndPoint.Address.ToString(), clientEndPoint.Port);
                            string message = clients.Count.ToString();
                            byte[] encoded = Encoding.UTF8.GetBytes(message);
                            udpToClient.Send(encoded, encoded.Length);
                            clients.Add(clientEndPoint);
                        }
                        
                    }
                    stopwatch.Stop();
                    stopwatch.Restart();
                }
            }
        } 
        catch 
        { 
            Console.WriteLine("Exception"); 
            return clients;
        }
    }
}