using Photon.Pun;
using UnityEngine;

public class StartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private GameObject lobbyPanel;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joining to the room...");
        lobbyPanel.SetActive(true);
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting game...");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}
