using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class BuildingPlacer : MonoBehaviourPunCallbacks
{
    private Building placedBuilding;

    private Ray ray;
    private RaycastHit raycastHit;
    private Vector3 lastPlacementPosition;

    public void SelectPlacedBuilding(int buildingIndex)
    {
        PreparePlacedBuilding(buildingIndex);
    }

    private void Awake()
    {
        photonView.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(placedBuilding != null && Input.GetKeyUp(KeyCode.Escape))
        {
            CancelPlacedBuilding();
            return;
        }

        TrySetBuildingPosition();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceBuilding();
        }
    }

    private void TryPlaceBuilding()
    {
        if (CanPlaceBuilding())
        {
            PlaceBuilding();
        }
    }
    
    private void TrySetBuildingPosition()
    {
        if (Camera.main != null) ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (placedBuilding == null ||
            !Physics.Raycast(ray, out raycastHit, 1000f, Globals.TERRAIN_LAYER_MASK)) return;
        
        placedBuilding.SetPosition(raycastHit.point);
        if (lastPlacementPosition != raycastHit.point)
        {
            placedBuilding.CheckValidPlacement();
        }
        lastPlacementPosition = raycastHit.point;
    }
    
    private bool CanPlaceBuilding()
        => placedBuilding is { HasValidPlacement: true } &&
           !EventSystem.current.IsPointerOverGameObject();
    
    private void PreparePlacedBuilding(int buildingDataIndex)
    {
        if (placedBuilding is {IsFixed: false})
        {
            Destroy(placedBuilding.Transform.gameObject);
        }

        photonView.RPC("RPC_CreateBuildingPrefab", RpcTarget.AllBuffered, buildingDataIndex);
        lastPlacementPosition = Vector3.zero;
    }
    
    private void PlaceBuilding()
    {
        photonView.RPC("RPC_PlaceBuilding", RpcTarget.AllBuffered);
        
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
        
        TryContinuePlacing();
        
        Globals.UpdateNavMeshSurface();
        
        Debug.Log("Completed");
    }

    private void TryContinuePlacing()
    {
        if (placedBuilding.CanBuy())
        {
            PreparePlacedBuilding(placedBuilding.DataIndex);
        }
        else
        {
            EventManager.TriggerEvent("PlaceBuildingOff");
            placedBuilding = null;
        }
    }
    
    [PunRPC]
    private void RPC_CreateBuildingPrefab(int buildingDataIndex)
    {
        var building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex]
        );

        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        placedBuilding = building;
    }
    
    [PunRPC]
    private void RPC_PlaceBuilding()
        => placedBuilding.Place();

    private void CancelPlacedBuilding()
    {
        Destroy(placedBuilding.Transform.gameObject);
        placedBuilding = null;
    }
}
