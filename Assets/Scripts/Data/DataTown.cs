using System;

[Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    public int level;
    public int goldin;
    public int koofcreature;
    public bool isBuild;
    public string HeroinTown;
    public SerializableDictionary<int, EntityCreature> Creatures;
    public SerializableDictionary<string, BuildGeneral> Generals;
    public SerializableDictionary<string, BuildArmy> Armys;
}
