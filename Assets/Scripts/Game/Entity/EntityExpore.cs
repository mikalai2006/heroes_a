using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public struct DataExplore
{

}
[Serializable]
public class EntityExpore : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataExplore Data = new DataExplore();
    public ScriptableEntityExplore ConfigData => (ScriptableEntityExplore)ScriptableData;
    public EntityExpore(
        GridTileNode node,
        ScriptableEntityExplore configData,
        SaveDataUnit<DataExplore> saveData = null)
    {
        if (saveData == null)
        {
            // List<ScriptableEntityExplore> list = ResourceSystem.Instance
            //     .GetEntityByType<ScriptableEntityExplore>(TypeEntity.Explore)
            //     .ToList();
            ScriptableData = configData; // list[UnityEngine.Random.Range(0, list.Count)];
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Explore)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);

        // TODO PERK
        List<GridTileNode> noskyNodes
            = GameManager.Instance.MapManager.DrawSky(OccupiedNode.position, 10);

        player.SetNosky(noskyNodes);
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.explorers.Add(sdata);
    }
    #endregion
}
