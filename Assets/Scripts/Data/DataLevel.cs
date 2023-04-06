using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class DataLevel
{

    [SerializeField] public int countPlayer;
    [SerializeField] public int countArea;
    [SerializeField] public List<Player> listPlayer;
    [SerializeField] public List<Area> listArea;
    [SerializeField] public int countDay;
    [SerializeField] public int activePlayer;

    public DataLevel()
    {
        listPlayer = new List<Player>();
        listArea = new List<Area>();
    }
}


[System.Serializable]
public class DataUnit
{
    public List<SaveDataUnit<DataResourceMapObject>> resources;
    public List<SaveDataUnit<DataTown>> towns;
    public List<SaveDataUnit<DataHero>> heroes;
    public List<SaveDataUnit<DataCreature>> warriors;
    public List<SaveDataUnit<DataMonolith>> monoliths;
    public List<SaveDataUnit<DataMine>> mines;
    public List<SaveDataUnit<DataArtifact>> artifacts;
    public List<SaveDataUnit<DataExplore>> explorers;
    public List<SaveDataUnit<DataSkillSchool>> skillSchools;
    public List<SaveDataUnit<DataResourceMapObject>> resourcesmap;

    public DataUnit()
    {
        towns = new List<SaveDataUnit<DataTown>>();
        heroes = new List<SaveDataUnit<DataHero>>();
        monoliths = new List<SaveDataUnit<DataMonolith>>();
        warriors = new List<SaveDataUnit<DataCreature>>();
        mines = new List<SaveDataUnit<DataMine>>();
        resources = new List<SaveDataUnit<DataResourceMapObject>>();
        artifacts = new List<SaveDataUnit<DataArtifact>>();
        explorers = new List<SaveDataUnit<DataExplore>>();
        skillSchools = new List<SaveDataUnit<DataSkillSchool>>();
        resourcesmap = new List<SaveDataUnit<DataResourceMapObject>>();
    }
}

[System.Serializable]
public struct SaveDataUnit<T>
{
    public string idUnit;
    public string idObject;
    public TypeEntity typeEntity;
    // public TypeMapObject typeMapObject;
    public Vector3Int position;
    public T data;
}