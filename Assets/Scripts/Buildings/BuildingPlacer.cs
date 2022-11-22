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
        _PreparePlacedBuilding(buildingIndex);
    }

    private void Awake()
    {
        photonView.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(placedBuilding != null && Input.GetKeyUp(KeyCode.Escape))
        {
            _CancelPlacedBuilding();
            return;
        }

        TrySetBuildingPosition();

        TryPlaceBuilding();
    }

    private void TryPlaceBuilding()
    {
        if (CanPlaceBuilding())
        {
            _PlaceBuilding();
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
           Input.GetMouseButtonDown(0) &&
           !EventSystem.current.IsPointerOverGameObject();
    
    private void _PreparePlacedBuilding(int buildingDataIndex)
    {
        if (placedBuilding is {IsFixed: false})
        {
            Destroy(placedBuilding.Transform.gameObject);
        }

        Building building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex]
        );

        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        placedBuilding = building;
        lastPlacementPosition = Vector3.zero;
    }
    
    private void _PlaceBuilding()
    {
        //Utils.Serialize(placedBuilding);
        placedBuilding.Place();
        //photonView.RPC("RPC_PlaceBuilding", RpcTarget.AllBuffered, placedBuilding);
        
        if (placedBuilding.CanBuy())
            _PreparePlacedBuilding(placedBuilding.DataIndex);
        else
            placedBuilding = null;
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
        Debug.Log("Completed");
    }

    [PunRPC]
    private void RPC_PlaceBuilding(Building building)
        => building.Place();

    private void _CancelPlacedBuilding()
    {
        Destroy(placedBuilding.Transform.gameObject);
        placedBuilding = null;
    }
}
