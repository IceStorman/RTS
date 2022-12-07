using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterManager : EntityManager
{
    private NavMeshAgent agent;
    
    private Character character;
    public override Entity Entity
    {
        get => character;
        set => character = value is Character character1 ? character1 : null;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }
}
