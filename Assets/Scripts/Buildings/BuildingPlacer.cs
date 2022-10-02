using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private Structure _placedStructure = null;

    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    private void Start()
    {
        _PreparePlacedBuilding(0);
    }

    private void Update()
    {
        if(_placedStructure != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }
        }

        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(
            _ray,
            out _raycastHit,
            1000f,
            Globals.TERRAIN_LAYER_MASK
        ))
        {
            _placedStructure.SetPosition(_raycastHit.point);
            if (_lastPlacementPosition != _raycastHit.point)
            {
                _placedStructure.CheckValidPlacement();
            }
            _lastPlacementPosition = _raycastHit.point;
        }

        if (_placedStructure.HasValidPlacement && Input.GetMouseButtonDown(0))
        {
            _PlaceBuilding();
        }
    }

    private void _PreparePlacedBuilding(int buildingDataIndex)
    {
        if (_placedStructure != null && !_placedStructure.IsFixed)
        {
            Destroy(_placedStructure.Transform.gameObject);
        }

        Structure building = new Structure(
            Globals.BUILDING_DATA[buildingDataIndex]
        );

        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        _placedStructure = building;
        _lastPlacementPosition = Vector3.zero;
    }

    private void _PlaceBuilding()
    {
        _placedStructure.Place();
        _PreparePlacedBuilding(_placedStructure.DataIndex);
    }

    private void _CancelPlacedBuilding()
    {
        Destroy(_placedStructure.Transform.gameObject);
        _placedStructure = null;
    }
}
