import threading

from EtherealC.Client.WebSocket.WebSocketClient import WebSocketClient
from EtherealC.Core.Model.AbstractTypes import AbstractTypes
from EtherealC.Core.Model.TrackException import TrackException, ExceptionCode
from EtherealC.Net.Abstract.Net import Net
from EtherealC.Net.WebSocket.WebSocketNetConfig import WebSocketNetConfig


class WebSocketNet(Net):
    def __init__(self, name):
        super().__init__(name=name)
        self.config = WebSocketNetConfig()
        self.connectSign = threading.Event()

    def Publish(self):
        def reactorStart():
            from twisted.internet import reactor
            if not reactor.running:
                reactor.suggestThreadPoolSize(10)
                reactor.run(False)

        threading.Thread(target=reactorStart).start()
        if self.config.netNodeMode is True:
            types = AbstractTypes()
            types.add(type=int, type_name="Int")
            from EtherealC.Net.NetNodeClient.Model.NetNode import NetNode
            types.add(type=type(NetNode()), type_name="NetNode")
            types.add(type=str, type_name="String")
            types.add(type=bool, type_name="Bool")
            from EtherealC.Service import ServiceCore
            from EtherealC.Net.NetNodeClient.Service.ClientNetNodeService import ClientNetNodeService
            netNodeService = ServiceCore.Register(service=ClientNetNodeService(name="ClientNetNodeService", types=types), net=self)
            from EtherealC.Request import RequestCore
            from EtherealC.Net.NetNodeClient.Request.ServerNetNodeRequest import ServerNetNodeRequest
            netNodeRequest = RequestCore.Register(net=self, request=ServerNetNodeRequest(name="ServerNetNodeService", types=types))

            def NetNodeSearchRunner():
                try:
                    from EtherealC.Net import NetCore
                    while NetCore.Get(self.name) is not None:
                        self.NetNodeSearch()
                        self.connectSign.wait(timeout=self.config.netNodeHeartbeatCycle/1000)
                        self.connectSign.clear()
                except Exception as exception:
                    self.OnException(
                            TrackException(exception=exception, message="NetNodeSearch循环报错", code=ExceptionCode.Runtime))

            threading.Thread(target=NetNodeSearchRunner).start()
        else:
            try:
                for request in self.requests.values():
                    request.client.Connect()
            except Exception as e:
                self.OnException(exception=TrackException(exception=e))
        return True

    def NetNodeSearch(self):
        flag = False
        for request in self.requests.values():
            if request.client is None and request.name != "ServerNetNodeService":
                flag = True
                break
        if flag is True:
            for item in self.config.netNodeIps:
                prefixes = item["prefixes"]
                config = item["config"]
                from EtherealC.Client import ClientCore
                client = ClientCore.Register(net=self, service_name="ServerNetNodeService",client=WebSocketClient(prefixes=prefixes))
                client.Connect(isSync=True)
                try:
                    if client.IsConnect():
                        from EtherealC.Request import RequestCore
                        from EtherealC.Net.NetNodeClient.Request.ServerNetNodeRequest import ServerNetNodeRequest
                        netNodeRequest: ServerNetNodeRequest = RequestCore.Get(net=self,
                                                                               service_name="ServerNetNodeService")
                        if netNodeRequest is None:
                            raise TrackException(code=ExceptionCode.Runtime, message="无法找到{0}-ServerNetNodeService"
                                                 .format(self.name))
                        for request in self.requests.values():
                            if request.client is not None:
                                continue
                            node = netNodeRequest.GetNetNode(request.name)
                            if node is None:
                                raise TrackException(code=ExceptionCode.Runtime,
                                                     message="{0}-{1}-在NetNode分布式中未找到节点"
                                                     .format(self.name, request.name))
                            requestClient = ClientCore.Register(net=self, service_name=request.name,
                                                                client=WebSocketClient(prefixes=prefixes))
                            requestClient.disconnect_event.register(self.ClientConnectFailEvent)
                            requestClient.Connect()
                        break
                finally:
                    ClientCore.UnRegister(net=self, service_name="ServerNetNodeService")

    def ClientConnectFailEvent(self, client):
        from EtherealC.Client import ClientCore
        ClientCore.UnRegister(net=self, service_name=client.name)
        self.connectSign.set()
