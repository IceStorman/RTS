using UnityEngine;
using Photon.Pun;

public class QuitMechanic : MonoBehaviour
{
    public void QuitTheGame()
    {
        Application.Quit();
    }

    public void BackToMenuPanel(GameObject panel)
    {
        panel.SetActive(false);
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
    }
}
