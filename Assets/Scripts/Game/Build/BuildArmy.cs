using System;

using UnityEngine;

[Serializable]
public class BuildArmy : BaseBuild
{
    public DataBuildArmy Data = new DataBuildArmy();

    public BuildArmy(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        Player player,
        SaveDataBuild<DataBuildArmy> saveData = null
        )
    {
        base.Init(level, town, player);
        if (saveData == null)
        {
            ConfigData = configData;
            Data.quantity = ((ScriptableBuildingArmy)configData).Creatures[0].CreatureParams.Growth;
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }

    }
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
    //     OnRunArmyBuilds();
    // }
    // private void OnNextDay()
    // {

    //     Debug.Log($"Army::: Next day - {ConfigData.name}");

    // }

    // private void OnRunArmyBuilds()
    // {

    //     Debug.Log($"Army::: Next week - {ConfigData.name}");

    // }
    // public void OnRunEffects()
    // {
    //     ((ScriptableBuildingArmy)ConfigData).BuildLevels[level].RunOne(ref _player, Town);
    // }
    // public void OnRun()
    // {
    //     var data = ConfigData.BuildLevels[level].Attributes;
    //     if (data.Creature != null)
    //     {
    //         Data.quantity += data.Creature.CreatureParams.Growth;
    //     }
    // }
}
