using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : EntityManager
{
    public NavMeshAgent agent;
    
    private Character character;
    public override Entity Entity
    {
        get => character;
        set => character = value is Character character1 ? character1 : null;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.destination = targetPosition;
    }
}
