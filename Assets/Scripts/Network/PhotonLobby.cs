using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotonLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static PhotonLobby instance;

    private string roomName;
    private int roomSize;

    [SerializeField]
    private GameObject roomListingPrefab;
    [SerializeField]
    private Transform roomsPanel;

    [SerializeField]
    private GameObject createLobbyPanel;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject startGameButton;

    [SerializeField]
    private TextMeshProUGUI roomNameDisplay;
    [SerializeField]
    private TextMeshProUGUI countOfPlayersDisplay;
    [SerializeField]
    private TextMeshProUGUI maxPlayersDisplay;

    private void Awake()
    {
        instance = this;
        SwitchPanel(createLobbyPanel, false);
        SwitchPanel(lobbyPanel, false);
        SwitchPanel(roomPanel, false);
        SwitchStartGameButton();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RemoveRoomListings();
        foreach(RoomInfo room in roomList)
        {
            if(room.IsVisible && room.IsOpen)
                ListRoom(room);
        }
    }

    private void RemoveRoomListings()
    {
        for(int i = 0; i < roomsPanel.childCount; i++)
        {
            Destroy(roomsPanel.GetChild(i).gameObject);
        }
    }

    private void ListRoom(RoomInfo room)
    {
        GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
        RoomButton tempButton = tempListing.GetComponent<RoomButton>();

        if (!room.IsOpen || !room.IsVisible) return;
        tempButton.roomName = room.Name;
        tempButton.roomSize = room.MaxPlayers;
        tempButton.SetRoom();
    }

    public void OpenCreateRoomPanelOnClick()
    {
         SwitchPanel(createLobbyPanel, true);
    }

    public void CreateRoomOnClick()
    {
        if (CanCreate())
            CreateRoom();
    }

    private bool CanCreate()
        => roomName != "" && roomSize >= 1;

    private void CreateRoom()
    {
        RoomOptions roomOptions = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        SwitchPanel(roomPanel, true);
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            SwitchPanel(lobbyPanel, true);
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        SwitchPanel(roomPanel, true);
        UpdateRoomInfo();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo();
    }

    public override void OnLeftRoom()
    {
        SwitchPanel(lobbyPanel, false);
        SwitchPanel(createLobbyPanel, false);
        SwitchPanel(roomPanel, false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo();
    }

    private void UpdateRoomInfo()
    {
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        countOfPlayersDisplay.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        maxPlayersDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        SwitchStartGameButton();
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        if(!PhotonNetwork.CurrentRoom.IsOpen && !PhotonNetwork.CurrentRoom.IsVisible)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    private void SwitchPanel(GameObject panel, bool isActive)
    {
        panel.SetActive(isActive);
    }

    private void SwitchStartGameButton()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}
