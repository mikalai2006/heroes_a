using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntityMapObject : BaseEntity
{
    [SerializeField]
    public DataEntityMapObject Data = new DataEntityMapObject();
    public ScriptableEntityMapObject ConfigData => (ScriptableEntityMapObject)ScriptableData;
    public EntityMapObject(
        ScriptableEntityMapObject configData,
        SaveDataUnit<DataEntityMapObject> saveData = null
        )
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            _idObject = ScriptableData.idObject;
            SetData();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject)
                .First();

            Data = saveData.data;
            _idEntity = saveData.idEntity;
            _idObject = saveData.idObject;
            Effects = saveData.Effects;
        }
    }

    public void SetData()
    {
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        configData.SetData(this);
    }

    public override void SetPlayer(Player player)
    {
        // ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        // // configData.RunHero(ref player, this);
        // // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // // player.ChangeResource(dataResource.TypeResource, value);
        // // for (int i = 0; i < Data.Value.Count; i++)
        // // {
        // //     player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        // // }
        // if (configData.TypeWorkObject == TypeWorkObject.One)
        // {
        //     List<GridTileNode> nodes
        //         = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(OccupiedNode, configData.RulesInput);
        //     foreach (var node in nodes)
        //     {
        //         node.RemoveStateNode(StateNode.Input);
        //     }
        //     DestroyEntity();
        // }
        // else
        // {
        //     OccupiedNode.ChangeStatusVisit(true);
        // }
    }

    #region SaveLoadData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.entityMapObjects.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataEntityMapObject
{
}