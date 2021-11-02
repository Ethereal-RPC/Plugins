class NetNode:
    def __init__(self):
        self.Name = None
        self.Connects = 0
        self.HardwareInformation = None
        self.Prefixes = None
        self.requests = dict()
        self.service = dict()
