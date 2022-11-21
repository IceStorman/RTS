using UnityEngine;

public class CharacterManager : EntityManager
{
    private Character character;
    public override Entity Entity
    {
        get => character;
        set => character = value is Character character1 ? character1 : null;
    }
}
