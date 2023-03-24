using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWarriors : UnitBase, IDataPlay
{
    public DataWarrior Data;

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);
        Data = new DataWarrior();
        //ScriptableWarriors dataWarrior = (ScriptableWarriors)data;
        //Data.quantity = dataWarrior.level * ;
        // OnChangeQuantityWarrior();
    }

    public override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        OccupiedNode.SetProtectedNeigbours(null);

        OccupiedNode.SetOcuppiedUnit(null);

        Destroy(gameObject.gameObject);

        Debug.LogWarning("Show dialog war!");
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
