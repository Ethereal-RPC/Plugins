namespace NetNodePlugin.NetNodeClient.Request
{
    public interface IServerNodeRequest
    {
        /// <summary>
        /// 注册节点信息
        /// </summary>
        /// <param name="node">节点信息</param>
        [EtherealC.Request.Attribute.RequestMethod]
        public bool Register(Model.NetNode node);
    }
}
