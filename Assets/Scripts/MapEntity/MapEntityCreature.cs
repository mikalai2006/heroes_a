using Cysharp.Threading.Tasks;

using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            MapObjectClass.OccupiedNode.SetProtectedNeigbours(null);

            MapObjectClass.OccupiedNode.SetOcuppiedUnit(null);

            Destroy(gameObject);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {

        // var creature = (EntityCreature)MapObjectClass;
        // string nameText = Helpers.GetStringNameCountWarrior(creature.Data.quantity);
        // LocalizedString stringCountWarriors = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, nameText);

        // var title = MapObjectClass.ScriptableData.Text.title.GetLocalizedString();
        // LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "army_attack")
        // {
        //     { "name", new StringVariable { Value = "<color=#FFFFAB>" + title + "</color>" } },
        // };

        var dialogData = new DataDialog()
        {
            // Header = string.Format("{0} {1}", stringCountWarriors.GetLocalizedString(), title),
            // Description = message.GetLocalizedString(),
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
