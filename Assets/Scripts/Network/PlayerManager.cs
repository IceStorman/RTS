using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject connectionText;

    private PlayerProfile playerProfile;

    private List<PlayerProfile> profiles;

    private List<GameObject> profilesDisplay;

    [SerializeField]
    private GameObject playerInfoDisplayPrefab;
    [SerializeField]
    private Transform playerPanel;

    private void Start()
    {
        connectionText.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        playerProfile = new PlayerProfile();
        profiles = new List<PlayerProfile>();
        profilesDisplay = new List<GameObject>();
    }

    public override void OnConnectedToMaster()
    {
        connectionText.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = otherPlayer.ActorNumber;
        if (index != -1)
        {
            Destroy(profilesDisplay[index]);
        }
        UpdatePlayersInfoUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayersInfoUI();
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayersInfoUI();
    }

    private void UpdatePlayersInfoUI()
    {
        foreach(GameObject profileDisplay in profilesDisplay)
        {
            Destroy(profileDisplay);
        }
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject display = Instantiate(playerInfoDisplayPrefab, playerPanel);
            display.transform.Find("NickName")
                .GetComponent<TextMeshProUGUI>().text = player.Value.NickName;
            profilesDisplay.Add(display);
        }
    }
}
