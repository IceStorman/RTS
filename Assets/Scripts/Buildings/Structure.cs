using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacement
{
    VALID,
    INVALID,
    FIXED
}

public class Structure
{
    private BuildingData _data;

    private BuildingPlacement _placement;
    private List<Material> _materials;

    private BuildingManager _buildingManager;

    private Transform _transform;
    private int _currentHealth;

    public Structure(BuildingData data)
    {
        _data = data;
        _currentHealth = data.HP;

        GameObject g = Object.Instantiate(
            Resources.Load($"Prefabs/Buildings/{_data.Code}")
        ) as GameObject;

        _transform = g.transform;

        _materials = new List<Material>();

        foreach (Material material in _transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            _materials.Add(material);
        }

        _buildingManager = g.GetComponent<BuildingManager>();
        _placement = BuildingPlacement.VALID;
        SetMaterials();
    }

    public void SetMaterials() => SetMaterials(_placement);
    public void SetMaterials(BuildingPlacement placement)
    {
        List<Material> materials;
        if (placement == BuildingPlacement.VALID)
        {
            Material refMaterial = Resources.Load("Materials/Valid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacement.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/Invalid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacement.FIXED)
        {
            materials = _materials;
        }
        else
        {
            return;
        }
        _transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }

    public void Place()
    {
        _placement = BuildingPlacement.FIXED;
        SetMaterials();
        _transform.GetComponent<BoxCollider>().isTrigger = false;
    }

    public void CheckValidPlacement()
    {
        if (_placement == BuildingPlacement.FIXED) return;
        _placement = _buildingManager.CheckPlacement()
            ? BuildingPlacement.VALID
            : BuildingPlacement.INVALID;
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public string Code { get => _data.Code; }
    public Transform Transform { get => _transform; }
    public int HP { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHP { get => _currentHealth; }
    public bool HasValidPlacement { get => _placement == BuildingPlacement.VALID; }
    public bool IsFixed { get => _placement == BuildingPlacement.FIXED; }
    public int DataIndex
    {
        get
        {
            for(int i = 0; i < Globals.BUILDING_DATA.Length; i++)
            {
                if(Globals.BUILDING_DATA[i].Code == _data.Code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
