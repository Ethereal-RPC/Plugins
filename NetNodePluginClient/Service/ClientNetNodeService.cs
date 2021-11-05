using EtherealC.Client;
using EtherealC.Client.Abstract;
using EtherealC.Client.WebSocket;
using EtherealC.Core.Model;
using EtherealC.Net;
using EtherealC.Net.Abstract;
using EtherealC.Request;
using EtherealC.Service.WebSocket;
using NetNodePluginClient.Model;
using NetNodePluginClient.Request;
using System;
using System.Threading;

namespace NetNodePluginClient.Service
{
    public class ClientNetNodeService : WebSocketService
    {
        private AutoResetEvent connectSign = new AutoResetEvent(false);

        #region --属性--

        #endregion
        public ClientNetNodeService()
        {
            name = "ClientNetNodeService";
            config = new ClientNetNodeServiceConfig();
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<NetNode>("NetNode");
        }
        public override void Initialize()
        {
            if (NetCore.Get(netName, out Net net))
            {
                //向网关注册请求
                ServerNetNodeRequest netNodeRequest =
                    RequestCore.Register<ServerNetNodeRequest, IServerNetNodeRequest>(net, "ServerNetNodeService",
                        types);
            }
            else throw new TrackException($"{name}服务查询{netName}网关时未找到");
            new Thread(() =>
            {
                while (NetCore.Get(name, out Net net))
                {
                    NetNodeSearch();
                    connectSign.WaitOne((Config as ClientNetNodeServiceConfig).NetNodeHeartInterval);
                }
            }).Start();
            Console.WriteLine($"{name}已载入");
        }

        public override void UnInitialize()
        {
            Console.WriteLine($"{name}已注销");
        }
        public async void NetNodeSearch()
        {
            if (!NetCore.Get(netName, out Net net))
            {
                throw new TrackException($"{name}服务查询{netName}网关时未找到");
            }
            bool flag = false;
            foreach (EtherealC.Request.Abstract.Request request in net.Requests.Values)
            {
                if (request.Client == null && request.Name != "ServerNetNodeService")
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                //搜寻正常启动的注册中心
                foreach (string prefixes in (Config as ClientNetNodeServiceConfig).NetNodeIps)
                {
                    //向网关注册连接
                    WebSocketClient client = (WebSocketClient)ClientCore.Register(net, "ServerNetNodeService", new WebSocketClient(prefixes));
                    try
                    {
                        await client.ConnectSync();
                        //连接成功
                        if (client?.Accept?.State == System.Net.WebSockets.WebSocketState.Open)
                        {
                            if (RequestCore.Get(net, "ServerNetNodeService",
                                out EtherealC.Request.Abstract.Request netNodeRequest))
                            {
                                foreach (EtherealC.Request.Abstract.Request request in net.Requests.Values)
                                {
                                    if (request.Client == null)
                                    {
                                        //获取服务节点
                                        NetNode node = (netNodeRequest as ServerNetNodeRequest).GetNetNode(request.Name);
                                        if (node != null)
                                        {
                                            //注册连接并启动连接
                                            Client requestClient = ClientCore.Register(request, new WebSocketClient(node.Prefixes[0]));
                                            requestClient.ConnectFailEvent += ClientConnectFailEvent;
                                            requestClient.DisConnectEvent += ClientDisConnectEvent; ;
                                            requestClient.Connect();
                                        }
                                        else
                                            throw new TrackException(TrackException.ErrorCode.Runtime,
                                                $"{name}-{request.Name}-在NetNode分布式中未找到节点");
                                    }
                                }
                                return;
                            }
                            throw new TrackException(TrackException.ErrorCode.Runtime, $"无法找到{name}-ServerNetNodeService");
                        }
                    }
                    finally
                    {
                        ClientCore.UnRegister(net, "ServerNetNodeService");
                    }
                }
            }
        }

        private void ClientDisConnectEvent(Client client)
        {
            ClientCore.UnRegister(client.NetName, client.ServiceName);
            connectSign.Set();
        }

        private void ClientConnectFailEvent(Client client)
        {
            ClientCore.UnRegister(client.NetName, client.ServiceName);
        }
    }
}
