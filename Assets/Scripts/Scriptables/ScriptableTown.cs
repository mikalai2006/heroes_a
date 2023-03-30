using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTown", menuName = "Game/Units/New Town")]
public class ScriptableTown : ScriptableUnitBase
{
    public List<ScriptableHero> heroes;
    public List<ScriptableMine> mines;
    public List<ScriptableWarriors> warriors;
}

