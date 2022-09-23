using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView playerPhotonView;

    [SerializeField]
    private int multiplayerSceneIndex;

    private int roomSize;
    private int countOfPlayers;
    [SerializeField]
    private int minPlayersToStart;

    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private TextMeshProUGUI countOfPlayersDisplay;
    [SerializeField]
    private TextMeshProUGUI maxPlayersDisplay;
    [SerializeField]
    private TextMeshProUGUI timerToStartDisplay;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullGameWaitTime;

    private void Start()
    {
        playerPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        countOfPlayers = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        countOfPlayersDisplay.text = countOfPlayers.ToString();
        maxPlayersDisplay.text = roomSize.ToString();

        if (countOfPlayers == roomSize)
            readyToStart = true;
        else if (countOfPlayers >= minPlayersToStart)
            readyToCountDown = true;
        else
        {
            readyToCountDown = false;
            readyToStart = false;
        }  
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

        if (PhotonNetwork.IsMasterClient)
            playerPhotonView.RPC("RPS_SendTimer", RpcTarget.Others, timerToStartGame);
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    private void Update()
    {
        WaitingForMorePlayers();
    }

    private void WaitingForMorePlayers()
    {
        if(countOfPlayers <= 1)
        {
            ResetTimer();
        }

        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = tempTimer;

        if(timerToStartGame <= 0f)
        {
            if (startingGame)
                return;
            StartGame();
        }
    }

    private void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void StartGame()
    {
        startingGame = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    public void CancelGame()
    {
        PhotonNetwork.LeaveRoom();
        lobbyPanel.SetActive(false);
    }
}
