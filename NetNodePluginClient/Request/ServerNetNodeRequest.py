from EtherealC.Net.NetNodeClient.Model.NetNode import NetNode
from EtherealC.Request.Decorator.Request import Request
from EtherealC.Request.WebSocket.WebSocketRequest import WebSocketRequest


class ServerNetNodeRequest(WebSocketRequest):
    def __init__(self, name, types):
        super().__init__()
        self.name = name
        self.types = types

    @Request()
    def GetNetNode(self, servicename: str) -> NetNode:
        pass
