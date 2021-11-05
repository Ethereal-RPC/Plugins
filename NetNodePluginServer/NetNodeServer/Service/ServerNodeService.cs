using EtherealS.Core.Model;
using EtherealS.Net;
using EtherealS.Net.Abstract;
using EtherealS.Request;
using EtherealS.Server.Abstract;
using EtherealS.Service.WebSocket;
using NetNodePlugin.Model;
using NetNodePlugin.NetNodeClient.Request;
using NetNodePlugin.NetNodeClient.Service;
using NetNodePlugin.NetNodeServer.Request;
using NetNodePluginServer.NetNodeServer.Service;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NetNodePlugin.NetNodeServer.Service
{
    public class ServerNodeService:WebSocketService
    {
        private AutoResetEvent connectSign = new AutoResetEvent(false);
        public ServerNodeService()
        {
            name = "ServerNetNodeService";
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<NetNode>("NetNode");
        }
        #region --字段--
        /// <summary>
        /// 节点信息
        /// </summary>
        private Dictionary<string, Tuple<Token,NetNode>> netNodes = new ();
        /// <summary>
        /// 分布式请求
        /// </summary>
        private ClientNodeRequest distributeRequest;

        private Random random = new Random();
        /// <summary>
        /// 分布式IP组
        /// </summary>
        private List<string> netNodeIps;
        #endregion

        #region --属性--
        public ClientNodeRequest DistributeRequest { get => distributeRequest; set => distributeRequest = value; }
        public Dictionary<string, Tuple<Token, NetNode>> NetNodes { get => netNodes; set => netNodes = value; }
        #endregion

        #region --RPC方法--

        /// <summary>
        /// 注册节点
        /// </summary>
        /// <param name="token">Tooken</param>
        /// <param name="netNode">节点信息</param>
        /// <returns></returns>
        [EtherealS.Service.Attribute.ServiceMethod]
        public bool Register([EtherealS.Server.Attribute.Token] Token token, Model.NetNode netNode)
        {
            token.key = $"{netNode.Name}-{string.Join("::",netNode.Prefixes)}";
            //自建一份字典做缓存
            if(NetNodes.TryGetValue((string)token.key,out Tuple<Token, Model.NetNode> value))
            {
                value.Item1.DisConnectEvent -= Sender_DisConnectEvent;
                NetNodes.Remove((string)token.key);
            }
            NetNodes.Add((string)token.key, new (token,netNode));
            token.DisConnectEvent += Sender_DisConnectEvent;
            OnLog(TrackLog.LogCode.Runtime, $"{token.key}注册节点成功");
            PrintNetNode();
            return true;
        }


        /// <summary>
        /// 获取对应服务的网络节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [EtherealS.Service.Attribute.ServiceMethod]
        public NetNode GetNetNode([EtherealS.Server.Attribute.Token] Token sender, string serviceName)
        {
            //负载均衡的优化算法后期再写，现在采取随机分配
            List<Model.NetNode> nodes = new List<Model.NetNode>();
            foreach(Tuple<Token, Model.NetNode> tuple in NetNodes.Values)
            {
                if (tuple.Item2.Services.ContainsKey(serviceName))
                {
                    nodes.Add(tuple.Item2);
                }
            }
            if(nodes.Count > 0)
            {
                //成功返回对应节点
                return nodes[random.Next(0, nodes.Count)];
            }
            return null;
        }
        /// <summary>
        /// 获取对应服务的网络节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [EtherealS.Service.Attribute.ServiceMethod]
        public string GetName(string username)
        {
            return $"Hello {username}，I am " + name;
        }

        #endregion

        #region --普通方法--
        /// <summary>
        /// 如果断开连接，字典中删掉该节点
        /// </summary>
        /// <param name="token"></param>
        private void Sender_DisConnectEvent(Token token)
        {
            NetNodes.Remove((string)token.key);
            OnLog(TrackLog.LogCode.Runtime,$"成功删除节点{(token.key)}");
            PrintNetNode();
        }

        private void PrintNetNode()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tuple<Token, Model.NetNode> tuple in NetNodes.Values)
            {
                sb.AppendLine($"{tuple.Item2.Name}::{string.Join("&&", tuple.Item2.Prefixes)}");
            }
            OnLog(TrackLog.LogCode.Runtime, $"当前节点信息:\n{sb}");
        }

        public override void Initialize()
        {
            #region --Server--
            if (!NetCore.Get(netName, out Net net))
            {
                //向网关注册请求
                throw new TrackException($"{name}服务查询{netName}网关时未找到");
            }
            distributeRequest = RequestCore.Register<ClientNodeRequest, IClientNodeRequest>(net);
            #endregion

            #region --Client--
            Thread thread = new Thread(() =>
            {
                while (NetCore.Get(NetName, out Net temp))
                {
                    try
                    {
                        foreach (string prefixes in
                            (config as ServerNodeServiceConfig).NetNodeIps)
                        {
                            if (!EtherealC.Net.NetCore.Get($"NetNodeClient-{name}",
                                out EtherealC.Net.Abstract.Net net))
                            {
                                net = EtherealC.Net.NetCore.Register(new EtherealC.Net.WebSocket.WebSocketNet($"NetNodeClient-{name}"));
                                net.LogEvent += Net_LogEvent;
                                net.ExceptionEvent += Net_ExceptionEvent;
                            }

                            if (!EtherealC.Request.RequestCore.Get(net, $"ServerNetNodeService", out ServerNodeRequest serverNodeRequest))
                            {
                                //注册请求
                                serverNodeRequest = EtherealC.Request.RequestCore.Register<ServerNodeRequest, IServerNodeRequest>(net);
                            }

                            if (!EtherealC.Service.ServiceCore.Get(net, "ClientNetNodeService", out ClientNodeService clientNodeService))
                            {
                                //注册服务
                                clientNodeService = EtherealC.Service.ServiceCore.Register(net, new ClientNodeService());
                            }

                            if (serverNodeRequest.Client == null)
                            {
                                EtherealC.Client.Abstract.Client client =
                                    new EtherealC.Client.WebSocket.WebSocketClient(prefixes);
                                //注册连接
                                EtherealC.Client.ClientCore.Register(serverNodeRequest, client);
                                client.ConnectEvent += ClientConnectSuccessEvent;
                                client.ConnectFailEvent += ClientConnectFailEvent;
                                client.DisConnectEvent += ClientDisConnectEvent;
                                //部署
                                net.Publish();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        OnException(new TrackException(e));
                    }
                    finally
                    {
                        connectSign.WaitOne((config as ServerNodeServiceConfig).NetNodeHeartInterval);
                    }
                }
            });
            thread.Start();
            #endregion
            Console.WriteLine($"{name}服务已部署");
        }

        public override void UnInitialize()
        {
            Console.WriteLine($"{name}服务已注销");
        }


        private void Net_LogEvent(EtherealC.Core.Model.TrackLog log)
        {
            OnLog(TrackLog.LogCode.Runtime, "NetNodeClient\n" + log.Message);
        }
        private void Net_ExceptionEvent(EtherealC.Core.Model.TrackException exception)
        {
            OnException(new TrackException(exception));
        }

        private void ClientConnectSuccessEvent(EtherealC.Client.Abstract.Client client)
        {
            if (!NetCore.Get(netName, out Net net))
            {
                //向网关注册请求
                throw new TrackException($"{name}服务查询{netName}网关时未找到");
            }
            //注册节点信息
            if (EtherealC.Request.RequestCore.Get($"NetNodeClient-{client.Prefixes}", "ServerNetNodeService", out EtherealC.Request.Abstract.Request serverDistributeRequest))
            {
                //生成节点信息
                NetNode node = new NetNode();
                node.Prefixes = net.Server.Prefixes.ToArray();
                node.Name = $"{name}";
                node.HardwareInformation = new HardwareInformation();
                node.HardwareInformation.NetworkInterfaces = Utils.NetworkInterfaceHelper.GetAllNetworkInterface();
                node.HardwareInformation.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem.ToString();
                node.HardwareInformation.ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
                node.HardwareInformation.OSArchitecture = RuntimeInformation.OSArchitecture.ToString();
                node.HardwareInformation.OSDescription = RuntimeInformation.OSDescription.ToString();
                node.Services = new Dictionary<string, ServiceNode>();
                node.Requests = new Dictionary<string, RequestNode>();
                //添加服务信息
                foreach (EtherealS.Service.Abstract.Service service in net.Services.Values)
                {
                    ServiceNode serviceNode = new ServiceNode();
                    serviceNode.Name = service.Name;
                    node.Services.Add(serviceNode.Name, serviceNode);
                }
                //添加请求信息    
                foreach (EtherealS.Request.Abstract.Request request in net.Requests.Values)
                {
                    RequestNode requestNode = new RequestNode();
                    requestNode.Name = request.Name;
                    node.Requests.Add(requestNode.Name, requestNode);
                }
                //向目标主机注册节点信息
                ((IServerNodeRequest)serverDistributeRequest).Register(node);
            }
            else throw new TrackException(TrackException.ErrorCode.Runtime, $"EtherealC中未找到 NetNodeClient-{client.Prefixes}-ServerNodeService");
        }

        private void ClientConnectFailEvent(EtherealC.Client.Abstract.Client client)
        {
            EtherealC.Client.ClientCore.UnRegister(client.NetName, client.ServiceName);
        }
        private void ClientDisConnectEvent(EtherealC.Client.Abstract.Client client)
        {
            EtherealC.Client.ClientCore.UnRegister(client.NetName, client.ServiceName);
            connectSign.Set();
        }
        #endregion

    }
}
