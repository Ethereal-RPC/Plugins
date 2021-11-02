using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetNodePluginClient.Model;
using EtherealC.Request.WebSocket;

namespace NetNodePluginClient.Request
{
    public class ServerNetNodeRequest:WebSocketRequest,IServerNetNodeRequest
    {
        public virtual NetNode GetNetNode(string servicename)
        {
            throw new NotImplementedException();
        }

        public override void Initializate()
        {

        }

        public override void UnInitialize()
        {

        }
    }
}
