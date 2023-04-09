using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public struct DataMine
{
    public int idPlayer;
    public bool isMeet;

}
[Serializable]
public class EntityMine : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataMine Data = new DataMine();
    public ScriptableEntityMine ConfigData => (ScriptableEntityMine)ScriptableData;
    public EntityMine(
        GridTileNode node,
        // TypeGround typeGround,
        // TypeMine typeMine,
        ScriptableEntityMine configData,
        SaveDataUnit<DataMine> saveData = null
        )
    {
        if (saveData == null)
        {
            if (configData == null)
            {
                // List<ScriptableEntityMine> list = ResourceSystem.Instance
                //     .GetEntityByType<ScriptableEntityMine>(TypeEntity.Mine)
                //     // .Where(t => t.TypeMine == typeMine
                //     //     && (t.TypeGround == typeGround))
                //     .ToList();
                // ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
                ScriptableData = configData;
            }
            else
            {
                ScriptableData = configData;
            }
            Data.idPlayer = -1;
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMine>(TypeEntity.Mine)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        Data.idPlayer = player.DataPlayer.id;
        player.AddMines(this);
    }

    #region Change GameState
    public override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
        if (newState == GameState.StepNextPlayer)
        {
            ScriptableEntityMine configData = (ScriptableEntityMine)ScriptableData;
            Player player = LevelManager.Instance.ActivePlayer;
            configData.OnDoHero(ref player, this);
            // if (Data.idPlayer == player.DataPlayer.id)
            // {
            //     if (ConfigData.Resources.Count > 0)
            //     {
            //         var res = ConfigData.Resources[0].ListVariant[0];
            //         player.ChangeResource(res.Resource.TypeResource, res.maxValue);//res.maxValue
            //     }
            // }
        }
    }
    #endregion

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.mines.Add(sdata);
    }
    #endregion
}
