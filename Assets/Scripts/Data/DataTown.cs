using System;
using System.Collections.Generic;

[Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    // public List<TypeBuild> ProgressBuilds;
    // public TypeBuildArmy ProgressBuildsArmy;
    public bool isBuild;
    public SerializableDictionary<TypeBuild, BuildItem> LevelsBuilds;

    // public EntityHero Guest;
    public EntityHero HeroinTown;
    public SerializableDictionary<string, BuildGeneral> Generals;
    public SerializableDictionary<string, BuildArmy> Armys;
}

[Serializable]
public struct BuildItem
{
    public List<string> ids;
    public int level;

}