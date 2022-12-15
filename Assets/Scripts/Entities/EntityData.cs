using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Scriptable Objects/Entity", order = 1)]
public class EntityData : ScriptableObject
{
    public string unitName;
    public string code;
    public string description;
    public int healthpoints;
    public float fieldOfView;
    public GameObject prefab;
    public List<ResourceValue> cost;
    public List<SkillData> skills = new();

    public bool CanBuy()
    {
        return cost.All(resource => Globals.GAME_RESOURCES[resource.code].Amount >= resource.amount);
    }
}
