using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
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
        MapObjectClass.ProtectedNode.DisableProtectedNeigbours(MapObjectClass);

        // TODO ARENA

        Destroy(gameObject);
        MapObjectClass.SetPlayer(player);
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var creature = (EntityCreature)MapObjectClass;
        ScriptableEntityCreature configData = (ScriptableEntityCreature)MapObjectClass.ScriptableData;

        string nameText = Helpers.GetStringNameCountWarrior(creature.Data.value);
        LocalizedString stringCountWarriors = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, nameText);

        // var title = !configData.title.IsEmpty
        //     ? configData.title.GetLocalizedString(creature.Data.value)
        //     : "No_LANG";
        var dataPlural = new Dictionary<string, int> { { "value", 0 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            configData.title,
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
            Sprite = configData.MenuSprite,
            TypeCheck = TypeCheck.Default
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
