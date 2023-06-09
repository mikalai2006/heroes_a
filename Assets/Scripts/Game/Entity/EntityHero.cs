using System;
using System.Collections.Generic;
using System.Linq;
// using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
// using UnityEngine.Localization;

using Random = UnityEngine.Random;

[Serializable]
public class EntityHero : BaseEntity
{
    public static event Action<EntityHero> OnChangeParamsActiveHero;
    [SerializeField] public DataHero Data = new DataHero();
    [NonSerialized] public EntityBook SpellBook;
    //sqrt((Attack × 0.05 + 1) × (Defense × 0.05 + 1))
    public int Streight => (int)((Data.PSkills[TypePrimarySkill.Attack] * 0.05f + 1) * (Data.PSkills[TypePrimarySkill.Defense] * 0.05f + 1));
    public int AIHeroCreatures => Data.Creatures.Select(t => t.Value != null ? t.Value.totalAI : 0).Sum() * Streight;
    // [NonSerialized] public ArenaHeroEntity ArenaHeroEntity;
    public ScriptableEntityHero ConfigData => (ScriptableEntityHero)ScriptableData;
    // public ScriptableAttributeHero ConfigAttribute => (ScriptableAttributeHero)ScriptableDataAttribute;

    public bool IsExistPath
    {
        get
        {
            return Data.path != null && Data.path.Count > 0 && Data.mp > 0;
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
            // Data.hit = 100f;
            Data.speed = 100;
            Data.State = StateHero.OnMap;
            Data.name = !ConfigData.Text.title.IsEmpty ? ConfigData.Text.title.GetLocalizedString() : "No_LANg";

            Data.PSkills = new SerializableDictionary<TypePrimarySkill, int>();
            Data.PSkills.Add(TypePrimarySkill.Attack, ConfigData.ClassHero.startAttack);
            Data.PSkills.Add(TypePrimarySkill.Defense, ConfigData.ClassHero.startDefense);
            Data.PSkills.Add(TypePrimarySkill.Power, ConfigData.ClassHero.startPower);
            Data.PSkills.Add(TypePrimarySkill.Knowledge, ConfigData.ClassHero.startKnowlenge);
            Data.PSkills.Add(TypePrimarySkill.Experience, Random.Range(40, 90));
            Data.nextLevel = 1000;
            // Data.attack = ((ScriptableEntityHero)ScriptableData).ClassHero.startAttack;
            // Data.defense = ((ScriptableEntityHero)ScriptableData).ClassHero.startDefense;
            // Data.power = ((ScriptableEntityHero)ScriptableData).ClassHero.startPower;
            // Data.knowledge = ((ScriptableEntityHero)ScriptableData).ClassHero.startKnowlenge;
            // Data.experience = 50;
            Data.level = 1;
            Data.nextLevel = 1000;

            Data.SSkills = new SerializableDictionary<TypeSecondarySkill, SkillDataItem>();
            // Data.SecondarySkills = new SerializableDictionary<ScriptableAttributeSecondarySkill, int>();
            foreach (var skill in ConfigData.StartSecondarySkill)
            {
                var skillData = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
                .Find(t => t.TypeTwoSkill == skill.SecondarySkill.TypeTwoSkill);

                Data.SSkills.Add(skill.SecondarySkill.TypeTwoSkill, new SkillDataItem()
                {
                    level = skill.value,
                    value = skillData.Levels[skill.value].value
                });
                // skill.SecondarySkill.RunEffect(Player, this);
                // Data.SecondarySkills.Add(skill.SecondarySkill, skill.value);
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

            // Create book if exists start spells.
            if (ConfigData.StartSpells.Count > 0)
            {
                Data.isBook = true;
                Data.spells = ConfigData.StartSpells.Select(t => t.idObject).ToList();
            }

            // Create War Machine.
            AddWarMachine(TypeWarMachine.Catapult);

            if (ConfigData.isBallista)
            {
                AddWarMachine(TypeWarMachine.Ballista);
            }

            if (ConfigData.isAmmoCart)
            {
                AddWarMachine(TypeWarMachine.AmmoCart);
            }

            if (ConfigData.isFirstAidTent)
            {
                AddWarMachine(TypeWarMachine.FirstAidTent);
            }

            // Create movement data.
            Data.mp = GetMoveMentPoints();

            // Set start value mana.
            Data.mana = GetMana();

            UnitManager.IdsExistsHeroes?.Add(ScriptableData.idObject);
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

            // Create War Machine.
            AddWarMachine(TypeWarMachine.Catapult);

            if (ConfigData.isBallista)
            {
                AddWarMachine(TypeWarMachine.Ballista);
            }

            if (ConfigData.isAmmoCart)
            {
                AddWarMachine(TypeWarMachine.AmmoCart);
            }

            if (ConfigData.isFirstAidTent)
            {
                AddWarMachine(TypeWarMachine.FirstAidTent);
            }
        }
        // Create artifacts.
        Data.Artifacts = new List<EntityArtifact>();

        // Create SpellBook.
        if (Data.isBook)
        {
            SpellBook = new EntityBook(this);
        }
    }

    private float GetMoveMentPoints()
    {
        var baseMp = LevelManager.Instance.ConfigGameSettings.baseMovementValue;
        float result = baseMp;
        var tableDependicy = LevelManager.Instance.ConfigGameSettings.DependencyCreatureOnMove;
        var slowCreature = Data.Creatures
            .Where(t => t.Value != null)
            .OrderBy(t => t.Value.ConfigAttribute.CreatureParams.Speed)
            .First()
            .Value;
        var valueSpeedCreature = slowCreature.ConfigAttribute.CreatureParams.Speed;
        var itemMovement = tableDependicy.Find(t => t.levelCreature == valueSpeedCreature);
        if (valueSpeedCreature > 2 && valueSpeedCreature < 12)
        {
            result = itemMovement.movementValue;
        }
        if (valueSpeedCreature >= 12)
        {
            result = tableDependicy[tableDependicy.Count - 1].movementValue;
        }

        // add bonus secondarySkill.
        var mpSSkil = 0f;
        if (Data.SSkills.ContainsKey(TypeSecondarySkill.Logistics))
        {
            mpSSkil = ((result * Data.SSkills[TypeSecondarySkill.Logistics].value) / 100f);
        }

        // bonus special = mpSSkil * (0.05f * Data.level + 1) in %
        // result = result + (((mpSSkil * (Data.level * 0.05f + 1)) / 100f) * result);
        // Debug.Log($"GetMoveMentPoints ::: result={result}[r={((int)((mpSSkil * Data.level * 1.05f) / 100f) * result)}][valueSpeedCreature={valueSpeedCreature}][itemMovement.movementValue={itemMovement.movementValue}]");

        result = result + mpSSkil;
        Debug.Log($"GetMoveMentPoints ::: result={result}[ssk={mpSSkil}]");
        return result;
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
                    Data.mp = GetMoveMentPoints();

                    // Effect Estates
                    if (Data.SSkills.ContainsKey(TypeSecondarySkill.Estates))
                    {
                        var valueEstates = Data.SSkills[TypeSecondarySkill.Estates];
                        Player.ChangeResource(TypeResource.Gold, valueEstates.value);
                    }

                    // Recovery mana if hero in town
                    if (Data.mana < GetMana())
                    {
                        int countMana = LevelManager.Instance.ConfigGameSettings.countRecoveryManaPerDay;
                        // Effect Mysticism
                        if (Data.SSkills.ContainsKey(TypeSecondarySkill.Mysticism))
                        {
                            var valueMysticism = Data.SSkills[TypeSecondarySkill.Mysticism];
                            Data.mana += valueMysticism.value;
                        }
                        else
                        {
                            Data.mana += countMana;
                        }
                    }


                }
                break;
        }
    }
    #endregion

    #region Getters
    public float GetMana()
    {
        int result = Data.PSkills[TypePrimarySkill.Knowledge] * 10;

        // Effect Intelligence
        if (Data.SSkills.ContainsKey(TypeSecondarySkill.Intelligence))
        {
            result = result + (int)((result * Data.SSkills[TypeSecondarySkill.Intelligence].value) / 100f);
        }

        return result;
    }
    public void ChangeManaHero(float value)
    {
        Data.mana += value;

        if (Player != null && Player.DataPlayer.playerType != PlayerType.Bot)
        {
            OnChangeParamsActiveHero?.Invoke(this);
        }
    }

    public int GetLevelSSkil(TypeSecondarySkill typeSecondarySkill)
    {
        int result = -1;
        if (Data.SSkills.ContainsKey(typeSecondarySkill))
        {
            result = Data.SSkills[typeSecondarySkill].level;
        }
        return result;
    }
    #endregion

    public void SetHeroAsActive()
    {
        Player.SetActiveHero(this);
        MapObject.SetPositionCamera(MapObject.Position);
        SetClearSky(MapObject.OccupiedNode);

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
        // player.AddHero(this);
    }

    public void AddWarMachine(TypeWarMachine typeWarMachine)
    {
        if (Data.WarMachines == null) Data.WarMachines = new();
        var allWarMachine = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeWarMachine>(TypeAttribute.WarMachine);
        var configBallista = allWarMachine
            .Where(t => t.TypeWarMachine == typeWarMachine)
            .First();
        Data.WarMachines[typeWarMachine] = new EntityCreature(configBallista);
    }

    #region Move
    public float CalculateHitByNode(GridTileNode node, bool isDiagonal)
    {
        float moveCost = isDiagonal ? 141 : 100;
        float penaltie = 0;
        var nativeHeroGround = ConfigData.TypeGround;
        var dataNode = ResourceSystem.Instance.GetLandscape(node.TypeGround);

        // Calculate penalti.
        if (dataNode.Penalties > 100 && !dataNode.typeGround.HasFlag(nativeHeroGround))
        {
            penaltie = (dataNode.Penalties * moveCost) / 100;
        }

        // check creatures.
        if (penaltie > 0)
        {
            var allCreatures = Data.Creatures.Where(t => t.Value != null);
            var creaturesNodeGround = allCreatures.Where(t => t.Value.ConfigAttribute.TypeGround.HasFlag(node.TypeGround));
            if (creaturesNodeGround.Count() == allCreatures.Count())
            {
                penaltie = 0;
            }
        }

        // Use bonus cancel penaltie.
        if (penaltie > 0 && Data.SSkills.ContainsKey(TypeSecondarySkill.Pathfinding))
        {
            penaltie = penaltie - Data.SSkills[TypeSecondarySkill.Pathfinding].value;
        }

        float val = penaltie == 0 ? moveCost : penaltie;
        // Debug.Log($"MoveCost=[{val}]/penaltie={penaltie}/dataNode.typeGround={dataNode.typeGround}/nativeHeroGround={nativeHeroGround}/{dataNode.typeGround.HasFlag(nativeHeroGround)}");
        return val;
    }

    public void ChangeHitHero(GridTileNode node, GridTileNode prevNode)
    {
        var isDiagonal = node.position.x != prevNode.position.x
            && node.position.y != prevNode.position.y;
        var val = CalculateHitByNode(node, isDiagonal);
        Data.mp -= val;

        // Debug.Log($"ChangeHitHero:::[node {node.position}]{ScriptableData.name}-{Data.hit}");
        if (Player != null && Player.DataPlayer.playerType != PlayerType.Bot)
        {
            OnChangeParamsActiveHero?.Invoke(this);
        }
    }

    public int GetExperience(int value)
    {
        // Effect Learning.
        int dop = 0;
        if (Data.SSkills.ContainsKey(TypeSecondarySkill.Learning))
        {
            dop = (int)(value * Data.SSkills[TypeSecondarySkill.Learning].value / 100f);
        }

        return value += dop;
    }

    public async UniTask ChangeExperience(int value = 0)
    {
        int val = GetExperience(value);
        Debug.Log($"Change experience::: old={value}, new={val}");
        Data.PSkills[TypePrimarySkill.Experience] += val;

        var quantityExperience = Data.PSkills[TypePrimarySkill.Experience];
        while (quantityExperience >= Data.nextLevel)
        {
            Data.level++;
            await UpdateLevel();
        }

        GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
    }

    public async UniTask ChangePrimarySkill(TypePrimarySkill typePrimarySkill, int value = 0)
    {
        Data.PSkills[typePrimarySkill] += value;
        await UniTask.Delay(1);
        GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
    }

    public void ChangeSecondarySkill(TypeSecondarySkill typeSecondarySkill, int level = 0)
    {
        // Debug.Log($"ChangeSecondarySkill::: {typeSecondarySkill}:{value}");
        var skillData = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Find(t => t.TypeTwoSkill == typeSecondarySkill);
        Data.SSkills[typeSecondarySkill] = new SkillDataItem()
        {
            level = level,
            value = skillData.Levels[level].value
        };
        // skill.RunEffect(Player, this);
        // SkillDataItem skillItem;
        // Data.SSkills.TryGetValue(typeSecondarySkill, out skillItem);
        // if (Data.SSkills.ContainsKey(typeSecondarySkill))
        // {
        //     skillItem.level = level;
        //     skillItem.value = skillData.Levels[level].value;
        //     // Data.SSData[typeSecondarySkill] = skill.Levels[level].value;
        // }
        // else
        // {
        //     skillItem.level = level;
        //     skillItem.value = skillData.Levels[level].value;
        //     // Data.SSData[typeSecondarySkill] = skill.Levels[level].value;
        // }


        GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
    }

    public async UniTask UpdateLevel()
    {
        var a = LevelManager.Instance.ConfigGameSettings.probabilityExperience.Evaluate(Data.level * 0.01f);

        var newNextLevel = Data.nextLevel + (int)(a * Data.nextLevel);
        Data.nextLevel = newNextLevel;
        // Debug.Log($"NextLevel::: [[level={Data.level}]|key={Data.level * 0.01f}]| NextLevel={Data.nextLevel} (keyValue={a})");

        // Generate secondary skills.
        var listSkills = GenerateSecondSkills();
        var primarySkill = GeneratePrimarySkill();

        int keySecondarySkill = 0;
        // Show dialog.
        if (Player.DataPlayer.playerType != PlayerType.Bot)
        {
            var dialogData = new DataDialogLevelHero()
            {
                sprite = this.ConfigData.MenuSprite,
                name = Data.name,
                gender = ConfigData.TypeGender.ToString(),
                level = string.Format("{0}, {1}", Data.level, ConfigData.ClassHero.name),
                SecondarySkills = listSkills,
                PrimarySkill = primarySkill
            };
            var dialogHeroInfo = new DialogHeroLevelOperation(dialogData);
            var result = await dialogHeroInfo.ShowAndHide();
            keySecondarySkill = result.keySecondarySkill;
        }
        else
        {
            keySecondarySkill = Random.Range(0, listSkills.Count);
        }

        await ChangePrimarySkill(primarySkill.TypeSkill, 1);

        var chooseSSkill = listSkills.ElementAt(keySecondarySkill); //.Where(t => t.Key == result.typeSecondarySkill).First();

        ChangeSecondarySkill(chooseSSkill.Key, chooseSSkill.Value);

        // Effect eagle eye
        if (Data.SSkills.ContainsKey(TypeSecondarySkill.EagleEye))
        {
            var sskillData = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
                .Find(t => t.TypeTwoSkill == TypeSecondarySkill.EagleEye);
            await sskillData.RunEffects(Player, Player.ActiveHero);
        }
    }

    private ScriptableAttributePrimarySkill GeneratePrimarySkill()
    {
        var potentialSkills = ConfigData.ClassHero.ChancesPrimarySkill
            .Where(t => t.minlevel <= Data.level && t.maxlevel >= Data.level)
            .Select(t => t.Item)
            .ToList();
        return Helpers.GetProbabilityItem<ScriptableAttributePrimarySkill>(potentialSkills).Item;
    }

    private SerializableDictionary<TypeSecondarySkill, int> GenerateSecondSkills()
    {
        var result = new SerializableDictionary<TypeSecondarySkill, int>();

        // Find new secondary skill.
        var potentialSkills = ConfigData.ClassHero.ChancesSecondarySkill
            .Where(t => !Data.SSkills.Keys.Contains(t.Item.TypeTwoSkill))
            .ToList();
        var firstSkill = Helpers.GetProbabilityItem<ScriptableAttributeSecondarySkill>(potentialSkills);
        result.Add(firstSkill.Item.TypeTwoSkill, 0);
        potentialSkills.RemoveAt(firstSkill.index);

        // Find not expert exist secondary skill.
        var notExpertSSkills = Data.SSkills
            .Where(t => t.Value.level < 2)
            .ToList();
        if (notExpertSSkills.Count > 0)
        {
            var secondarySkill = notExpertSSkills[Random.Range(0, notExpertSSkills.Count)];
            result.Add(secondarySkill.Key, secondarySkill.Value.level + 1);
        }
        else
        {
            var secondSkill = Helpers.GetProbabilityItem<ScriptableAttributeSecondarySkill>(potentialSkills);
            result.Add(secondSkill.Item.TypeTwoSkill, 0);
        }

        return result;
    }

    public void SetPositionHero(GridTileNode newNode)
    {
        // MapObjectGameObject.transform.position = newPosition;// + new Vector3(.5f, .5f);
        MapObject.SetPosition(newNode.position);
        if (Player != null)
        {
            //SetPositionCamera(newPosition);
            // GameManager.Instance.MapManager.SetColorForTile(newPosition, Color.cyan);
            SetClearSky(newNode);
        }
    }

    public void FastMoveHero(Vector3Int newPosition)
    {

        var newNode = GameManager.Instance
            .MapManager
            .GridTileHelper()
            .GetNode(newPosition);

        SetPositionHero(newNode);

        SetGuestForNode(newNode);
        MapObject.SetPositionCamera(newPosition);
        //Move GameObject.
        MapObject.MapObjectGameObject.gameObject.transform.position = newPosition;

    }

    public void SetGuestForNode(GridTileNode newNode)
    {
        MapObject.OccupiedNode.SetAsGuested(null);
        if (MapObject.OccupiedNode.OccupiedUnit == MapObject)
        {
            MapObject.OccupiedNode.SetOcuppiedUnit(null);
        }

        SetPositionHero(newNode);

        MapObject.OccupiedNode = newNode;
        MapObject.OccupiedNode.SetAsGuested(MapObject);
        // if (MapObject.OccupiedNode.OccupiedUnit == null)
        // {
        //     MapObject.OccupiedNode.SetOcuppiedUnit(MapObject);
        // }
    }

    public void SetClearSky(GridTileNode startNode)
    {
        int scoutingValue = 0;
        if (Data.SSkills.ContainsKey(TypeSecondarySkill.Scouting))
            scoutingValue = Data.SSkills[TypeSecondarySkill.Scouting].value;

        List<GridTileNode> noskyNodes
            = GameManager.Instance.MapManager.DrawSky(startNode, scoutingValue);
        Player.SetNosky(noskyNodes);
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
        if (Data.path.Count > 0 && Data.mp > 0)
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
        // GameManager.Instance.ChangeState(GameState.CreatePathHero);
        // OnChangeParamsActiveHero?.Invoke(this);
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
        if (path != null && path.Count > 0)
        {
            this.SetPathHero(path);
        }
        OnChangeParamsActiveHero?.Invoke(this);
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
        while (Data.mp > 0 && countProbe < countProbePath)
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

        // if (Data.mp > 0)
        // {
        //     FindPathForHero(Player.DataPlayer.PlayerDataReferences.ListTown[0].MapObject.OccupiedNode.position, true);
        //     await StartMove();
        // }
    }

    public void AIFindResource()
    {
        if (Data.Creatures.Where(t => t.Value != null).Count() == 0)
        {
            return;
        }

        // start data.
        int countCheckNodes = 0;

        var flag = ((NoskyMask)(1 << LevelManager.Instance.ActivePlayer.DataPlayer.team));
        //Find
        var potentialPoints = GameManager.Instance
            .MapManager
            .gridTileHelper
            .GetNeighboursAtDistance(MapObject.OccupiedNode, 25, true)
            .Where(t =>
                LevelManager.Instance.Level.nosky.ContainsKey(t.position)
                && LevelManager.Instance.Level.nosky[t.position].HasFlag(flag)
                && !t.StateNode.HasFlag(StateNode.Disable)
                && t != MapObject.OccupiedNode
                && (
                    !t.StateNode.HasFlag(StateNode.Occupied)
                    ||
                    (t.StateNode.HasFlag(StateNode.Occupied) && !t.StateNode.HasFlag(StateNode.Protected))
                )
            )
            .OrderBy(t => GameManager.Instance
                .MapManager
                .gridTileHelper
                .GetDistanceBetweeenPoints(MapObject.Position, t.position)
            ).ToList();

        // Debug.Log($"potentialPoints={potentialPoints.Count}");
        // Dictionary<BaseEntity, List<GridTileNatureNode>> potentialObjects = new();
        // foreach(var node in potentialPoints) {
        //     if (node.StateNode.HasFlag(StateNode.Empty)) {
        //         potentialObjects.Add()
        //     }
        // }

        while (potentialPoints.Count > 0)
        {
            var node = potentialPoints[0];
            int streightNeutralArmy = 0;

            // if visited another hero from team.
            if (node.OccupiedUnit != null
                && node.OccupiedUnit.Entity != null
                && node.OccupiedUnit.Entity.Player != null
                && node.OccupiedUnit.Entity.Player.DataPlayer.team == Player.DataPlayer.team
                // && node.StateNode.HasFlag(StateNode.Visited)
                )
            {
                potentialPoints.RemoveAt(0);
                continue;
            }

            // get streight protected unit.
            if (
                streightNeutralArmy == 0
                && node.ProtectedUnit != null
                && node.ProtectedUnit.Entity is EntityCreature
            )
            {
                streightNeutralArmy = ((EntityCreature)node.ProtectedUnit.Entity).totalAI;
            }

            // get streight defenders.
            if (
                streightNeutralArmy == 0
                && node.OccupiedUnit != null
                && node.OccupiedUnit.Entity is EntityMapObject
                && ((EntityMapObject)node.OccupiedUnit.Entity).Data.Defenders != null
                && ((EntityMapObject)node.OccupiedUnit.Entity).Data.Defenders.Count > 0
            )
            {
                streightNeutralArmy = ((EntityMapObject)node.OccupiedUnit.Entity).Data.Defenders.Select(t => t.Value.totalAI).Sum();
            }

            var streightHeroArmy = Streight * Data.Creatures.Select(t => t.Value?.totalAI).Sum();

            if (streightNeutralArmy > 0 && ((float)streightNeutralArmy / (float)streightHeroArmy >= ConfigData.ClassHero.levelAgression))
            {
                potentialPoints.RemoveAt(0);
                continue;
            }

            // Debug.Log($"streightNeutralArmy={streightNeutralArmy}");
            // Debug.Log($"streightHeroArmy={streightHeroArmy}");
            // Debug.Log($"levelAgr={ConfigData.ClassHero.levelAgression}, koof={(float)streightNeutralArmy / (float)streightHeroArmy}");
            // Debug.Log($"node={node}/{Player.ActiveHero.MapObject.OccupiedNode}");

            if (
                (node.OccupiedUnit != null
                // node.StateNode.HasFlag(StateNode.Occupied)
                // && !node.StateNode.HasFlag(StateNode.Protected)
                // && !node.StateNode.HasFlag(StateNode.Guested)
                // && !node.StateNode.HasFlag(StateNode.Empty)
                // && node.OccupiedUnit.Entity.Player != Player
                && !node.StateNode.HasFlag(StateNode.Visited)
                && node != MapObject.OccupiedNode) // || countCheckNodes > 20
                )
            {
                var path = FindPathForHero(node.position, true);
                // Debug.Log($"path={path?.Count}");
                if (path != null)
                {
                    break;
                }
                else
                {
                    potentialPoints.RemoveAt(0);
                }
            }
            else
            {
                potentialPoints.RemoveAt(0);
            }
            countCheckNodes++;
            // Debug.Log($"countCheckNodes={countCheckNodes}");
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
