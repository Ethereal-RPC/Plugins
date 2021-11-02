from EtherealC.Service.WebSocket.WebSocketService import WebSocketService

from EtherealS.Net.NetNode.Model.NetNode import NetNode


class ClientNodeService(WebSocketService):
    def __init__(self):
        super().__init__()
        self.name = "ClientNetNodeService"
        self.types.add(type=int, type_name="Int")
        self.types.add(type=str, type_name="String")
        self.types.add(type=bool, type_name="Bool")
        self.types.add(type=type(NetNode()), type_name="NetNode")
