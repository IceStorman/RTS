using Photon.Pun;
using UnityEngine;

public class JoinLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject lobbyPanel;

    private void Start()
    {
        lobbyPanel.SetActive(false);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Join failed");
    }
}
