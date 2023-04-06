using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class EntityResource : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataResourceMapObject Data = new DataResourceMapObject();
    public ScriptableEntityMapResource ConfigData => (ScriptableEntityMapResource)ScriptableData;
    public EntityResource(
        GridTileNode node,
        List<TypeWorkPerk> listTypeWork = null,
        SaveDataUnit<DataResourceMapObject> saveData = null
        )
    {
        if (saveData == null)
        {
            List<ScriptableEntityMapResource> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapResource>(TypeEntity.MapResource)
                .Where(t => listTypeWork.Contains(t.TypeWorkPerk))
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            SetData();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapResource>(TypeEntity.MapResource)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }

        base.Init(ScriptableData, node);
    }

    public void SetData()
    {
        ScriptableEntityMapResource scriptData = (ScriptableEntityMapResource)ScriptableData;

        Data.Value = new List<DataResourceValue>();
        Data.TypeWork = scriptData.TypeWorkPerk;//.TypeWorkMapObject;

        GroupResource groupResource = scriptData
            .Resources[Random.Range(0, scriptData.Resources.Count)];
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

    public override void SetPlayer(Player player)
    {
        // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // player.ChangeResource(dataResource.TypeResource, value);
        for (int i = 0; i < Data.Value.Count; i++)
        {
            player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        }
        if (Data.TypeWork == TypeWorkPerk.One)
        {
            //ScriptableData.MapPrefab.ReleaseInstance(gameObject);
            MapObjectGameObject.DestroyGameObject();
        }
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.resources.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataResourceMapObject
{
    public int idPlayer;
    public List<DataResourceValue> Value;
    public TypeWorkPerk TypeWork;
}

[System.Serializable]
public struct DataResourceValue
{
    public TypeResource typeResource;
    public int value;

    public ScriptableEntityResource Resource;

}
