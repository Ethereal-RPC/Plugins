using EtherealS.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetNodePluginServer.NetNodeServer.Service
{
    public class ServerNodeServiceConfig : ServiceConfig
    {
        private int netNodeHeartInterval;//心跳周期
        private List<string> netNodeIps;//集群地址

        public int NetNodeHeartInterval { get => netNodeHeartInterval; set => netNodeHeartInterval = value; }
        public List<string> NetNodeIps { get => netNodeIps; set => netNodeIps = value; }
    }
}
