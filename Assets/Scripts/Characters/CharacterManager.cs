using UnityEngine;

public class CharacterManager : UnitManager
{
    private Character character;
    public override Unit Unit
    {
        get => character;
        set => character = value is Character character1 ? character1 : null;
    }
}
