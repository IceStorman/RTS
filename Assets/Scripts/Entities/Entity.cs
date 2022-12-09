using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    protected EntityData _data;
    protected Transform _transform;
    protected int _currentHealth;
    protected string _uid;
    protected int _level;
    protected List<ResourceValue> _production;
    protected List<SkillManager> _skillManagers;

    public Entity(EntityData data) : this(data, new List<ResourceValue>()) { }

    public Entity(EntityData data, List<ResourceValue> production)
    {
        _data = data;
        _currentHealth = data.healthpoints;

        var g = Object.Instantiate(data.prefab);
        _transform = g.transform;

        _uid = System.Guid.NewGuid().ToString();
        _level = 1;
        _production = production;

        _skillManagers = new List<SkillManager>();
        foreach (SkillData skill in _data.skills)
        {
            var sm = g.AddComponent<SkillManager>();
            sm.Initialize(skill, g);
            _skillManagers.Add(sm);
        }
    }

    public void LevelUp()
    {
        _level += 1;
    }

    public void ProduceResources()
    {
        foreach (ResourceValue resource in _production)
            Globals.GAME_RESOURCES[resource.code].AddAmount(resource.amount);
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public virtual void Place()
    {
        _transform.GetComponent<BoxCollider>().isTrigger = false;
        
        foreach (ResourceValue resource in _data.cost)
        {
            Globals.GAME_RESOURCES[resource.code].AddAmount(-resource.amount);
        }
    }

    public bool CanBuy()
    {
        return _data.CanBuy();
    }

    public void TriggerSkill(int index, GameObject target = null)
    {
        _skillManagers[index].Trigger(target);
    }

    public EntityData Data { get => _data; }
    public string Code { get => _data.code; }
    public Transform Transform { get => _transform; }
    public int HP { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHP { get => _data.healthpoints; }
    public string Uid { get => _uid; }
    public int Level { get => _level; }
    public List<ResourceValue> Production { get => _production; }
    public List<SkillManager> SkillManagers { get => _skillManagers; }
}
