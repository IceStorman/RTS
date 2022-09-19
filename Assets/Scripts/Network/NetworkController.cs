using Photon.Pun;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject connectionText;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        connectionText.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        connectionText.SetActive(false);
    }
}
