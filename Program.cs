using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // AWS 'Kart Server' Public IPv4
        // due to AWS free tier limitations the server IP is not static
        string serverIPAddress = GetServerIP(); 
        const int serverPort = 8888;

        List<IPEndPoint> clients = new List<IPEndPoint>();

        // Dictionary holds all the data regarding clients
        // format = string ip, string velocity:position:rotation
        Dictionary<string, string> clientData = new Dictionary<string, string>();

        // creates new UdpClient at the specified port
        UdpClient udpListener = new UdpClient(serverPort);

        Console.WriteLine($"Server started at {serverIPAddress}:{serverPort}");

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

            string message = String.Empty;
            string[] ipMess;
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
            byte[] receivedData = udpListener.Receive(ref clientEndPoint);
            if (receivedData != null)
            {
                message = Encoding.UTF8.GetString(receivedData);
                if (string.IsNullOrEmpty(message) == false)
                {
                    ipMess = message.Split("+"); // [ipaddress],[(velocity):(position):(rotation)]
                    
                    if (clientData.ContainsKey(ipMess[0]))
                    {
                        // client is known, their data on the server is updated
                        // and all other client data in the dictionary is sent to them

                        clientData[ipMess[0]] = ipMess[1];
                        foreach(KeyValuePair<string,string> kvp in clientData)
                        {
                            BroadcastString(udpListener, clientEndPoint, $"{kvp.Key}+{kvp.Value}");
                        }
                    }
                    else
                    {
                        // unfamiliar client, added to dictionary
                        clientData.Add(ipMess[0], ipMess[1]);
                    }
                }
            }
        }
    }

    private static IPEndPoint PrintReceived(UdpClient udpListener, string ip1, string ip2)
    {
        string message = String.Empty;

        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        
        byte[] receivedData = udpListener.Receive(ref clientEndPoint);
        if (receivedData != null)
        {
            message = Encoding.UTF8.GetString(receivedData);
        }
        string[] ipMess = message.Split("+");
        if (ip1 == null)
        {
            ip1 = ipMess[0];
        }
        else if (ip2 == null)
        {
            ip2 = ipMess[0];
        }
        else if (ipMess[0] == ip1)
        {
            
        }
        else if (ipMess[1] == ip2)
        {

        }
        
        Console.WriteLine(ipMess[1]);
        return clientEndPoint;
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
    private static void BroadcastData(UdpClient udpListener, Dictionary<string, string> data)
    {
        string message;
        byte[] encoded;
        foreach(KeyValuePair<string, string> numPosRot in data)
        {
            message = $"{numPosRot.Key}!{numPosRot.Value}";
            // format-> playerNum!position:rotation
            encoded = Encoding.UTF8.GetBytes(message);
            udpListener.Send(encoded, encoded.Length);
        }
    }
    private static void BroadcastString(UdpClient udpListener, IPEndPoint ip, string message)
    {
        byte[] encoded = Encoding.UTF8.GetBytes(message);
        try
        {
            udpListener.Send(encoded, encoded.Length, ip);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private static string GetServerIP()
    {
        string ipAddress = String.Empty;

        // uses WebClient to get server's public IPv4 from https://api.ipify.org
        // which contains the IP as text in a HTML document
        using (WebClient webClient = new WebClient())
        {
            ipAddress = webClient.DownloadString("https://api.ipify.org");     
        }
        return ipAddress;
    }
}
