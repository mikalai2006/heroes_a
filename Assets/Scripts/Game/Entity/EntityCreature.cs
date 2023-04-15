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

            Data.value = 10;
            OnChangeQuantityWarrior();
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

    public void OnChangeQuantityWarrior()
    {
        //Data.protectedNode = ProtectedNode.position;
        //if (ProtectedNode != null)
        //{
        //    UnitBase protectedUnit = ProtectedNode.OccupiedUnit;
        //    Data.quantity = protectedUnit.ScriptableData.level + (protectedUnit.ScriptableData.level * 2) - (this.ScriptableData.level * 2);

        //    // Debug.Log($"Warrior {name} protectedNode as :::name[{protectedUnit.ScriptableData.name}]level[{protectedUnit.ScriptableData.level}]");

        //}
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
