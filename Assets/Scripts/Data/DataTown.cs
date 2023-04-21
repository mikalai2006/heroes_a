using System;
using System.Collections.Generic;

[Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    public int goldin;
    public int koofcreature;
    public bool isBuild;
    // public SerializableDictionary<TypeBuild, BuildItem> LevelsBuilds;
    public string HeroinTown;
    public SerializableDictionary<int, EntityCreature> Creatures;
    public SerializableDictionary<string, BuildGeneralBase> Generals;
    public SerializableDictionary<string, BuildArmy> Armys;
}

// [Serializable]
// public struct BuildItem
// {
//     public List<string> ids;
//     public int level;
// }