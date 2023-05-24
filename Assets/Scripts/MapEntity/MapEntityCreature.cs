using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

using Random = System.Random;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
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

        // TODO ARENA

        var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
        {
            hero = player.ActiveHero,
            creature = entityCreature
        });
        var result = await loadingOperations.ShowHide();

        await player.ActiveHero.ChangeExperience(entityCreature.ConfigAttribute.CreatureParams.HP * entityCreature.Data.value);

        MapObject.DoHero(player);

        Destroy(gameObject);
    }
}
