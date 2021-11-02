package com.ethereal.NetNode.Request;

import com.ethereal.client.Net.NetNode.Model.NetNode;
import com.ethereal.client.Request.Annotation.RequestMethod;
import com.ethereal.client.Request.WebSocket.WebSocketRequest;

public class ServerNetNodeRequest extends WebSocketRequest {
    @RequestMethod
    public NetNode GetNetNode(String servicename){
        return null;
    }
}
