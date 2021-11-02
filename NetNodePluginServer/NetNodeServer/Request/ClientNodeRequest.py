from EtherealS.Request.WebSocket.WebSocketRequest import WebSocketRequest


class ClientNodeRequest(WebSocketRequest):
    def __init__(self, name, types):
        super().__init__()
        self.name = name
        self.types = types
