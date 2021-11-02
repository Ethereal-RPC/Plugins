from abc import ABC

from EtherealS.Service.Abstract.Service import Service


class NetConfig(ABC):

    def __init__(self):
        self.netNodeMode = False
        self.netNodeIps = None
        self.netNodeHeartbeatCycle = 20000

