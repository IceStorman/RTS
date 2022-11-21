using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntitiesSelection : MonoBehaviour
{
    public UIManager uiManager;

    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;

    private Ray _ray;
    private RaycastHit _raycastHit;

    private Dictionary<int, List<EntityManager>> _selectionGroups = new Dictionary<int, List<EntityManager>>();

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            _isDraggingMouseBox = false;

        if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
            _SelectUnitsInDraggingBox();

        if (Globals.SELECTED_UNITS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _DeselectAllUnits();
            if (Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(
                    _ray,
                    out _raycastHit,
                    1000f
                ))
                {
                    if (_raycastHit.transform.tag == "Terrain")
                        _DeselectAllUnits();
                }
            }
        }

        if (!Input.anyKeyDown) return;
        int alphaKey = Utils.GetAlphaKeyValue(Input.inputString);
        if (alphaKey == -1) return;
        if (
            Input.GetKey(KeyCode.X) ||
            Input.GetKey(KeyCode.RightControl) ||
            Input.GetKey(KeyCode.LeftApple) ||
            Input.GetKey(KeyCode.RightApple)
        )
        {
            _CreateSelectionGroup(alphaKey);
        }
        else
            _ReselectGroup(alphaKey);
    }

    public void SelectUnitsGroup(int groupIndex)
    {
        _ReselectGroup(groupIndex);
    }

    private void _CreateSelectionGroup(int groupIndex)
    {
        if (Globals.SELECTED_UNITS.Count == 0)
        {
            if (_selectionGroups.ContainsKey(groupIndex))
                _RemoveSelectionGroup(groupIndex);
            return;
        }
        List<EntityManager> groupUnits = new(Globals.SELECTED_UNITS);
        _selectionGroups[groupIndex] = groupUnits;
        uiManager.ToggleSelectionGroupButton(groupIndex, true);
    }

    private void _RemoveSelectionGroup(int groupIndex)
    {
        _selectionGroups.Remove(groupIndex);
        uiManager.ToggleSelectionGroupButton(groupIndex, false);
    }

    private void _ReselectGroup(int groupIndex)
    {
        if (!_selectionGroups.ContainsKey(groupIndex)) return;
        _DeselectAllUnits();
        foreach (EntityManager um in _selectionGroups[groupIndex])
            um.Select();
    }

    private void _DeselectAllUnits()
    {
        List<EntityManager> selectedUnits = new List<EntityManager>(Globals.SELECTED_UNITS);
        foreach (EntityManager um in selectedUnits)
            um.Deselect();
    }

    private void _SelectUnitsInDraggingBox()
    {
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            _dragStartPosition,
            Input.mousePosition
        );
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("Entity");
        bool inBounds;
        foreach (GameObject unit in selectableUnits)
        {
            inBounds = selectionBounds.Contains(
                Camera.main.WorldToViewportPoint(unit.transform.position)
            );
            if (inBounds)
                unit.GetComponent<EntityManager>().Select();
            else
                unit.GetComponent<EntityManager>().Deselect();
        }
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }

}
