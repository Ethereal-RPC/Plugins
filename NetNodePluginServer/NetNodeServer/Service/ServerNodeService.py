import random

from EtherealS.Net.NetNode.Model.NetNode import NetNode

from EtherealS.Server.Abstract.Token import Token
from EtherealS.Service.Decorator.ServiceMethod import ServiceMethod
from EtherealS.Service.WebSocket.WebSocketService import WebSocketService


class ServerNodeService(WebSocketService):
    def __init__(self, name, types):
        super().__init__()
        self.name = name
        self.types = types
        self.netNodes = dict()
        self.random = random.Random()

    @ServiceMethod()
    def Register(self, token: Token, node: NetNode) -> bool:
        token.key = "{0}-{1}".format(node.Name, node.Prefixes[0])
        value = self.netNodes.get(token.key, None)
        if value is not None:
            old_token: Token = value[0]
            old_token.disconnect_event.UnRegister(self.Sender_DisConnectEvent)
        self.netNodes[token.key] = (token, node)
        token.disconnect_event.Register(self.Sender_DisConnectEvent)
        print("{0}注册节点成功".format(token.key))
        self.printNetNodes()
        return True

    @ServiceMethod()
    def GetNetNode(self, token: Token, service_name: str) -> NetNode:
        nodes = list()
        for item in self.netNodes.values():
            node: NetNode = item[1]
            if node.Services.get(service_name, None) is not None:
                nodes.append(node)
        if nodes.__len__() > 0:
            return nodes[self.random.randint(0, nodes.__len__())]
        return None

    def Sender_DisConnectEvent(self, token):
        del self.netNodes[token.key]
        print("成功删除节点")
        self.printNetNodes()

    def printNetNodes(self):
        sb = "当前信息节点：\n"
        for item in self.netNodes.values():
            sb += item[0].key + "\n"
        print(sb)
