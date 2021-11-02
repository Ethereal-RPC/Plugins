using NetNodePlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetNodePlugin.NetNodeClient.Request
{
    public class ServerNodeRequest : EtherealC.Request.WebSocket.WebSocketRequest,IServerNodeRequest
    {
        public ServerNodeRequest()
        {
            name = "ServerNetNodeService";
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<Model.NetNode>("NetNode");
        }

        public override void Initializate()
        {

        }

        public bool Register(NetNode node)
        {
            return true;
        }

        public override void UnInitialize()
        {

        }
    }
}
