using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile
{
    private string nickName;
    public Sprite avatar;

    public string NickName { get => nickName ?? "Player"; set => nickName = value; }
}
