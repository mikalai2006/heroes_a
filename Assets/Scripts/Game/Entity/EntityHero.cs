using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class EntityHero : BaseEntity
{
    [SerializeField] public DataHero Data = new DataHero();
    public ScriptableEntityHero ConfigData => (ScriptableEntityHero)ScriptableData;
    // private bool _canMove = false;

    public bool IsExistPath
    {
        get
        {
            return Data.path.Count() > 0 && Data.hit > 0;
        }
        private set { }
    }

    public EntityHero(TypeFaction typeFaction, SaveDataUnit<DataHero> saveData = null)
    {
        base.Init();

        Data.Artifacts = new List<EntityArtifact>();
        Data.path = new List<GridTileNode>();

        if (saveData == null)
        {
            List<ScriptableEntityHero> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t => t.TypeFaction == typeFaction
                    && !UnitManager.IdsExistsHeroes.Contains(t.idObject))
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            Data.hit = 100f;
            Data.speed = 100;
            Data.State = StateHero.OnMap;
            Data.name = ScriptableData.name;
            idObject = ScriptableData.idObject;

            Data.path = new List<GridTileNode>();

            // Generate creatures.
            ScriptableEntityHero configData = (ScriptableEntityHero)ScriptableData;
            Data.Creatures = new SerializableDictionary<int, EntityCreature>();
            for (int i = 0; i < 7; i++)
            {
                Data.Creatures.Add(i, null);
            }

            for (int i = 0; i < configData.StartCreatures.Count; i++)
            {
                var creature = configData.StartCreatures[i];
                var newCreature = new EntityCreature(creature.creature);
                newCreature.Data.value = Random.Range(creature.min, creature.max);
                newCreature.Data.idObject = creature.creature.idObject;
                Data.Creatures[i] = newCreature;
            }
            UnitManager.IdsExistsHeroes.Add(ScriptableData.idObject);
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            Data.Creatures = new SerializableDictionary<int, EntityCreature>();

            var creatures = saveData.data.Creatures;
            for (int i = 0; i < creatures.Count; i++)
            {
                var creature = creatures[i];
                EntityCreature newCreature = null;
                if (creature.Data.idObject != "")
                {
                    newCreature = new EntityCreature(null, new SaveDataUnit<DataCreature>()
                    {
                        data = creature.Data,
                        idObject = creature.Data.idObject,
                    });
                }
                Data.Creatures[i] = newCreature;
            }

            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }
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
        if (Player != null)
        {
            SetPositionCamera(newPosition);
            // GameManager.Instance.MapManager.SetColorForTile(newPosition, Color.cyan);
            SetClearSky(newPosition);
        }
    }

    public void SetGuestForNode(GridTileNode newNode)
    {
        OccupiedNode.SetAsGuested(null);
        // OccupiedNode.SetAsGuested(null);

        // OccupiedNode.SetOcuppiedUnit(null);
        SetPositionHero(newNode.position);

        OccupiedNode = newNode;
        OccupiedNode.SetAsGuested(this);

        // if (newNode.OccupiedUnit == null)
        // {
        //     OccupiedNode = newNode;
        // }
        // else
        // {
        //     OccupiedNode = newNode;
        //     newNode.SetAsGuested(this);
        // }
    }

    public void SetClearSky(Vector3Int startPosition)
    {
        List<GridTileNode> noskyNode
            = GameManager.Instance.MapManager.DrawSky(startPosition, 4);
        Player.SetNosky(noskyNode);
    }

    public async UniTask StartMove()
    {
        await ((MapEntityHero)MapObjectGameObject).StartMove();
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
        Data.path = _path != null ? _path : new List<GridTileNode>();
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

    // public void SaveDataPlay(ref DataPlay data)
    // {
    //     var sdata = SaveUnit(Data);
    //     data.entity.heroes.Add(sdata);
    // }
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.heroes.Add(sdata);
    }

    #endregion
}
