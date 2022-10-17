using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private UIManager _uiManager;

    private Structure _placedStructure = null;

    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    public void SelectPlacedBuilding(int buildingIndex)
    {
        _PreparePlacedBuilding(buildingIndex);
    }

    private void Awake()
    {
        _uiManager = GetComponent<UIManager>();
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

        if (_placedStructure != null &&
            Physics.Raycast(
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

        if (_placedStructure != null && 
            _placedStructure.HasValidPlacement &&
            Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
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
        if (_placedStructure.CanBuy())
            _PreparePlacedBuilding(_placedStructure.DataIndex);
        else
            _placedStructure = null;
        _uiManager.UpdateResourceTexts();
        _uiManager.CheckBuildingButtons();
    }

    private void _CancelPlacedBuilding()
    {
        Destroy(_placedStructure.Transform.gameObject);
        _placedStructure = null;
    }
}
