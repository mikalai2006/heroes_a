using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityCreature : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataCreature Data = new DataCreature();
    public ScriptableEntityCreature ConfigData => (ScriptableEntityCreature)ScriptableData;

    public EntityCreature(GridTileNode node, SaveDataUnit<DataCreature> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityCreature> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            Data.quantity = 10;
            OnChangeQuantityWarrior();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }

        base.Init(ScriptableData, node);
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

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.creatures.Add(sdata);
    }
    #endregion
}
