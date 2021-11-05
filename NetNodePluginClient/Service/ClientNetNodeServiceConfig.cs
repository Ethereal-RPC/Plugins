using EtherealC.Client.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetNodePluginClient.Service
{
    public class ClientNetNodeServiceConfig : EtherealC.Service.WebSocket.WebSocketServiceConfig
    {
        private int netNodeHeartInterval;//心跳周期
        private List<string> netNodeIps;//集群地址

        public int NetNodeHeartInterval { get => netNodeHeartInterval; set => netNodeHeartInterval = value; }
        public List<string> NetNodeIps { get => netNodeIps; set => netNodeIps = value; }
    }
}
