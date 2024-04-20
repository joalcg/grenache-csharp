﻿using System;
using System.Threading.Tasks;
using Grenache.Models.PeerRPC;

namespace Grenache.Example.Server
{
  class Program
  {
    static HttpPeerRPCServer _server;
    static async Task Main(string[] args)
    {
      Utils.HttpUtil.SetClient(new System.Net.Http.HttpClient());

      Link link = new("http://127.0.0.1:30001");
      _server = new HttpPeerRPCServer(link, 10000);
      _server.AddRequestHandler((req, res) =>
      {
        res.Invoke(new RpcServerResponse { RId = req.RId, Data = req.Payload });
      });
      var started = await _server.Listen("rpc_ping", 7070);
      if (!started) throw new Exception("Couldn't start the server!");
      Console.WriteLine("Server started!");

      CloseHandler();

      await _server.ListenerTask; // used to keep the app always running
    }

    static void CloseHandler()
    {
      Task.Factory.StartNew(async () =>
      {
        Console.WriteLine("Press any key to close the server");
        Console.Read();
        await _server.Close();
      });
    }
  }
}