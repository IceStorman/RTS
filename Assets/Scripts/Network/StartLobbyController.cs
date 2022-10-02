using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject newLobbyPanel;

    [SerializeField]
    private int roomSize;

    private void Start()
    {
        newLobbyPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room now...");

        int randomRoomNumber = GenerateRoomNumber();

        RoomOptions roomOps = CreateRoomOptions();  

        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        CreateRoomName(randomRoomNumber);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreatedRoom()
    {
        newLobbyPanel.SetActive(true);
    }

    private int GenerateRoomNumber()
        => Random.Range(0, 10000);

    private RoomOptions CreateRoomOptions()
        => new RoomOptions() { IsVisible = true , IsOpen = true, MaxPlayers = (byte)roomSize};

    private void CreateRoomName(int randomRoomName)
        => PhotonNetwork.CurrentLobby.Name = "Room" + randomRoomName;

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to create a room");
        CreateRoom();
    }
}
