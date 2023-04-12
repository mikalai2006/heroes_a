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
    public SerializableShortPosition nosky = new SerializableShortPosition();
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
    [SerializeField] public List<EntityHero> ListHero;
    [SerializeField] public List<EntityTown> ListTown;
    [SerializeField] public List<EntityMine> ListMines;
    [SerializeField] public EntityHero ActiveHero;
    [SerializeField] public EntityTown ActiveTown;

    public PlayerDataReferences()
    {
        ListTown = new List<EntityTown>();
        ListHero = new List<EntityHero>();
        ListMines = new List<EntityMine>();
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
    public EntityHero ActiveHero => _data.PlayerDataReferences.ActiveHero;
    public EntityTown ActiveTown => _data.PlayerDataReferences.ActiveTown;

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
    }

    public void SetActiveHero(EntityHero entityHero)
    {
        // GameManager.Instance.ChangeState(GameState.ChooseHero);

        _data.PlayerDataReferences.ActiveHero = entityHero;
    }

    public void SetActiveTown(EntityTown entityTown)
    {
        // GameManager.Instance.ChangeState(GameState.ChooseHero);

        _data.PlayerDataReferences.ActiveTown = entityTown;

    }

    public void AddHero(EntityHero hero)
    {
        // hero.SetPlayer(DataPlayer);
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

    public void AddMines(EntityMine mine)
    {
        // mine.SetPlayer(this);
        DataPlayer.PlayerDataReferences.ListMines.Add(mine);
    }
    public void ChangeResource(TypeResource typeResource, int value = 0)
    {
        _data.Resource[typeResource] += value;

        GameManager.Instance.ChangeState(GameState.ChangeResources);
    }

    public bool IsExistsResource(List<CostEntity> resources)
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

    public EntityHero GetHero(int id)
    {
        return DataPlayer.PlayerDataReferences.ListHero[id];
    }

    public List<GridTileNode> FindPathForHero(Vector3Int endPoint, bool force)
    {
        EntityHero activeHero = DataPlayer.PlayerDataReferences.ActiveHero;
        Vector3Int startPoint = new Vector3Int(activeHero.Position.x, activeHero.Position.y);
        List<GridTileNode> path = GameManager.Instance.MapManager.GridTileHelper().FindPath(startPoint, endPoint, force);

        if (path != null && DataPlayer.PlayerDataReferences.ActiveHero != null)
        {
            activeHero.SetPathHero(path);
            GameManager.Instance.MapManager.DrawCursor(path, activeHero);
            GameManager.Instance.ChangeState(GameState.CreatePathHero);
        }


        return path;
    }

    public void AddTown(BaseEntity town)
    {
        // EntityTown _town = (EntityTown)town;
        // _town.SetPlayer(DataPlayer);
        DataPlayer.PlayerDataReferences.ListTown.Add((EntityTown)town);

    }

    public EntityTown GetTown(int id)
    {
        return DataPlayer.PlayerDataReferences.ListTown[id];
    }

}
