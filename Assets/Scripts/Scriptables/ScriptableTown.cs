using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMapObjectTown", menuName = "Game/MapObject/New MapObject Town")]
public class ScriptableTown : ScriptableMapObjectBase
{
    public TypeFaction TypeFaction;
    public List<ScriptableHero> heroes;
    public List<ScriptableMine> mines;
    public List<ScriptableWarriors> warriors;
    public ScriptableBuildTown BuildTown;
}

