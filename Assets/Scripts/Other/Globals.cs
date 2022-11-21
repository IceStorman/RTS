using System.Collections.Generic;
using Photon.Pun;

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;

    public static List<EntityManager> SELECTED_UNITS = new List<EntityManager>();

    public static BuildingData[] BUILDING_DATA;

    public static Dictionary<string, GameResource> GAME_RESOURCES = 
        new Dictionary<string, GameResource>()
        {
            {"gold", new GameResource("Gold", 800) },
            {"wood", new GameResource("Wood", 800) },
            {"stone", new GameResource("Stone", 800) }
        };
}
