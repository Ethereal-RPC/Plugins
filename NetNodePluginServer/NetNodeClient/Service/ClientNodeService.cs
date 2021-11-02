using EtherealC.Core.Model;
using NetNodePlugin.Model;
using NetNodePlugin.NetNodeClient.Request;

namespace NetNodePlugin.NetNodeClient.Service
{
    public class ClientNodeService:EtherealC.Service.WebSocket.WebSocketService
    {
        #region --字段--

        public ClientNodeService()
        {
            name = "ClientNetNodeService";
            types.Add<int>("Int");
            types.Add<long>("Long");
            types.Add<string>("String");
            types.Add<bool>("Bool");
            types.Add<NetNode>("NetNode");
        }


        #endregion

        #region --属性--

        #endregion

        #region --RPC方法--

        #endregion

        #region --普通方法--

        public override void Initialize()
        {

        }


        public override void UnInitialize()
        {

        }
        #endregion


    }
}
