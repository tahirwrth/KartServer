using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
class Program
{
    static void Main(string[] args)
    {
        string serverIPAddress = GetServerIP(); // AWS 'Kart Server' Public IPv4
        const int serverPort = 8888;

        List<IPEndPoint> clients = new List<IPEndPoint>();
        Dictionary<int, string> clientData = new Dictionary<int, string>(); // int playerNumber, string position:rotation

        UdpClient udpListener = new UdpClient(serverPort);
        Console.WriteLine($"Server started at {serverIPAddress}:{serverPort}");
        
        PrintReceived(udpListener);

        /*try
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
        }*/
    }

    private static void PrintReceived(UdpClient udpListener)
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
                string message = String.Empty;

                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    
                byte[] receivedData = udpListener.Receive(ref clientEndPoint);
                if (receivedData != null)
                {
                    message = Encoding.UTF8.GetString(receivedData);
                }

                Console.WriteLine(message);

                BroadcastString(new UdpClient(clientEndPoint.Address.ToString(), clientEndPoint.Port), message);
                stopwatch.Stop();
                stopwatch.Restart();
            }
        }
    }
    


    private static string ReceiveString(UdpClient udpListener)
    {
        string message = String.Empty;

        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    
        byte[] receivedData = udpListener.Receive(ref clientEndPoint);
        if (receivedData != null)
        {
            message = Encoding.UTF8.GetString(receivedData);
        }

        return message;
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
            }
        }
        return clients;
    }
    private static void BroadcastAll(UdpClient udpListener, Dictionary<int, string> data)
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
    private static void BroadcastString(UdpClient udpListener, string message)
    {
        byte[] encoded;
        encoded = Encoding.UTF8.GetBytes(message);
        try
        {
            udpListener.Send(encoded, encoded.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static string GetServerIP()
    {
        string ipAddress = String.Empty;
        using (WebClient webClient = new WebClient())
        {
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            ipAddress = uTF8Encoding.GetString(webClient.DownloadData("https://whatismyip.com/automation/n09230945.asp"));            
        }
        return ipAddress;
    }
}