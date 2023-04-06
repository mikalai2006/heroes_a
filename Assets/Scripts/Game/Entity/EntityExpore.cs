using System.Collections.Generic;
using System.Linq;
using System;

public class EntityExpore : BaseEntity, IDataPlay
{
    public ScriptableEntityExplore ConfigData => (ScriptableEntityExplore)ScriptableData;
    public EntityExpore(GridTileNode node)
    {
        List<ScriptableEntityExplore> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityExplore>(TypeEntity.Explore)
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
