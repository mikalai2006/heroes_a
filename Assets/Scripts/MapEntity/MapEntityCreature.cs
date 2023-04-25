using Cysharp.Threading.Tasks;

using UnityEngine.Localization;
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

        var title = !configData.title.IsEmpty ? configData.title.GetLocalizedString() : "No_LANG";
        LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "army_attack")
        {
            { "name", new StringVariable { Value = "<color=#FFFFAB>" + title + "</color>" } },
        };

        var dialogData = new DataDialogMapObject()
        {
            Header = string.Format("{0} {1}", stringCountWarriors.GetLocalizedString(), title),
            Description = message.GetLocalizedString(),
            Sprite = configData.MenuSprite,
            TypeCheck = TypeCheck.Default
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
