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
        Dictionary<int, string> clientData = new Dictionary<int, string>(); // int playerNumber, string position:rotation

        UdpClient udpListener = new UdpClient(serverPort);
        Console.WriteLine($"Server started at {serverIPAddress}:{serverPort}");

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
                    clients = Handshake(udpListener, clients);
                    stopwatch.Stop();
                    stopwatch.Restart();
                }
                

                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                
                if (receivedData != null)
                {
                    //string[] posRot = Encoding.UTF8.GetString(receivedData).Split(':');
                    clientData[clients.IndexOf(clientEndPoint)] = Encoding.UTF8.GetString(receivedData);
                }
                Broadcast(new UdpClient(clientEndPoint.Address.ToString(), clientEndPoint.Port), clientData);
            }
        } 
        catch (Exception e)
        { 
            Console.WriteLine(e.ToString()); 
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

    private static List<IPEndPoint> Handshake(UdpClient udpListener, List<IPEndPoint> clients)
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
                return clients;
            }
        
        }
        return clients;
    }
    private static void Broadcast(UdpClient udpListener, Dictionary<int, string> data)
    {
        string message;
        byte[] encoded;
        foreach(KeyValuePair<int, string> numPosRot in data)
        {
            message = $"{numPosRot.Key}!{numPosRot.Value}";
            // format-> playerNum!position:rotation
            encoded = Encoding.UTF8.GetBytes(message);
            udpListener.Send(encoded, encoded.Length);
        }
    }
}