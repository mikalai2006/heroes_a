using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTown", menuName = "Game/Units/New Town")]
public class ScriptableTown : ScriptableUnitBase
{

    //public TownType TownType;
    //public TypeGround typeGround;
    public List<ScriptableHero> heroes;
    public List<ScriptableMine> mines;
    public List<ScriptableWarriors> warriors;
}

//public enum TownType
//{
//    Castle = 0,

//}

