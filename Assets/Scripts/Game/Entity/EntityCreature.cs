using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityCreature : BaseEntity
{
    [SerializeField] public DataCreature Data = new DataCreature();
    public ScriptableEntityCreature ConfigData => (ScriptableEntityCreature)ScriptableData;

    public EntityCreature(
        ScriptableEntityCreature configData,
        SaveDataUnit<DataCreature> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            idObject = ScriptableData.idObject;
            // if (configData == null)
            // {
            //     List<ScriptableEntityCreature> list = ResourceSystem.Instance
            //         .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
            //         .ToList();
            //     ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            // }
            // else
            // {
            //     ScriptableData = configData;
            // }

            Data.value = 1;
            // SetValueCreature();
        }
        else
        {
            if (saveData.idObject != "")
            {
                ScriptableData = ResourceSystem.Instance
                    .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                    .Where(t => t.idObject == saveData.idObject)
                    .First();
            }
            Data = saveData.data;
            idObject = saveData.idObject;
            idUnit = saveData.idUnit;
        }
    }

    public void SetValueCreature(int quantityCreature)
    {
        Data.value = quantityCreature;
        // // Data.protectedNode = ProtectedNode.position;
        // if (ProtectedNode != null)
        // {
        //    BaseEntity protectedUnit = ProtectedNode.OccupiedUnit;
        //    Data.value = protectedUnit.ScriptableData.level + (protectedUnit.ScriptableData.level * 2) - (this.ScriptableData.level * 2);

        //    // Debug.Log($"Warrior {name} protectedNode as :::name[{protectedUnit.ScriptableData.name}]level[{protectedUnit.ScriptableData.level}]");

        // }
    }

    public override void SetPlayer(Player player)
    {
        // ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        ScriptableEntityCreature configData = (ScriptableEntityCreature)ScriptableData;
        // configData.OnDoHero(ref player, this);
        MapObjectGameObject.DestroyGameObject();
        DestroyMapGameObject();
    }

    public void SetOccupiedNode(GridTileNode node)
    {
        OccupiedNode = node;
    }

    public void SetProtectedNode(GridTileNode protectedNode)
    {
        ProtectedNode = protectedNode;
        Data.protectedNode = protectedNode.position;
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.creatures.Add(sdata);
    }

    #endregion
}
