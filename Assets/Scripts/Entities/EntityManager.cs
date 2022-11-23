using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public GameObject selectionCircle;

    private Transform _canvas;
    private GameObject _healthbar;

    protected BoxCollider _collider;
    public virtual Entity Entity { get; set; }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
    }

    public void Initialize(Entity entity)
    {
        _collider = GetComponent<BoxCollider>();
        Entity = entity;
    }

    private void OnMouseDown()
    {
        if (IsActive())
        {
            Select(
                true,
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift)
            );
        }
    }

    protected virtual bool IsActive()
    {
        return true;
    }

    private void _SelectUtil()
    {
        if (Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);

        if (_healthbar == null)
        {
            _healthbar = Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            _healthbar.transform.SetParent(_canvas);
            Healthbar h = _healthbar.GetComponent<Healthbar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                transform.Find("Mesh").GetComponent<Renderer>().bounds,
                Camera.main
            );
            h.Initialize(transform, boundingBox.height);
            h.SetPosition();
        }
        EventManager.TriggerEvent("SelectEntity", Entity);
    }

    public void Select() => Select(false, false);

    public void Select(bool singleClick, bool holdingShift)
    {
        if (!singleClick)
        {
            _SelectUtil();
            return;
        }

        if (!holdingShift)
        {
            List<EntityManager> selectedUnits = new List<EntityManager>(Globals.SELECTED_UNITS);
            foreach (EntityManager um in selectedUnits)
                um.Deselect();
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
                _SelectUtil();
            else
                Deselect();
        }
    }

    public void Deselect()
    {
        if (!Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
        Destroy(_healthbar);
        _healthbar = null;
        EventManager.TriggerEvent("DeselectEntity", Entity);
    }
}
