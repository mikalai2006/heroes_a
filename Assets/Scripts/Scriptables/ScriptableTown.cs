using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMapTown", menuName = "Game/Units/New Map Town")]
public class ScriptableTown : ScriptableUnitBase
{
    public TypeFaction TypeFaction;
    public List<ScriptableHero> heroes;
    public List<ScriptableMine> mines;
    public List<ScriptableWarriors> warriors;
}

