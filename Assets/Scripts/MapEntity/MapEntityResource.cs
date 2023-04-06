using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization.Settings;

public class MapEntityResource : BaseMapEntity, IDialogMapObjectOperation
{

    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

        var mapObjectClass = (EntityResource)MapObjectClass;
        mapObjectClass.SetData();
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntityResource entity = (EntityResource)MapObjectClass;
        var listValue = new List<DataDialogItem>(entity.Data.Value.Count);
        for (int i = 0; i < entity.Data.Value.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = entity.Data.Value[i].Resource.MenuSprite,
                Value = entity.Data.Value[i].value
            });
        }

        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            Description = MapObjectClass.ScriptableData.DialogText.VisitOk.GetLocalizedString(),
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

        // // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // // player.ChangeResource(dataResource.TypeResource, value);
        // for (int i = 0; i < Data.Value.Count; i++)
        // {
        //     player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        // }
        // if (Data.TypeWork == TypeWorkPerk.One)
        // {
        //     //ScriptableData.MapPrefab.ReleaseInstance(gameObject);
        //     Destroy(gameObject);
        // }
    }



    public override void OnNextDay()
    {
        base.OnNextDay();
        var mapObjectClass = (EntityResource)MapObjectClass;
        mapObjectClass.SetData();
    }

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}

    // public void LoadDataPlay(DataPlay data)
    // {
    //     //throw new System.NotImplementedException();
    // }


    // public void SaveDataPlay(ref DataPlay data)
    // {
    //     var sdata = SaveUnit(Data);
    //     data.Units.resources.Add(sdata);
    // }
}
