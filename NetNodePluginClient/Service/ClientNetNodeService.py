from EtherealC.Service.WebSocket.WebSocketService import WebSocketService


class ClientNetNodeService(WebSocketService):
    def __init__(self, name, types):
        super().__init__()
        self.name = name
        self.types = types
