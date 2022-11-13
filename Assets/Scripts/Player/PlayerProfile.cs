using Photon.Realtime;
using UnityEngine;
using TMPro;

public class PlayerProfile
{
    public Player Player { get; private set; }

    public void SetPlayerInfo(Player player)
    {
        this.Player = player;
    }
}
