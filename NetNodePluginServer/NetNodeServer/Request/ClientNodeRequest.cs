using EtherealS.Core.Model;
using EtherealS.Request.WebSocket;
using NetNodePlugin.Model;
using System;

namespace NetNodePlugin.NetNodeServer.Request
{
    public class ClientNodeRequest : WebSocketRequest
    {
        public ClientNodeRequest()
        {
            name = "ClientNetNodeService";
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<NetNode>("NetNode");
        }
        public override void Initialize()
        {
            Console.WriteLine($"{name}服务已部署");
        }

        public override void UnInitialize()
        {
            Console.WriteLine($"{name}服务已注销");
        }
    }
}
