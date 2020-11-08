﻿using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

class MyTcpListener
{
  public static List<TcpClient> clients;

  public static void Main()
  {

  }

  public static void OpenServer()
  {
    TcpListener server=null;
    clients = new List<TcpClient>();

    try
    {
      Int32 port = 13000;
      IPAddress localAddr = IPAddress.Parse("26.129.184.182");
      server = new TcpListener(localAddr, port);
      server.Start();

      while(true)
      {
        Console.Write("Waiting for a connection... ");
        TcpClient client = server.AcceptTcpClient();
        clients.Add(client);
        Console.WriteLine($"Connected new client {clients.Count-1}!");
        Thread t = new Thread(new ParameterizedThreadStart(HandleConnection));
        t.Start(client);
        Console.WriteLine("New handler online");
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
       // Stop listening for new clients.
       server.Stop();
    }
  }

  public static void HandleConnection(Object c)
  {
        var client = c as TcpClient;
        Byte[] bytes = new Byte[256];
        String data = null;
        NetworkStream stream = client.GetStream();
        int i;

        while((i = stream.Read(bytes, 0, bytes.Length))!=0)
        {
          // Полученние данных от клиента
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
          Console.WriteLine("Received: {0}", data);

          byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

          // Send back a response.
          stream.Write(msg, 0, msg.Length);
          Console.WriteLine("Sent: {0}", data);
        }

        // Shutdown and end connection
        client.Close();
  }

  public void SendAnswer(TcpClient client, string message)
  {
          byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
          NetworkStream stream = client.GetStream();
          stream.Write(msg, 0, msg.Length);
          Console.WriteLine("Sent: {0}", message);
  }
}