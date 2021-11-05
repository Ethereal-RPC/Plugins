using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetNodePluginClient
{
    public class blue
    {
        /*
        void b()
        {
            //注册数据类型
            AbstractTypes types = new AbstractTypes();
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<NetNode>("NetNode");
            //向网关注册服务
            Service.Abstract.Service netNodeService = ServiceCore.Register(this, new ClientNetNodeService(),
                "ClientNetNodeService", types);
            //向网关注册请求
            ServerNetNodeRequest netNodeRequest =
                RequestCore.Register<ServerNetNodeRequest, IServerNetNodeRequest>(this, "ServerNetNodeService",
                    types);
            new Thread(() =>
            {
                try
                {
                    while (NetCore.Get(name, out Abstract.Net net))
                    {
                        NetNodeSearch();
                        connectSign.WaitOne(Config.NetNodeHeartInterval);
                    }
                }
                catch (Exception e)
                {
                    OnException(new TrackException(e));
                }
            }).Start();
        }
        /// <summary>
        /// 分布式模式Demo
        /// </summary>
        /// <param name="netName">网关名</param>
        /// <param name="ip">本地集群地址</param>
        public static void NetNode(string netName,string ip)
        {
            //注册数据类型
            AbstractTypes types = new AbstractTypes();
            types.Add<int>("Int");
            types.Add<User>("User");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            //建立网关
            Net net = NetCore.Register(new WebSocketNet(netName));
            net.ExceptionEvent += Config_ExceptionEvent;
            //向网关注册服务
            Service service = ServiceCore.Register(net, new ClientService(), "Client", types);
            //向网关注册请求
            Request request = RequestCore.Register<ServerRequest,IServerRequest>(net, "Server", types);
        //开启分布式模式
        net.Config.NetNodeMode = true;
            //添加分布式地址
            List<Tuple<string, ClientConfig>> ips = new();
        ips.Add(new Tuple<string, ClientConfig>($"ethereal://{ip}:{28015}/NetDemo/", new WebSocketClientConfig()));
            ips.Add(new Tuple<string, ClientConfig>($"ethereal://{ip}:{28016}/NetDemo/", new WebSocketClientConfig()));
            ips.Add(new Tuple<string, ClientConfig>($"ethereal://{ip}:{28017}/NetDemo/", new WebSocketClientConfig()));
            ips.Add(new Tuple<string, ClientConfig>($"ethereal://{ip}:{28018}/NetDemo/", new WebSocketClientConfig()));
            net.Config.NetNodeIps = ips;
            request.ConnectSuccessEvent += Request_ConnectSuccessEvent;
            net.Publish();
            Console.Read();
        }
        public async void NetNodeSearch()
        {
            bool flag = false;
            foreach (Request.Abstract.Request request in Requests.Values)
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
                foreach (Tuple<string, ClientConfig> item in Config.NetNodeIps)
                {
                    string prefixes = item.Item1;
                    ClientConfig config = item.Item2;
                    //向网关注册连接
                    WebSocketClient client = (WebSocketClient)ClientCore.Register(this, "ServerNetNodeService", new WebSocketClient(prefixes));
                    try
                    {
                        await client.ConnectSync();
                        //连接成功
                        if (client?.Accept?.State == System.Net.WebSockets.WebSocketState.Open)
                        {
                            if (RequestCore.Get(this, "ServerNetNodeService",
                                out Request.Abstract.Request netNodeRequest))
                            {
                                foreach (Request.Abstract.Request request in Requests.Values)
                                {
                                    if (request.Client == null)
                                    {
                                        //获取服务节点
                                        NetNode node = (netNodeRequest as ServerNetNodeRequest).GetNetNode(request.Name);
                                        if (node != null)
                                        {
                                            //注册连接并启动连接
                                            Client.Abstract.Client requestClient = ClientCore.Register(request, new WebSocketClient(node.Prefixes[0]));
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
                        ClientCore.UnRegister(this, "ServerNetNodeService");
                    }
                }
            }
        }

        private void ClientDisConnectEvent(Client.Abstract.Client client)
        {
            ClientCore.UnRegister(client.NetName, client.ServiceName);
            connectSign.Set();
        }

        private void ClientConnectFailEvent(Client.Abstract.Client client)
        {
            ClientCore.UnRegister(client.NetName, client.ServiceName);
        }
        */
    }
}
