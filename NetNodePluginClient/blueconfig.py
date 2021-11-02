from abc import ABC


class NetConfig(ABC):

    def __init__(self):
        self.netNodeMode = False
        self.netNodeIps = None
        self.netNodeHeartbeatCycle = 60000
        self.maxThreadCount = 5

