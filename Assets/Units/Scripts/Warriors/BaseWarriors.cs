using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization.Settings;

public class BaseWarriors : UnitBase, IDataPlay, IDialogMapObjectOperation
{
    public DataWarrior Data;

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);
        Data = new DataWarrior();
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
            OccupiedNode.SetProtectedNeigbours(null);

            OccupiedNode.SetOcuppiedUnit(null);

            Destroy(gameObject);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var listValue = new List<DataDialogItem>();
        listValue.Add(new DataDialogItem()
        {
            Sprite = ScriptableData.MenuSprite,
            Value = Data.quantity
        });

        var t = HelperLanguage.GetLocaleText(this.ScriptableData);
        var dialogData = new DataDialog()
        {
            Description = t.Text.title,
            Header = t.Text.description,
            Sprite = this.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public void OnChangeQuantityWarrior()
    {
        //if (ProtectedNode != null)
        //{
        //    UnitBase protectedUnit = ProtectedNode.OccupiedUnit;
        //    Data.quantity = protectedUnit.ScriptableData.level + (protectedUnit.ScriptableData.level * 2) - (this.ScriptableData.level * 2);

        //    // Debug.Log($"Warrior {name} protectedNode as :::name[{protectedUnit.ScriptableData.name}]level[{protectedUnit.ScriptableData.level}]");

        //}
    }

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        //Debug.Log($"Save Warrior {name}");
        var sdata = SaveUnit(Data);
        data.Units.warriors.Add(sdata);
    }
}
