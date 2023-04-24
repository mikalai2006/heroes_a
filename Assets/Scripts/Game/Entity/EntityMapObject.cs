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
            idObject = ScriptableData.idObject;
            SetData();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }
    }

    public void SetData()
    {
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        configData.SetData(this);
        // Data.AttributeValues = new List<DataEntityResourceValues>();
        // if (scriptData.Attributes.Count > 0)
        // {
        //     var group = Helpers.GetProbabilityItem<GroupAttributes>(scriptData.Attributes);
        //     Data.index = group.index;
        //     if (group.Item.Artifacts.Artifact.Count > 0)
        //     {
        //         if (group.Item.Artifacts.isOne)
        //         {
        //             var artifact = group.Item.Artifacts
        //                 .Artifact[Random.Range(0, group.Item.Artifacts.Artifact.Count)];
        //             Data.AttributeValues.Add(new DataEntityResourceValues()
        //             {
        //                 Attribute = artifact,
        //                 TypeAttribute = artifact.TypeAttribute,
        //                 idObject = artifact.idObject,
        //             });
        //         }
        //         else
        //         {
        //             foreach (var result in group.Item.Artifacts.Artifact)
        //             {
        //                 Data.AttributeValues.Add(new DataEntityResourceValues()
        //                 {
        //                     Attribute = result,
        //                     TypeAttribute = result.TypeAttribute,
        //                     idObject = result.idObject,
        //                 });
        //             }
        //         }
        //     }

        //     foreach (var result in group.Item.Resources)
        //     {
        //         int stepsValue = (result.maxValue - result.minValue) / result.step;
        //         int randomIndexValue = Random.Range(1, stepsValue);

        //         Data.AttributeValues.Add(new DataEntityResourceValues()
        //         {
        //             Attribute = result.Resource,
        //             value = Helpers.GenerateValueByRangeAndStep(
        //                 result.minValue,
        //                 result.maxValue,
        //                 result.step
        //             ),
        //             TypeAttribute = result.Resource.TypeAttribute,
        //             idObject = result.Resource.idObject,
        //         });
        //     }

        //     foreach (var result in group.Item.PrimarySkills)
        //     {
        //         Data.AttributeValues.Add(new DataEntityResourceValues()
        //         {
        //             Attribute = result.Skill,
        //             TypeAttribute = result.Skill.TypeAttribute,
        //             value = result.Value,
        //             idObject = result.Skill.idObject,
        //         });
        //     }

        //     foreach (var result in group.Item.SecondarySkills)
        //     {
        //         Data.AttributeValues.Add(new DataEntityResourceValues()
        //         {
        //             Attribute = result.SecondarySkill,
        //             value = result.value,
        //             TypeAttribute = result.SecondarySkill.TypeAttribute,
        //             idObject = result.SecondarySkill.idObject,
        //         });
        //     }
        // }
    }

    public override void SetPlayer(Player player)
    {
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        // configData.RunHero(ref player, this);
        // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // player.ChangeResource(dataResource.TypeResource, value);
        // for (int i = 0; i < Data.Value.Count; i++)
        // {
        //     player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        // }
        if (configData.TypeWorkObject == TypeWorkObject.One)
        {
            List<GridTileNode> nodes
                = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(OccupiedNode, configData.RulesInput);
            foreach (var node in nodes)
            {
                node.RemoveStateNode(StateNode.Input);
            }
            MapObjectGameObject.DestroyGameObject();
            DestroyMapGameObject();
        }
        else
        {
            OccupiedNode.ChangeStatusVisit(true);
        }
    }

    #region SaveLoadData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.mapObjects.Add(sdata);

        // ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        // foreach (var ef in DataEffects.Effects)
        // {
        //     ef.Effect.Save(this, ref data);
        // }
    }
    #endregion
}

[System.Serializable]
public struct DataEntityMapObject
{
}