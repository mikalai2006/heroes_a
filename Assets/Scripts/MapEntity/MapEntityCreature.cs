using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

using Random = UnityEngine.Random;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    private EntityCreature creature => (EntityCreature)MapObject.Entity;
    private ScriptableAttributeCreature creatureConfig => creature.ConfigAttribute;

    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
    }

    public async override void OnClick(InputAction.CallbackContext context)
    {
        if (
            context.performed
            && !_inputManager.ClickedOnUi()
            )
        {
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
            if (!rayHit.collider) return;
            if (rayHit.collider.gameObject == gameObject) // _model.gameObject
            {
                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    var activeHero = LevelManager.Instance.ActivePlayer.ActiveHero;
                    if (activeHero != null)
                    {

                        Vector3 posObject = transform.position;
                        if (activeHero.Data.path.Count > 0 && activeHero.Data.path[activeHero.Data.path.Count - 1].position == posObject)
                        {
                            GameManager.Instance.ChangeState(GameState.StartMoveHero);
                        }
                        else
                        {
                            if (posObject != null)
                            {
                                Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
                                activeHero.FindPathForHero(end, true);
                            }
                        }
                    }
                    else
                    {
                        var dialogData = new DataDialogHelp()
                        {
                            Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                            Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "needchoosehero").GetLocalizedString(),
                        };

                        var dialogWindow = new DialogHelpProvider(dialogData);
                        await dialogWindow.ShowAndHide();
                    }
                }
                else if (context.interaction is HoldInteraction)
                {
                    _inputManager.Disable();
                    string title = GetTitle();
                    LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "armygroup_desc")
                    {
                        { "name", new StringVariable { Value = "<color=#FFFFAB>" + title + "</color>" } },
                    };

                    var dialogData = new DataDialogHelp()
                    {
                        Header = title,
                        Description = message.GetLocalizedString(),
                        Sprite = creatureConfig.MenuSprite
                    };

                    var dialogWindow = new DialogHelpProvider(dialogData);
                    await dialogWindow.ShowAndHide();
                    _inputManager.Enable();
                }
            }
        }
    }

    private string GetTitle()
    {
        string nameText = Helpers.GetStringNameCountWarrior(creature.Data.value);
        LocalizedString stringCountWarriors = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, nameText);

        var dataPlural = new Dictionary<string, int> { { "value", 0 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            creature.ConfigAttribute.Text.title,
            arguments,
            dataPlural
            );
        var title = string.Format("{0}({1}) {2}", stringCountWarriors.GetLocalizedString(), creature.Data.value, titlePlural);


        return title;
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var creature = (EntityCreature)_mapObject.Entity;

        // Check diplomacy
        var activeHero = LevelManager.Instance.ActivePlayer.ActiveHero;
        if (activeHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Diplomacy))
        {
            var hpCreature = creature.Data.value * creature.ConfigAttribute.CreatureParams.HP;

            var hpArmy = 0;
            foreach (var cr in activeHero.Data.Creatures)
            {
                if (cr.Value != null)
                    hpArmy += cr.Value.Data.value * cr.Value.ConfigAttribute.CreatureParams.HP;
            }
            Debug.Log($"hpArmy={hpArmy}/hpCreature={hpCreature}");
        }

        string title = GetTitle();
        LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "army_attack")
        {
            { "name", new StringVariable { Value = "<color=#FFFFAB>" + title + "</color>" } },
        };
        var dialogData = new DataDialogMapObject()
        {
            Header = title,
            Description = message.GetLocalizedString(),
            Sprite = creature.ConfigAttribute.MenuSprite,
            TypeCheck = TypeCheck.Default
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }


    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            DataResultDialog result = await OnTriggeredHero();

            if (result.isOk)
            {
                await OnHeroGo(player);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
            await OnHeroGo(player);
        }
    }

    private async UniTask OnHeroGo(Player player)
    {
        var entityCreature = (EntityCreature)_mapObject.Entity;

        if (player.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.EagleEye))
        {
            // Effect eagle eye
            var sskillData = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
                .Find(t => t.TypeTwoSkill == TypeSecondarySkill.EagleEye);
            await sskillData.RunEffects(player, player.ActiveHero);
        }

        MapObject.ProtectedNode.DisableProtectedNeigbours(_mapObject);
        MapObject.OccupiedNode.DisableProtectedNeigbours(_mapObject);


        // Get setting for arena.
        var arenaSetting = LevelManager.Instance.ConfigGameSettings.ArenaSettings
            .Where(t => t.NativeGround.typeGround == MapObject.OccupiedNode.TypeGround)
            .ToList();
        // TODO ARENA
        var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
        {
            heroAttacking = player.ActiveHero,
            creature = entityCreature,
            town = null,
            ArenaSetting = arenaSetting[Random.Range(0, arenaSetting.Count())]
        });
        var result = await loadingOperations.ShowHide();

        Debug.Log("End battle");

        // await player.ActiveHero.ChangeExperience(entityCreature.ConfigAttribute.CreatureParams.HP * entityCreature.Data.value);

        MapObject.DoHero(player);

        Destroy(gameObject);
    }
}
