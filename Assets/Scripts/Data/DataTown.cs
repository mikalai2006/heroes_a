using System;

[Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    public int level;
    public int koofcreature;
    public int countBuild;
    public string HeroinTown;
    public SerializableDictionary<TypeResource, int> Resources;
    public SerializableDictionary<int, EntityCreature> Creatures;
    public SerializableDictionary<string, BuildGeneral> Generals;
    public SerializableDictionary<string, BuildArmy> Armys;
}
