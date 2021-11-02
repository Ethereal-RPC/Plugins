from EtherealC.Request.Decorator.Request import Request
from EtherealC.Request.WebSocket.WebSocketRequest import WebSocketRequest
from EtherealS.Net.NetNode.Model.NetNode import NetNode


class ServerNodeRequest(WebSocketRequest):
    def __init__(self):
        super().__init__()
        self.name = "ServerNetNodeService"
        self.types.add(type=int, type_name="Int")
        self.types.add(type=str, type_name="String")
        self.types.add(type=bool, type_name="Bool")
        self.types.add(type=type(NetNode()), type_name="NetNode")


    @Request()
    def Register(self, node: NetNode) -> bool:
        pass
