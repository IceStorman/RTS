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
                    
                    EventManager.PhotonView.RPC("RPC_InstantiateCharacter", RpcTarget.AllBuffered, source);
                    //RPC_InstantiateCharacter(source);
                }
                break;
            default:
                break;
        }
    }

    [PunRPC]
    private void RPC_InstantiateCharacter(object source)
    {
        GameObject g = (GameObject)source;

        var coll = g.GetComponent<BoxCollider>();
        
        Vector3 instantiationPosition = new (
            g.transform.position.x - coll.size.x * 0.7f,
            g.transform.position.y,
            g.transform.position.z - coll.size.z * 0.7f
        );
        
        var d = (CharacterData)entityReference;
        Character c = new (d);
        
        c.Transform.GetComponent<NavMeshAgent>().Warp(instantiationPosition);
        c.Transform.GetComponent<CharacterManager>().Initialize(c);
    }
}
