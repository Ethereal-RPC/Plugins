class NetNode:
    def __init__(self):
        self.Name = None
        self.Connects = 0
        self.HardwareInformation = None
        self.Prefixes = list()
        self.Requests = dict()
        self.Services = dict()
