using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Building), (byte)'B', Utils.Serialize, Utils.Deserialize);
        DataHandler.LoadGameData();
    }
}
