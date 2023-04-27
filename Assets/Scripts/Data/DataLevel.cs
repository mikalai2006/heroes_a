using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class DataLevel
{
    [SerializeField] public List<Player> listPlayer;
    [SerializeField] public List<Area> listArea;
    [SerializeField] public int countDay;
    [SerializeField] public int activePlayer;
    [SerializeField] public DataGameSetting Settings;
    [SerializeField] public DataGameMode GameModeData;

    public DataLevel()
    {
        listPlayer = new List<Player>();
        listArea = new List<Area>();
        GameModeData = new DataGameMode();
        Settings = new DataGameSetting();
    }
}


[System.Serializable]
public class DataUnit
{
    public List<SaveDataMapObject<DataMapObject>> mapObjects;
    public List<SaveDataUnit<DataEntityMapObject>> entityMapObjects;
    public List<SaveDataUnit<DataTown>> towns;
    public List<SaveDataUnit<DataHero>> heroes;
    public List<SaveDataUnit<DataCreature>> creatures;
    public List<SaveDataUnit<DataMonolith>> monoliths;
    public List<SaveDataUnit<DataMine>> mines;
    public List<SaveDataUnit<DataArtifact>> artifacts;
    public List<SaveDataUnit<DataExplore>> explorers;
    // public List<SaveDataUnit<DataSkillSchool>> skillSchools;
    public List<SaveDataUnit<DataEntityMapObject>> resourcesmap;
    public List<SaveDataUnit<DataEntityDwelling>> dwellings;

    public DataUnit()
    {
        mapObjects = new List<SaveDataMapObject<DataMapObject>>();
        towns = new List<SaveDataUnit<DataTown>>();
        heroes = new List<SaveDataUnit<DataHero>>();
        monoliths = new List<SaveDataUnit<DataMonolith>>();
        creatures = new List<SaveDataUnit<DataCreature>>();
        mines = new List<SaveDataUnit<DataMine>>();
        artifacts = new List<SaveDataUnit<DataArtifact>>();
        explorers = new List<SaveDataUnit<DataExplore>>();
        // skillSchools = new List<SaveDataUnit<DataSkillSchool>>();
        resourcesmap = new List<SaveDataUnit<DataEntityMapObject>>();
        dwellings = new List<SaveDataUnit<DataEntityDwelling>>();
        entityMapObjects = new List<SaveDataUnit<DataEntityMapObject>>();
    }
}

[System.Serializable]
public class SaveDataUnit<T>
{
    public string idEntity;
    public string idObject;
    public TypeEntity typeEntity;
    // public TypeMapObject typeMapObject;
    public DataEntityEffects Effects = new DataEntityEffects()
    {
        Effects = new List<DataEntityEffectsBase>()
    };
    // public Vector3Int position;
    public T data;
}

[System.Serializable]
public class SaveDataMapObject<T>
{
    public string idEntity;
    public T data;
}