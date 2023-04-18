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
public class EntityMine : BaseEntity
{
    [SerializeField] public DataMine Data = new DataMine();
    public ScriptableEntityMine ConfigData => (ScriptableEntityMine)ScriptableData;
    public EntityMine(
        ScriptableEntityMine configData,
        SaveDataUnit<DataMine> saveData = null
        )
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            Data.idPlayer = -1;
            idObject = ScriptableData.idObject;
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Mine)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        Data.idPlayer = player.DataPlayer.id;
        player.AddMines(this);
        _player = player;
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

    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.mines.Add(sdata);
    }
    #endregion
}
