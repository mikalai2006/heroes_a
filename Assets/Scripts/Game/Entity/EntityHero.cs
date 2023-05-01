using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

using Random = UnityEngine.Random;

[Serializable]
public class EntityHero : BaseEntity
{
    public static event Action<EntityHero> onChangeParamsActiveHero;
    [SerializeField] public DataHero Data = new DataHero();
    public ScriptableEntityHero ConfigData => (ScriptableEntityHero)ScriptableData;
    // public ScriptableAttributeHero ConfigAttribute => (ScriptableAttributeHero)ScriptableDataAttribute;

    public bool IsExistPath
    {
        get
        {
            return Data.path.Count() > 0 && Data.hit > 0;
        }
        private set { }
    }

    public EntityHero(
        TypeFaction typeFaction,
        ScriptableEntityHero heroData = null,
        SaveDataUnit<DataHero> saveData = null
    )
    {
        base.Init();

        if (saveData == null)
        {
            if (heroData == null)
            {
                List<ScriptableEntityHero> list = ResourceSystem.Instance
                    .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                    .Where(t => t.TypeFaction == typeFaction
                        && !UnitManager.IdsExistsHeroes.Contains(t.idObject))
                    .ToList();
                // ((ScriptableEntityHero)ScriptableData).Hero = list[UnityEngine.Random.Range(0, list.Count)];
                ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            }
            else
            {
                ScriptableData = heroData;
            }

            Data.ide = ConfigData.idObject;
            Data.hit = 100f;
            Data.speed = 100;
            Data.State = StateHero.OnMap;
            Data.name = !ConfigData.Text.title.IsEmpty ? ConfigData.Text.title.GetLocalizedString() : "No_LANg";

            Data.PSkills = new SerializableDictionary<TypePrimarySkill, int>();
            Data.PSkills.Add(TypePrimarySkill.Attack, ConfigData.ClassHero.startAttack);
            Data.PSkills.Add(TypePrimarySkill.Defense, ConfigData.ClassHero.startDefense);
            Data.PSkills.Add(TypePrimarySkill.Power, ConfigData.ClassHero.startPower);
            Data.PSkills.Add(TypePrimarySkill.Knowledge, ConfigData.ClassHero.startKnowlenge);
            Data.PSkills.Add(TypePrimarySkill.Experience, 50);
            // Data.attack = ((ScriptableEntityHero)ScriptableData).ClassHero.startAttack;
            // Data.defense = ((ScriptableEntityHero)ScriptableData).ClassHero.startDefense;
            // Data.power = ((ScriptableEntityHero)ScriptableData).ClassHero.startPower;
            // Data.knowledge = ((ScriptableEntityHero)ScriptableData).ClassHero.startKnowlenge;
            // Data.experience = 50;
            Data.level = 1;

            Data.SSkills = new SerializableDictionary<TypeSecondarySkill, int>();
            foreach (var skill in ConfigData.StartSecondarySkill)
            {
                Data.SSkills.Add(skill.SecondarySkill.TypeTwoSkill, skill.value);
            }

            _idEntity = ScriptableData.idObject;

            Data.artifacts = new List<string>();
            // Data.Artifacts = new List<EntityArtifact>();
            Data.path = new List<GridTileNode>();

            // Generate creatures.
            // ScriptableEntityHero configData = (ScriptableEntityHero)ScriptableData;
            Data.Creatures = new SerializableDictionary<int, EntityCreature>();
            for (int i = 0; i < 7; i++)
            {
                Data.Creatures.Add(i, null);
            }

            for (int i = 0; i < ConfigData.StartCreatures.Count; i++)
            {
                var creature = ConfigData.StartCreatures[i];
                var newCreature = new EntityCreature(creature.creature);
                newCreature.Data.value = Random.Range(creature.min, creature.max);
                newCreature.Data.idObject = creature.creature.idObject;
                Data.Creatures[i] = newCreature;
            }
            UnitManager.IdsExistsHeroes.Add(ScriptableData.idObject);
        }
        else
        {
            // Find config data.
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t => t.idObject == saveData.idEntity)
                .First();
            // ScriptableDataAttribute = ResourceSystem.Instance
            //     .GetAttributesByType<ScriptableAttributeHero>(TypeAttribute.Hero)
            //     .Where(t => t.idObject == saveData.data.ide)
            //     .First();
            Data = saveData.data;
            // Position = saveData.position;

            // Create creatures.
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
                        idEntity = creature.Data.idObject,
                    });
                }
                Data.Creatures[i] = newCreature;
            }

            // Create path.
            Data.path = new List<GridTileNode>();
            if (Data.State != StateHero.InTown && Data.nextPosition != Vector3Int.zero)
            {
                Data.path = FindPathForHero(saveData.data.nextPosition, true);
            }

            // var artifacts = saveData.data.artifacts;
            // for (int i = 0; i < artifacts.Count; i++)
            // {
            //     var artifact = artifacts[i];
            //     EntityArtifact newArtifact = null;
            //     if (artifact.Data.ida != "")
            //     {
            //         newArtifact = new EntityArtifact(null, null, new SaveDataUnit<DataArtifact>()
            //         {
            //             data = artifact.Data,
            //             idObject = artifact.Data.ida,
            //         });
            //     }
            //     Data.Artifacts.Add(newArtifact);
            // }
            _id = saveData.id;
            _idEntity = saveData.idEntity;
        }
        // Create artifacts.
        Data.Artifacts = new List<EntityArtifact>();
    }

    #region Events GameState
    public override void OnAfterStateChanged(GameState newState)
    {
        // base.OnBeforeStateChanged(newState);
        switch (newState)
        {
            case GameState.NextDay:
                if (LevelManager.Instance.ActivePlayer == Player)
                {
                    Data.hit = 100f;
                    Data.mana = 100f;
                }
                break;
        }
    }
    #endregion

    public void ChangePrimarySkill(TypePrimarySkill typePrimarySkill, int value = 0)
    {
        Data.PSkills[typePrimarySkill] += value;

        GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
    }

    public void ChangeSecondarySkill(TypeSecondarySkill typeSecondarySkill, int value = 0)
    {
        Data.SSkills[typeSecondarySkill] += value;

        GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
    }

    public void SetHeroAsActive()
    {
        Player.SetActiveHero(this);
        MapObject.SetPositionCamera(MapObject.Position);
        SetClearSky(MapObject.Position);

        if (Data.path != null)
        {
            GameManager.Instance.MapManager.DrawCursor(Data.path, this);
        }
        // LevelManager.Instance.ActivePlayer.ActiveHero = this;
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        Data.idPlayer = player.DataPlayer.id;
        player.AddHero(this);
    }

    #region Move
    public float CalculateHitByNode(GridTileNode node)
    {
        var dataNode = ResourceSystem.Instance.GetLandscape(node.TypeGround);
        float val = (100 - dataNode.dataNode.speed + (100 - Data.speed + 10));
        return val;
    }

    public void ChangeHitHero(GridTileNode node)
    {
        var val = CalculateHitByNode(node);
        Data.hit -= val;

        // Debug.Log($"ChangeHitHero:::[node {node.position}]{ScriptableData.name}-{Data.hit}");
        if (Player != null && Player.DataPlayer.playerType != PlayerType.Bot)
        {
            onChangeParamsActiveHero?.Invoke(this);
        }
    }


    public void SetPositionHero(Vector3Int newPosition)
    {
        // MapObjectGameObject.transform.position = newPosition;// + new Vector3(.5f, .5f);
        MapObject.SetPosition(newPosition);
        if (Player != null)
        {
            //SetPositionCamera(newPosition);
            // GameManager.Instance.MapManager.SetColorForTile(newPosition, Color.cyan);
            SetClearSky(newPosition);
        }
    }

    public void FastMoveHero(Vector3Int newPosition)
    {
        SetPositionHero(newPosition);

        var newNode = GameManager.Instance
            .MapManager
            .GridTileHelper()
            .GetNode(newPosition);

        SetGuestForNode(newNode);
        MapObject.SetPositionCamera(newPosition);
        //Move GameObject.
        MapObject.MapObjectGameObject.gameObject.transform.position = newPosition;

    }

    public void SetGuestForNode(GridTileNode newNode)
    {
        MapObject.OccupiedNode.SetAsGuested(null);
        // OccupiedNode.SetAsGuested(null);

        // OccupiedNode.SetOcuppiedUnit(null);
        SetPositionHero(newNode.position);

        MapObject.OccupiedNode = newNode;
        MapObject.OccupiedNode.SetAsGuested(MapObject);

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

    public void AddArtifact(EntityArtifact artifact)
    {
        // artifact.Data.idPlayer = _player.DataPlayer.id;
        Data.artifacts.Add(artifact.Data.ida);
        Data.Artifacts.Add(artifact);
    }

    public void RemoveArtifact(EntityArtifact artifact)
    {
        Data.artifacts.Remove(artifact.IdEntity);
        Data.Artifacts.Remove(artifact);
    }

    public async UniTask StartMove()
    {
        if (Data.path.Count > 0 && Data.hit > 0)
        {
            await ((MapEntityHero)MapObject.MapObjectGameObject).StartMove();
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.StopMoveHero);
        }
    }

    // public void SetPlayer(PlayerData playerData)
    // {
    //     Data.idPlayer = playerData.id;

    //     var _player = LevelManager.Instance.GetPlayer(Data.idPlayer);
    //     var hero = (MapEntityHero)MapObjectGameObject;
    //     // hero.SetPlayer(_player);
    // }

    public void SetPathHero(List<GridTileNode> _path = null)
    {
        if (_path != null)
        {
            // if (_path[0].position == this.Position) _path.RemoveAt(0);
            Data.path = _path;
        }
        else
        {
            Data.path = new List<GridTileNode>();
        }
        // Data.path = _path != null ? _path : new List<GridTileNode>();
        //for (int i = 1; i < path.Count; i++)
        //{
        //    HeroData.path.Add(path[i]._position);
        if (Player != null && Player.DataPlayer.playerType != PlayerType.Bot)
        {
            GameManager.Instance.MapManager.DrawCursor(Data.path, this);
        }
        //}
        if (Data.path.Count > 0)
        {
            Data.nextPosition = _path[_path.Count - 1].position;
        }
        else
        {
            Data.nextPosition = Vector3Int.zero;
        }
    }


    public List<GridTileNode> FindPathForHero(Vector3Int endPoint, bool isDiagonal)
    {
        if (MapObject == null)
        {
            return default;
        }
        // EntityHero activeHero = DataPlayer.PlayerDataReferences.ActiveHero;
        // if (this == null)
        // {
        //     GameManager.Instance.MapManager.ResetCursor();
        //     return default;
        // }
        Vector3Int startPoint = new Vector3Int(MapObject.Position.x, MapObject.Position.y);
        List<GridTileNode> path = GameManager.Instance
            .MapManager
            .GridTileHelper()
            .FindPath(startPoint, endPoint, isDiagonal);

        // Debug.Log($"Draw path::: {endPoint}[{Position}]= {path.Count}");
        this.SetPathHero(path);

        // if (
        //     path == null
        //     && Player.DataPlayer.playerType != PlayerType.Bot
        //     )
        // {
        //     var dialogData = new DataDialogHelp()
        //     {
        //         Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
        //         Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "nomove").GetLocalizedString(),
        //     };

        //     var dialogWindow = new DialogHelpProvider(dialogData);
        //     dialogWindow.ShowAndHide();
        // }

        return path;
    }
    #endregion

    public void InputHeroInTown(EntityTown activeTown)
    {
        var heroInTown = activeTown.Data.HeroinTown != null && activeTown.Data.HeroinTown != ""
            ? UnitManager.Entities[activeTown.Data.HeroinTown].MapObject
            : null;
        // merge creatures.
        if (activeTown.Data.Creatures.Where(t => t.Value != null).Count() > 0)
        {
            var resultMergeCreatures = Helpers.SummUnitBetweenList(
                this.Data.Creatures,
                activeTown.Data.Creatures
                );
            if (resultMergeCreatures.Count > 7)
            {
                // Show dialog No move.

            }
            else
            {
                this.Data.Creatures = resultMergeCreatures;
                activeTown.MapObject.OccupiedNode.SetAsGuested(heroInTown);

                // create new hero in town and destroy GameObject.
                activeTown.Data.HeroinTown = this.Id;
                // this.MapObject.MapObjectGameObject.gameObject.SetActive(false);
                this.MapObject.DestroyMapObject();

                this.Data.State = StateHero.InTown;

                // Set default slots for town.
                activeTown.ResetCreatures();
            }
        }
        else
        {
            activeTown.MapObject.OccupiedNode.SetAsGuested(heroInTown);

            // Create map object for old hero guest.
            if (activeTown.MapObject.OccupiedNode.GuestedUnit != null)
            {
                var oldHeroInTown = ((EntityHero)activeTown.MapObject.OccupiedNode.GuestedUnit.Entity);
                oldHeroInTown.Data.State = StateHero.OnMap;

                var newMapObject = new MapObject();
                oldHeroInTown.MapObject = newMapObject;
                UnitManager.MapObjects.Add(newMapObject.IdMapObject, newMapObject);

                newMapObject.SetEntity(oldHeroInTown, activeTown.MapObject.OccupiedNode);
                oldHeroInTown.MapObject.CreateMapGameObject(activeTown.MapObject.OccupiedNode);

                Player.SetActiveHero((EntityHero)activeTown.MapObject.OccupiedNode.GuestedUnit?.Entity);
            }
            else
            {
                Player.SetActiveHero(null);
            }


            // create new hero in town and destroy GameObject.
            activeTown.Data.HeroinTown = this.Id;
            MapObject.DestroyMapObject();
            Data.State = StateHero.InTown;
        }
    }

    public void OutputHeroFromTown(EntityTown activeTown)
    {
        var heroGuest = activeTown.MapObject.OccupiedNode.GuestedUnit != null
            ? (EntityHero)activeTown.MapObject.OccupiedNode.GuestedUnit.Entity
            : null;

        // move old guest and destroy GameObject.
        if (heroGuest != null)
        {
            activeTown.Data.HeroinTown = heroGuest.Id;
            heroGuest.Data.State = StateHero.InTown;
            heroGuest.MapObject.DestroyMapObject();
        }
        else
        {
            activeTown.Data.HeroinTown = "";
        }

        // create new guest and create GameObject.
        Data.State = StateHero.OnMap;

        var newMapObject = new MapObject();
        MapObject = newMapObject;
        UnitManager.MapObjects.Add(newMapObject.IdMapObject, newMapObject);
        newMapObject.SetEntity(this, activeTown.MapObject.OccupiedNode);

        MapObject.CreateMapGameObject(activeTown.MapObject.OccupiedNode);
        activeTown.MapObject.OccupiedNode.SetAsGuested(MapObject);

        Player.SetActiveHero(this);
    }

    #region AI
    public async UniTask AIFind()
    {
        var countProbePath = 5;
        var countProbe = 0;
        AIFindResource();
        while (Data.hit > 0 && countProbe < countProbePath)
        {
            if (Data.path.Count == 0)
            {
                // // .Where(t =>
                // //     t.StateNode.HasFlag(StateNode.Occupied)
                // //     && !t.StateNode.HasFlag(StateNode.Guested)
                // //     // && !t.StateNode.HasFlag(StateNode.Protected)
                // //     && !t.StateNode.HasFlag(StateNode.Empty)
                // //     && (t.OccupiedUnit != null && t.OccupiedUnit.Player != Player)
                // //     && !t.StateNode.HasFlag(StateNode.Visited)
                // //     )
                // // .OrderBy(t => GameManager.Instance
                // //     .MapManager
                // //     .gridTileHelper
                // //     .GetDistanceBetweeenPoints(Position, t.position)
                // // )
                // // .ToList();
                // // // .IsExistExit(hero.OccupiedNode, (StateNode.Occupied | ~StateNode.Guested));
                // // // .MapManager
                // // // .gridTileHelper
                // // // .GetNeighboursAtDistance(hero.OccupiedNode, 10)
                // // // .Where(t => !t.StateNode.HasFlag(StateNode.Disable))
                // // // .ToList();
                // // // var path = GameManager.Instance
                // // //     .MapManager
                // // //     .gridTileHelper
                // // //     .FindPath(
                // // //         hero.OccupiedNode.position,
                // // //         potentialPoints[Random.Range(0, potentialPoints.Count)].position,
                // // //         true
                // // //         );
                // // // hero.SetPathHero(path);
                // // // Debug.Log($"Bot::: Find path [{hero.OccupiedNode.position}]");
                // // Debug.Log($"Bot::: Move from [{hero.OccupiedNode.ToString()}] to node {path[path.Count - 1].ToString()}");
                // // var path = FindPathForHero(potentialPoints[0].position, true);

                AIFindResource();
                countProbe++;
            }

            await StartMove();
            // Debug.Log($"Data.hit after [hit={Data.hit}, path={Data.path.Count}], countProbe[{countProbe}]");
        }
    }

    public void AIFindResource()
    {
        //Find
        var potentialPoints = GameManager.Instance
            .MapManager
            .gridTileHelper
            .GetNeighboursAtDistance(MapObject.OccupiedNode, 25, true)
            .OrderBy(t => GameManager.Instance
                .MapManager
                .gridTileHelper
                .GetDistanceBetweeenPoints(MapObject.Position, t.position)
            ).ToList();

        for (int i = 0; i < potentialPoints.Count; i++)
        {
            var node = potentialPoints[i];
            if (node.OccupiedUnit != null && node.OccupiedUnit.ConfigData.TypeEntity == TypeEntity.MapObject)
            {
                ScriptableEntityMapObject configData = (ScriptableEntityMapObject)node.OccupiedUnit.ConfigData;
                if (
                    node.StateNode.HasFlag(StateNode.Occupied)
                    && !node.StateNode.HasFlag(StateNode.Guested)
                    && !node.StateNode.HasFlag(StateNode.Empty)
                    && (node.OccupiedUnit != null && node.OccupiedUnit.Entity.Player != Player)
                    && !node.StateNode.HasFlag(StateNode.Visited)
                    && node.position != MapObject.Position
                    )
                {
                    Debug.Log($"::: FindPathForHero for [{node.position}-{configData.name}]");
                    var path = FindPathForHero(node.position, true);
                    break;
                }
            }
        }
    }
    #endregion

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
