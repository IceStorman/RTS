using UnityEngine;

public static class DataHandler
{
    public static void LoadGameData()
    {
        Globals.BUILDING_DATA = Resources.LoadAll<BuildingData>("ScriptableObjects/Entities/Buildings");
    }
}
