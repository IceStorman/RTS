using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject newLobbyPanel;

    [SerializeField]
    private int roomSize;

    public void Start()
    {
        newLobbyPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void QuickStart()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to join a room");
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating room now...");
        int randomRoomNumber = GenerateRoomNumber();

        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };

        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
    }

    private int GenerateRoomNumber()
        => Random.Range(0, 10000);

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to create a room");
        CreateRoom();
    }

    public void CancelGame()
    {
        PhotonNetwork.LeaveRoom();
    }
}
