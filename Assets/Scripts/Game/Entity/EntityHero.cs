using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntityHero : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataHero Data = new DataHero();
    public ScriptableEntityHero ConfigData => (ScriptableEntityHero)ScriptableData;
    private bool _canMove = false;

    public bool CanMove
    {
        get
        {
            return Data.path.Count > 0 && Data.hit > 0;
        }
        private set { }
    }

    public EntityHero(GridTileNode node, TypeFaction typeFaction, SaveDataUnit<DataHero> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityHero> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t => t.TypeFaction == typeFaction)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            Data.hit = 100f;
            Data.speed = 100;
            Data.name = ScriptableData.name;

            Data.path = new List<GridTileNode>();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public float CalculateHitByNode(GridTileNode node)
    {
        var dataNode = ResourceSystem.Instance.GetLandscape(node.TypeGround);
        float val = (100 - dataNode.dataNode.speed + (100 - Data.speed + 10));
        //Debug.Log($"CalculateHitByNode::: {val}");
        return val;
    }

    public void SetHeroAsActive()
    {
        Player.SetActiveHero(this);
        SetPositionCamera(this.Position);
        SetClearSky(Position);

        if (Data.path != null)
        {
            GameManager.Instance.MapManager.DrawCursor(Data.path, this);
        }
        // LevelManager.Instance.ActivePlayer.ActiveHero = this;
    }

    public void SetPositionHero(Vector3Int newPosition)
    {
        // MapObjectGameObject.transform.position = newPosition;// + new Vector3(.5f, .5f);
        Position = newPosition;
        SetPositionCamera(newPosition);
        GameManager.Instance.MapManager.SetColorForTile(newPosition, Color.cyan);
        SetClearSky(newPosition);
    }

    public void SetClearSky(Vector3Int startPosition)
    {
        List<GridTileNode> noskyNode
            = GameManager.Instance.MapManager.DrawSky(startPosition, 4);
        Player.SetNosky(noskyNode);
    }

    public void SetPlayer(PlayerData playerData)
    {
        Data.idPlayer = playerData.id;

        var _player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        var hero = (MapEntityHero)MapObjectGameObject;
        // hero.SetPlayer(_player);
    }

    public void SetPathHero(List<GridTileNode> _path = null)
    {
        Data.path = _path;
        //for (int i = 1; i < path.Count; i++)
        //{
        //    HeroData.path.Add(path[i]._position);
        GameManager.Instance.MapManager.DrawCursor(Data.path, this);
        //}
        // Data.nextPosition = _path == null ?  : Data.path[Data.path.Count - 1].position;
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        Data.idPlayer = player.DataPlayer.id;
        player.AddHero(this);
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     // throw new NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.heroes.Add(sdata);
    }

    #endregion
}

[System.Serializable]
public struct DataHero
{
    public int idPlayer;
    public Vector3Int nextPosition;
    public float speed;
    public float hit;
    public float mana;
    public string name;

    [NonSerialized] public List<GridTileNode> path;
}