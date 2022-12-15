using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Vector3 startPosition;
    
    public GameParameters gameParameters;

    private Ray ray;
    private RaycastHit raycastHit;
    
    private void Awake()
    {
        Instance = this;
        DataHandler.LoadGameData();
        GetComponent<DayAndNightCycler>().enabled = gameParameters.enableDayAndNightCycle;
        Globals.NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
        Globals.UpdateNavMeshSurface();
        GetStartPosition();
    }

    private void Update()
    {
        CheckUnitsNavigation();
    }
    
    private void GetStartPosition()
    {
        startPosition = Utils.MiddleOfScreenPointToWorld();
    }
    
    private void CheckUnitsNavigation()
    {
        if (Globals.SELECTED_UNITS.Count <= 0 || !Input.GetMouseButtonUp(1)) return;
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(
                ray,
                out raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            )) return;

        foreach (var entityManager in Globals.SELECTED_UNITS)
        {
            if (entityManager.GetType() == typeof(CharacterManager))
            {
                ((CharacterManager)entityManager).MoveTo(raycastHit.point);
            }
        }
    }
}
