using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntityHero : BaseEntity, IDataPlay
{
    public ScriptableEntityHero ConfigData => (ScriptableEntityHero)ScriptableData;
    private bool _canMove = false;
    [SerializeField] public DataHero Data = new DataHero();

    public bool CanMove
    {
        get
        {
            return Data.path.Count > 0 && Data.hit > 0;
        }
        private set { }
    }

    public EntityHero(GridTileNode node)
    {
        List<ScriptableEntityHero> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        Data.hit = 100f;
        Data.speed = 100;
        Data.name = ScriptableData.name;

        Data.path = new List<GridTileNode>();
        base.Init(ScriptableData, node);
    }

    public void LoadDataPlay(DataPlay data)
    {
        // throw new NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.heroes.Add(sdata);
    }

    public float CalculateHitByNode(GridTileNode node)
    {
        var dataNode = ResourceSystem.Instance.GetLandscape(node.TypeGround);
        float val = (100 - dataNode.dataNode.speed + (100 - Data.speed + 10));
        //Debug.Log($"CalculateHitByNode::: {val}");
        return val;
    }

    public void SetPlayer(PlayerData playerData)
    {
        Data.idPlayer = playerData.id;

        var _player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        var hero = (MapEntityHero)MapObjectGameObject;
        // hero.SetPlayer(_player);
    }

    public void SetPathHero(List<GridTileNode> _path)
    {
        Data.path = _path;
        //for (int i = 1; i < path.Count; i++)
        //{
        //    HeroData.path.Add(path[i]._position);

        //}
        Data.nextPosition = Data.path[Data.path.Count - 1].position;
    }

    public void SetHeroAsActive()
    {
        LevelManager.Instance.ActivePlayer.ActiveHero = this;
    }
}

[System.Serializable]
public class DataHero
{
    public int idPlayer;
    public Vector3Int nextPosition;
    public float speed;
    public float hit;
    public float mana;
    public string name;

    [NonSerialized] public List<GridTileNode> path;
}