using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    protected EntityData data;
    protected Transform transform;
    protected int currentHealth;
    protected string uid;
    protected int level;
    protected List<ResourceValue> production;
    protected List<SkillManager> skillManagers;

    public Entity(EntityData data) : this(data, new List<ResourceValue>()) { }

    public Entity(EntityData data, List<ResourceValue> production)
    {
        this.data = data;
        currentHealth = data.healthpoints;

        var g = Object.Instantiate(data.prefab);
        transform = g.transform;
        //transform.Find("FOV").transform.localScale = new Vector3(data.fieldOfView, data.fieldOfView, 1f);

        uid = System.Guid.NewGuid().ToString();
        level = 1;
        this.production = production;

        skillManagers = new List<SkillManager>();
        foreach (SkillData skill in this.data.skills)
        {
            var sm = g.AddComponent<SkillManager>();
            sm.Initialize(skill, g);
            skillManagers.Add(sm);
        }
    }

    public void LevelUp()
    {
        level += 1;
    }

    public void ProduceResources()
    {
        foreach (ResourceValue resource in production)
            Globals.GAME_RESOURCES[resource.code].AddAmount(resource.amount);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void Place()
    {
        transform.GetComponent<BoxCollider>().isTrigger = false;

        transform.GetComponent<EntityManager>().EnableFOV();
    }

    public bool CanBuy()
    {
        return data.CanBuy();
    }

    public void TriggerSkill(int index, GameObject target = null)
    {
        skillManagers[index].Trigger(target);
    }

    public EntityData Data { get => data; }
    public string Code { get => data.code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => data.healthpoints; }
    public string Uid { get => uid; }
    public int Level { get => level; }
    public List<ResourceValue> Production { get => production; }
    public List<SkillManager> SkillManagers { get => skillManagers; }
}
