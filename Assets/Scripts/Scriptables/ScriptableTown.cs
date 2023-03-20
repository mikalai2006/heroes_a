using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Create a scriptable town 
/// </summary>
[CreateAssetMenu(fileName = "New Town", menuName = "Units/New Town")]
public class ScriptableTown : ScriptableUnitBase {

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

