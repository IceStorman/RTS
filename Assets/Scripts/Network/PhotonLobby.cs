using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject lobby;
    [SerializeField]
    private GameObject room;
    [SerializeField]
    private GameObject startGameButton;

    private void Awake()
    {
        instance = this;
        SwitchRoomPanel(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        foreach(RoomInfo room in roomList)
        {
            ListRoom(room);
        }
    }

    private void RemoveRoomListings()
    {
        while(roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
    }

    private void ListRoom(RoomInfo room)
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();
        }
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
        SwitchRoomPanel(true);
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
        }
    }

    public void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            Debug.Log("Left");
        }
    }

    public override void OnJoinedRoom()
    {
        SwitchRoomPanel(true);
    }

    public override void OnLeftRoom()
    {
        SwitchRoomPanel(false);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    private void SwitchRoomPanel(bool roomBool)
    {
        room.SetActive(roomBool);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}
