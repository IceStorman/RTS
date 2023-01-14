using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class RPCInvoker : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        photonView.GetComponent<PhotonView>();
    }
    
    public void Invoke(string methodName, float x, float y, float z)
    {
        photonView.RPC(methodName, RpcTarget.AllBuffered, x, y, z);
    }

    public void Invoke(string methodName, int buildingIndex, float x, float y, float z)
    {
        photonView.RPC(methodName, RpcTarget.AllBuffered, buildingIndex, x, y, z);
    }
}