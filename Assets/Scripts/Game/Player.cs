using System;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int id;
    public Color color;
    public PlayerType playerType;
    public int idArea;
    public SerializableDictionary<TypeResource, int> Resource;
    public SerializableShortPosition nosky;
    [System.NonSerialized] public PlayerDataReferences PlayerDataReferences;

    // public Hero ActiveHero => PlayerDataReferences.ActiveHero;

    public PlayerData()
    {
        PlayerDataReferences = new PlayerDataReferences();
    }
}

[System.Serializable]
public class PlayerDataReferences
{
    [SerializeField] public List<MapEntityHero> ListHero;
    [SerializeField] public List<BaseMapEntity> ListTown;
    [SerializeField] public List<BaseMapEntity> ListMines;
    [SerializeField] public MapEntityHero ActiveHero;
    [SerializeField] public MapEntityTown ActiveTown;

    public PlayerDataReferences()
    {
        ListTown = new List<BaseMapEntity>();
        ListHero = new List<MapEntityHero>();
        ListMines = new List<BaseMapEntity>();
    }
}

public enum PlayerType
{
    User = 0,
    Enemy = 1,
    Bot = 2,
}

[System.Serializable]
public class Player
{
    [SerializeField] private PlayerData _data;
    public PlayerData DataPlayer
    {
        get { return _data; }
        set
        {
            _data = value;

            // Event change data.

        }
    }
    public MapEntityHero ActiveHero
    {
        get { return _data.PlayerDataReferences.ActiveHero; }
        set
        {
            GameManager.Instance.ChangeState(GameState.ChooseHero);

            _data.PlayerDataReferences.ActiveHero = value;

            LevelManager.Instance.SetPositionCamera(new Vector3(value.Position.x, value.Position.y, -10f));

            if (value.path != null)
            {
                GameManager.Instance.MapManager.DrawCursor(value.path, DataPlayer.PlayerDataReferences.ActiveHero);
            }

        }
    }

    public MapEntityTown ActiveTown
    {
        get { return _data.PlayerDataReferences.ActiveTown; }
        set
        {
            GameManager.Instance.ChangeState(GameState.ChooseHero);

            _data.PlayerDataReferences.ActiveTown = value;

            LevelManager.Instance.SetPositionCamera(new Vector3(value.Position.x, value.Position.y, -10f));

        }
    }

    public Player(PlayerData data)
    {
        _data = new PlayerData();
        _data = data;
        _data.PlayerDataReferences = new PlayerDataReferences();
        _data.Resource = new SerializableDictionary<TypeResource, int>();
        foreach (TypeResource typeResource in (TypeResource[])Enum.GetValues(typeof(TypeResource)))
        {
            _data.Resource.Add(typeResource, typeResource == TypeResource.Gold ? 100000 : 200);
        }
        _data.nosky = new SerializableShortPosition();
    }

    public void AddHero(MapEntityHero hero)
    {
        hero.SetPlayer(DataPlayer);
        DataPlayer.PlayerDataReferences.ListHero.Add(hero);
    }

    public void SetNosky(List<GridTileNode> listNode)
    {
        for (int i = 0; i < listNode.Count; i++)
        {
            if (!_data.nosky.ContainsKey(listNode[i].position))
            {
                _data.nosky.Add(listNode[i].position, true);
            }
        }
    }

    public void AddMines(MapEntityMine mine)
    {
        mine.SetPlayer(this);
        DataPlayer.PlayerDataReferences.ListMines.Add(mine);
    }
    public void ChangeResource(TypeResource typeResource, int value = 0)
    {
        _data.Resource[typeResource] += value;

        GameManager.Instance.ChangeState(GameState.ChangeResources);
    }

    public bool IsExistsResource(List<BuildCostResource> resources)
    {
        bool result = true;
        foreach (var res in resources)
        {
            if (_data.Resource[res.Resource.TypeResource] < res.Count)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public MapEntityHero GetHero(int id)
    {
        return DataPlayer.PlayerDataReferences.ListHero[id];
    }

    // public void SetActiveTown(BaseTown town)
    // {
    //     GameManager.Instance.ChangeState(GameState.ChooseHero);

    //     _data.ActiveTown = town;

    //     LevelManager.Instance.SetPositionCamera(new Vector3(town.Position.x, town.Position.y, -10f));

    // }

    public List<GridTileNode> FindPathForHero(Vector3Int endPoint, bool isTrigger, bool force)
    {
        MapEntityHero activeHero = DataPlayer.PlayerDataReferences.ActiveHero;
        Vector3Int startPoint = new Vector3Int(activeHero.Position.x, activeHero.Position.y);
        List<GridTileNode> path = GameManager.Instance.MapManager.GridTileHelper().FindPath(startPoint, endPoint, isTrigger, force);

        if (path != null && DataPlayer.PlayerDataReferences.ActiveHero != null)
        {
            activeHero.SetPathHero(path);
            GameManager.Instance.MapManager.DrawCursor(path, activeHero);
            GameManager.Instance.ChangeState(GameState.CreatePathHero);
        }


        return path;
    }

    public void AddTown(BaseMapEntity town)
    {
        MapEntityTown _town = (MapEntityTown)town;
        _town.SetPlayer(DataPlayer);
        DataPlayer.PlayerDataReferences.ListTown.Add(town);

    }

    public BaseMapEntity GetTown(int id)
    {
        return DataPlayer.PlayerDataReferences.ListTown[id];
    }
}
