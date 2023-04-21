using System;

using UnityEngine;

[Serializable]
public abstract class BuildGeneralBase : BaseBuild
{
    // public DataBuildGeneral Data = new DataBuildGeneral();

    // public BuildGeneral(
    //     int level,
    //     ScriptableBuilding configData,
    //     EntityTown town,
    //     Player player,
    //     SaveDataBuild<DataBuildGeneral> saveData = null
    //     )
    // {
    //     base.Init(level, town, player);

    //     if (saveData == null)
    //     {
    //         ConfigData = configData;
    //         // OnRunEffects();
    //     }
    //     else
    //     {
    //         ConfigData = configData;
    //         Data = saveData.data;
    //     }
    // }

    // public override void OnAfterStateChanged(GameState newState)
    // {
    //     base.OnAfterStateChanged(newState);
    //     if (Player == LevelManager.Instance.ActivePlayer)
    //     {
    //         switch (newState)
    //         {
    //             case GameState.NextDay:
    //                 OnNextDay();
    //                 break;
    //             case GameState.NextWeek:
    //                 OnNextWeek();
    //                 break;
    //         }
    //     }
    // }
    // private void OnNextWeek()
    // {
    //     Debug.Log($"General::: Next week - {ConfigData.name}");
    // }
    // private void OnNextDay()
    // {

    //     Debug.Log($"General::: Next day - {ConfigData.name}");

    // }
}
