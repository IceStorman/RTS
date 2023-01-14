using System.Collections.Generic;
using UnityEngine.AI;

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;

    public static NavMeshSurface NAV_MESH_SURFACE;
    
    public static List<Entity> SUMMONED_ENTITIES = new();

    public static List<EntityManager> SELECTED_UNITS = new();

    public static BuildingData[] BUILDING_DATA;

    public static Dictionary<string, GameResource> GAME_RESOURCES = 
        new Dictionary<string, GameResource>()
        {
            {"gold", new GameResource("Gold", 800) },
            {"wood", new GameResource("Wood", 800) },
            {"stone", new GameResource("Stone", 800) }
        };

    public static void AddEntity(Entity entity)
    {
        SUMMONED_ENTITIES.Add(entity);
    }
    
    public static void UpdateNavMeshSurface()
    {
        NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
    }
}
