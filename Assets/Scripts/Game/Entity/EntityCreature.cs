using System.Collections.Generic;
using System.Linq;
using System;


public class EntityCreature : BaseEntity, IDataPlay
{
    public ScriptableEntityCreature ConfigData => (ScriptableEntityCreature)ScriptableData;

    public EntityCreature(ScriptableEntityCreature data, GridTileNode node)
    {
        List<ScriptableEntityCreature> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        base.Init(ScriptableData, node);
    }

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        // var sdata = SaveUnit(Data);
        // data.Units.warriors.Add(sdata);
    }
}
