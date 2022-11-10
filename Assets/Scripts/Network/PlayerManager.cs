using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject connectionText;

    private PlayerProfile player;

    private void Start()
    {
        connectionText.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectionText.SetActive(false);
        player = new PlayerProfile();
    }

    private void SetNickName(string newNickName)
    {
        player.NickName = newNickName;
    }

    private void SetAvatar(Sprite newAvatar)
    {
        player.avatar = newAvatar;
    }
}
