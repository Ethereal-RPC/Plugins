using System;
using System.Collections.Generic;
using System.Text;
using EtherealS.Core.Model;
using NetNodePlugin.NetNodeServer.Request;
using EtherealS.Service.WebSocket;
using EtherealS.Server.Abstract;
using NetNodePlugin.Model;

namespace NetNodePlugin.NetNodeServer.Service
{
    public class ServerNodeService:WebSocketService
    {
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
        private Dictionary<string, Tuple<Token,Model.NetNode>> netNodes = new ();
        /// <summary>
        /// 分布式请求
        /// </summary>
        private ClientNodeRequest distributeRequest;

        private Random random = new Random();
        /// <summary>
        /// 分布式IP组
        /// </summary>
        private List<Tuple<string, EtherealC.Client.Abstract.ClientConfig>> netNodeIps;

        #endregion

        #region --属性--

        public ClientNodeRequest DistributeRequest { get => distributeRequest; set => distributeRequest = value; }
        public Dictionary<string, Tuple<Token, Model.NetNode>> NetNodes { get => netNodes; set => netNodes = value; }

        public List<Tuple<string, EtherealC.Client.Abstract.ClientConfig>> NetNodeIps { get => netNodeIps; set => netNodeIps = value; }
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
            Console.WriteLine($"{name}服务已部署");
        }

        public override void UnInitialize()
        {
            Console.WriteLine($"{name}服务已注销");
        }
        #endregion

    }
}
