using ExitGames.Client.Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameParameters gameParameters;
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Building), (byte)'B', Utils.Serialize, Utils.Deserialize);
        DataHandler.LoadGameData();
        GetComponent<DayAndNightCycler>().enabled = gameParameters.enableDayAndNightCycle;
    }
}
