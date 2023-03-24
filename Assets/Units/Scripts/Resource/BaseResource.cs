using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DataResource
{
    public int idPlayer;
}

public abstract class BaseResource : BaseMapObject, IDataPlay
{
    private int _value = 0;
    public DataResource Data;
    public override void OnGoHero(Player player)
    {
        base.OnGoHero(player);
        SetPlayer(player);
    }
    public void SetPlayer(Player player)
    {
        ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        player.ChangeResource(dataResource.TypeResource, value);

        if (dataScriptable.TypeWork == TypeWork.One) Destroy(gameObject);
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data = new DataResource();
        var DataResource = (ScriptableResource)data;
        SetData();
    }

    private void SetData()
    {
        var datax = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);
        var ListValue = datax.ListResource[Random.Range(0, datax.ListResource.Count)].listValue;
        _value = ListValue[Random.Range(0, ListValue.Length)];

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
        var sdata = SaveUnit(Data);
        data.Units.resources.Add(sdata);
    }
}
