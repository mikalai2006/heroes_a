using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[System.Serializable]
public struct DataResourceMapObject
{
    public int idPlayer;
    public List<DataResourceValue> Value;
    public TypeWork TypeWork;
}
public abstract class BaseResourceMapObject : BaseMapObject, IDataPlay, IDialogMapObjectOperation
{
    private DataResourceMapObject Data;

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {

        var listValue = new List<DataDialogItem>(Data.Value.Count);
        for (int i = 0; i < Data.Value.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = Data.Value[i].Resource.MenuSprite,
                Value = Data.Value[i].value
            });
        }

        var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Description = t.Text.visit_ok,
            Header = t.Text.title,
            // Sprite = this.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);
        DataResultDialog result = await OnTriggeredHero();
        if (result.isOk)
        {
            SetPlayer(player);
        }
        else
        {
            // Click cancel.
        }
    }
    public void SetPlayer(Player player)
    {
        // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // player.ChangeResource(dataResource.TypeResource, value);
        for (int i = 0; i < Data.Value.Count; i++)
        {
            player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        }
        if (Data.TypeWork == TypeWork.One) Destroy(gameObject);
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data = new DataResourceMapObject();
        var DataResource = (ScriptableMapObject)data;
        SetData();
    }

    private void SetData()
    {
        ScriptableMapObject scriptDataObject = ResourceSystem.Instance.GetUnit<ScriptableMapObject>(idObject);

        Data.Value = new List<DataResourceValue>();
        Data.TypeWork = scriptDataObject.TypeWork;

        GroupResource groupResource = scriptDataObject
            .Resources[Random.Range(0, scriptDataObject.Resources.Count)];
        for (int i = 0; i < groupResource.ListVariant.Count; i++)
        {
            int stepsValue = groupResource.ListVariant[i].maxValue / groupResource.ListVariant[i].step;
            int randomIndexValue = Random.Range(1, stepsValue);
            Data.Value.Add(new DataResourceValue()
            {
                typeResource = groupResource.ListVariant[i].Resource.TypeResource,
                value = groupResource.ListVariant[i].step * randomIndexValue,
                Resource = groupResource.ListVariant[i].Resource
            });

        }
    }

    public override void OnNextDay()
    {
        base.OnNextDay();
        SetData();
    }

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }


    public void SaveDataPlay(ref DataPlay data)
    {
        // var sdata = SaveUnit(Data);
        // data.Units.resources.Add(sdata);
    }
}
