using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public struct DataExplore
{

}
[Serializable]
public class EntityExpore : BaseEntity
{
    [SerializeField] public DataExplore Data = new DataExplore();
    public ScriptableEntityExplore ConfigData => (ScriptableEntityExplore)ScriptableData;
    public EntityExpore(
        ScriptableEntityExplore configData,
        SaveDataUnit<DataExplore> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            _idEntity = ScriptableData.idObject;
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idEntity && t.TypeMapObject == TypeMapObject.Explore)
                .First();

            Data = saveData.data;
            _id = saveData.id;
            _idEntity = saveData.idEntity;
            Effects = saveData.Effects;
        }
    }

    public override void SetPlayer(Player player)
    {
        // base.SetPlayer(player);

        // // TODO PERK
        // List<GridTileNode> noskyNodes
        //     = GameManager.Instance.MapManager.DrawSky(OccupiedNode.position, 10);

        // player.SetNosky(noskyNodes);
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.explorers.Add(sdata);
    }
    #endregion
}
