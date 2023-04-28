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
            _idEntity = ScriptableData.idObject;
            configData.SetData(this);
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idEntity && t.TypeMapObject == TypeMapObject.Mine)
                .First();

            Data = saveData.data;
            _id = saveData.id;
            _idEntity = saveData.idEntity;
            Effects = saveData.Effects;
        }
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        Data.idPlayer = player.DataPlayer.id;
        player.AddMines(this);
        _player = player;
        // ((MapEntityMine)MapObjectGameObject).SetPlayer(player);
    }

    #region Change GameState
    public async override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
        switch (newState)
        {
            case GameState.NextDay:
                if (_player != null && LevelManager.Instance.ActivePlayer == _player)
                {
                    ScriptableEntityMine configData = (ScriptableEntityMine)ScriptableData;
                    await configData.RunHero(_player, this);
                }
                break;
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
