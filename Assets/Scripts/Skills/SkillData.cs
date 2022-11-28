using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum SkillType
{
    INSTANTIATE_CHARACTER
}

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill", order = 4)]
public class SkillData : ScriptableObject
{
    public string code;
    public string skillName;
    public string description;
    public SkillType type;
    [FormerlySerializedAs("unitReference")] public EntityData entityReference;
    public float castTime;
    public float cooldown;
    public Sprite sprite;

    public void Trigger(GameObject source, GameObject target = null)
    {
        switch (type)
        {
            case SkillType.INSTANTIATE_CHARACTER:
                {
                    RPC_InstantiateCharacter(source);
                }
                break;
            default:
                break;
        }
    }

    [PunRPC]
    private void RPC_InstantiateCharacter(GameObject source)
    {
        BoxCollider coll = source.GetComponent<BoxCollider>();
        
        Vector3 instantiationPosition = new Vector3(
            source.transform.position.x - coll.size.x * 0.7f,
            source.transform.position.y,
            source.transform.position.z - coll.size.z * 0.7f
        );
        
        CharacterData d = (CharacterData)entityReference;
        Character c = new Character(d);
        
        c.Transform.GetComponent<NavMeshAgent>().Warp(instantiationPosition);
        c.Transform.GetComponent<CharacterManager>().Initialize(c);
    }
}
