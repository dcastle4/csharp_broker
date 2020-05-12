using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/*
 *      Name: Broker.cs
 *      Created by: Daniel Castle
 *      Class: CpE4020 Device Networks
 *      Professor: Dr. Billy Kihei
 * 
 *      Description:
 *          This C# program acts a simple broker that receives our sensor data
 *          from the publisher using a TCP server and then displays this data on
 *          an HTTP server. Both of these servers run in parallel using threads
 *          in order to allow the sensor data to be transfered as quickly 
 *          as possible.
 * 
 *      References:
 *          C# simple HTTP server tutorial: 
 *                  https://codingvision.net/networking/c-simple-http-server
 *          Example from MS TcpListener page: 
 *                  https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netframework-4.6.1
 *          Example code from Lab 7 (threading)
 * 
 */


class Broker
{
    public static string subMessage = "you should not see this";


    public static void Main()
    {

        // create threads for HTTP server and TCP server so they will operate in parallel
        Thread httpThread = new Thread(httpWork);
        Thread tcpThread = new Thread(tcpWork);
        httpThread.Start();
        tcpThread.Start();
    }


    public static void httpWork()
    {
        /*  HTTP initialization code starts here    */

        // create a new HTTP server
        HttpListener httpServer = new HttpListener();
        // add prefix to access HTTP server with 
        httpServer.Prefixes.Add("http://*:13000/");
        // start the HTTP server
        httpServer.Start();
        // log HTTP server starting [for debug/verification]
        Console.WriteLine("HTTP server started.");

        /*  HTTP initialization code ends here      */


        /*  HTTP loop code starts here              */

        while (true)
        {
            // create HTTP server context object, which contains the request and response objects
            HttpListenerContext context = httpServer.GetContext();

            // display to console that context/request has been received
            if (context.ToString() != null) { Console.WriteLine("HTTP request recieved."); }

            // create HTTP server response object and assign it to the response from context
            HttpListenerResponse response = context.Response;

            String page = context.Request.Url.LocalPath;


            // create a string to be displayed on HTTP server
            string httpMessage = null;

            switch (page)
            {
                case "/api/rotation/":
                    httpMessage = subMessage;
                    break;
                case "api/rotation":
                    httpMessage = subMessage;
                    break;
                case "api/rotation/":
                    httpMessage = subMessage;
                    break;
                case "/api/rotation":
                    httpMessage = subMessage;
                    break;
                default:
                    httpMessage = generateDefaultMessage();
                    break;
            }


            // create buffer for message that takes in httpMessage converted to bytes
            byte[] buffer = Encoding.UTF8.GetBytes(httpMessage);

            // assign the response's size to the buffer's size
            response.ContentLength64 = buffer.Length;
            // create a stream to write to the response with
            Stream st = response.OutputStream;
            // write buffer to the response
            st.Write(buffer, 0, buffer.Length);

            // close the response
            context.Response.Close();
        }

        /*  HTTP loop code ends here                */


    }


    public static void tcpWork()
    {
        // create the TCP server
        TcpListener tcpServer = null;

        try
        {
            // create variables for TCP port and IP
            Int32 port = 12345;
            IPAddress localAddr = IPAddress.Parse("192.168.7.1");

            // instantiate the TCP server with IP and port
            tcpServer = new TcpListener(localAddr, port);

            // start the TCP server
            tcpServer.Start();

            // buffer for receiving TCP data from BBB
            Byte[] buffer = new Byte[256];

            // Enter the listening loop.
            while (true)
            {
                Console.WriteLine("Waiting for BBB.");

                // create and instantiate client connection
                TcpClient client = tcpServer.AcceptTcpClient();
                Console.WriteLine("Connected to BBB TCP client!");


                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int count;

                // main send/receive loop.
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // translate BBB TCP response to string and display it
                    subMessage = System.Text.Encoding.ASCII.GetString(buffer, 0, count);
                    Console.WriteLine("Received: {0}", subMessage);

                    // send confirmation message back to BBB TCP client
                    byte[] response = System.Text.Encoding.ASCII.GetBytes("TCP data received!"); 
                    stream.Write(response, 0, response.Length);
                }

                // end the client connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // end the TCP server
            tcpServer.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
















































    // secret
    public static string generateDefaultMessage()
    {
        int count = 0;
        string defaultMessage = "";
        while (count != 700)
        {
            defaultMessage = defaultMessage + "NO WAY! NO WAY! NO WAY! NO WAY? ";
            count++;
        }
        defaultMessage = defaultMessage + "GET BLUE SPHERES!";
        return defaultMessage;
    }

}
