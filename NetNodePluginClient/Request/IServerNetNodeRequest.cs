using NetNodePluginClient.Model;

namespace NetNodePluginClient.Request
{
    public interface IServerNetNodeRequest
    {
        [EtherealC.Request.Attribute.RequestMethod]
        public NetNode GetNetNode(string servicename);


    }
}
