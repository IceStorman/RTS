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
    public EntityData entityReference;
    public float castTime;
    public float cooldown;
    public Sprite sprite;

    public void Trigger(GameObject source, GameObject target = null)
    {
        switch (type)
        {
            case SkillType.INSTANTIATE_CHARACTER:
                {
                    InstantiateCharacter(source);
                }
                break;
            default:
                break;
        }
    }

    private void InstantiateCharacter(GameObject source)
    {
        var coll = source.GetComponent<BoxCollider>();

        var position = source.transform.position;

        var size = coll.size;
        Vector3 instantiationPosition = new (
            position.x - size.x * 0.7f,
            position.y,
            position.z - size.z * 0.7f
        );
        var x = instantiationPosition.x;
        var y = instantiationPosition.y;
        var z = instantiationPosition.z;
        
        //photonView.RPC("RPC_InstantiateCharacter", RpcTarget.AllBuffered, x, y, z);
        RPC_InstantiateCharacter(x, y, z);
    }
    
    [PunRPC]
    private void RPC_InstantiateCharacter(float x, float y, float z)
    {
        var d = (CharacterData)entityReference;
        Character c = new (d);
        
        Vector3 instantiationPosition = new(x, y, z);
        c.Transform.GetComponent<NavMeshAgent>().Warp(instantiationPosition);
        c.Transform.GetComponent<CharacterManager>().Initialize(c);
        
        Globals.AddEntity(c);
    }
}
