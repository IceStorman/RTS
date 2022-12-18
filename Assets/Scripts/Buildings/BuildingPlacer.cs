using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class BuildingPlacer : MonoBehaviourPunCallbacks
{
    private Building buildingSketch;

    private Ray ray;
    private RaycastHit raycastHit;
    private Vector3 lastPlacementPosition;

    private void Awake()
    {
        photonView.GetComponent<PhotonView>();
    }

    private void Start()
    {
        buildingSketch = new Building(Globals.BUILDING_DATA[0]);
        buildingSketch.Transform.GetComponent<BuildingManager>().Initialize(buildingSketch);
        
        buildingSketch.SetPosition(GameManager.Instance.startPosition);
        lastPlacementPosition = buildingSketch.Transform.position;
        
        PlaceBuilding();
        
        CancelPlacedBuilding();
    }

    private void Update()
    {
        if(buildingSketch != null && Input.GetKeyUp(KeyCode.Escape))
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
    
    public void SelectPlacedBuilding(int buildingIndex)
    {
        PreparePlacedBuilding(buildingIndex);
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
        if (Camera.main != null) 
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (buildingSketch == null ||
            !Physics.Raycast(ray, out raycastHit, 1000f, Globals.TERRAIN_LAYER_MASK)) return;
        
        buildingSketch.SetPosition(raycastHit.point);
        if (lastPlacementPosition != raycastHit.point)
        {
            buildingSketch.CheckValidPlacement();
        }
        lastPlacementPosition = raycastHit.point;
    }
    
    private bool CanPlaceBuilding()
        => buildingSketch is { HasValidPlacement: true } &&
           !EventSystem.current.IsPointerOverGameObject();
    
    private void PreparePlacedBuilding(int buildingDataIndex)
    {
        if (buildingSketch is {IsFixed: false})
        {
            Destroy(buildingSketch.Transform.gameObject);
        }

        Building building = new (Globals.BUILDING_DATA[buildingDataIndex]);

        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        buildingSketch = building;
        lastPlacementPosition = Vector3.zero;
    }
    
    private void PlaceBuilding()
    {
        var buildingDataIndex = buildingSketch.DataIndex;
        var x = lastPlacementPosition.x;
        var y = lastPlacementPosition.y;
        var z = lastPlacementPosition.z;

        photonView.RPC("RPC_PlaceBuilding", RpcTarget.AllBuffered, 
            buildingDataIndex, x, y, z);
        TakeResources();

        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
        
        TryContinuePlacing();
    }

    private void TryContinuePlacing()
    {
        if (buildingSketch.CanBuy())
        {
            PreparePlacedBuilding(buildingSketch.DataIndex);
        }
        else
        {
            CancelPlacedBuilding();
        }
    }

    [PunRPC]
    private void RPC_PlaceBuilding(int buildingDataIndex, float x, float y, float z)
    {
        Building building = new (Globals.BUILDING_DATA[buildingDataIndex]);
        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        
        Vector3 position = new (x, y, z);
        building.SetPosition(position);

        building.Place();

        Globals.UpdateNavMeshSurface();
    }

    private void TakeResources()
    {
        foreach (ResourceValue resource in buildingSketch.Data.cost)
        {
            Globals.GAME_RESOURCES[resource.code].AddAmount(-resource.amount);
        }
    }

    private void CancelPlacedBuilding()
    {
        Destroy(buildingSketch.Transform.gameObject);
        buildingSketch = null;
    }
}
