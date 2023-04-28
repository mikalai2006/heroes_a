using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var creature = (EntityCreature)_mapObject.Entity;

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
                OnHeroGo(player);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
            OnHeroGo(player);
        }
    }

    private void OnHeroGo(Player player)
    {
        MapObject.ProtectedNode.DisableProtectedNeigbours(_mapObject);

        // TODO ARENA

        MapObject.DoHero(player);

        Destroy(gameObject);
    }
}
