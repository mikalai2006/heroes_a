using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityCreature : BaseMapEntity, IDialogMapObjectOperation
{
    public DataCreature Data;

    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);

        Data = new DataCreature();
        Data.quantity = 10;
        //ScriptableWarriors dataWarrior = (ScriptableWarriors)data;
        //Data.quantity = dataWarrior.level * ;
        // OnChangeQuantityWarrior();
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
        // var listValue = new List<DataDialogItem>();
        // listValue.Add(new DataDialogItem()
        // {
        //     Sprite = ScriptableData.MenuSprite,
        //     Value = Data.quantity
        // });
        string nameText = Helpers.GetStringNameCountWarrior(Data.quantity);
        LocalizedString stringCountWarriors = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, nameText);

        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var title = MapObjectClass.ScriptableData.Text.title.GetLocalizedString();
        LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "army_attack")
        {
            { "name", new StringVariable { Value = "<color=#FFFFAB>" + title + "</color>" } },
        };

        var dialogData = new DataDialog()
        {
            Header = string.Format("{0} {1}", stringCountWarriors.GetLocalizedString(), title),
            Description = message.GetLocalizedString(),
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public void OnChangeQuantityWarrior()
    {
        Data.protectedNode = MapObjectClass.ProtectedNode.position;
        //if (ProtectedNode != null)
        //{
        //    UnitBase protectedUnit = ProtectedNode.OccupiedUnit;
        //    Data.quantity = protectedUnit.ScriptableData.level + (protectedUnit.ScriptableData.level * 2) - (this.ScriptableData.level * 2);

        //    // Debug.Log($"Warrior {name} protectedNode as :::name[{protectedUnit.ScriptableData.name}]level[{protectedUnit.ScriptableData.level}]");

        //}
    }

}
