using System;

[Serializable]
public class BuildArmy : BaseBuild
{
    public DataBuildArmy Data = new DataBuildArmy();

    public BuildArmy(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        SaveDataBuild<DataBuildArmy> saveData = null
        )
    {
        base.Init(level, town);
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


    public void OnRunEffects()
    {
        ((ScriptableBuildingArmy)ConfigData).BuildLevels[level].OnAddEffect(ref _player, Town);
    }
    // public void OnRun()
    // {
    //     var data = ConfigData.BuildLevels[level].Attributes;
    //     if (data.Creature != null)
    //     {
    //         Data.quantity += data.Creature.CreatureParams.Growth;
    //     }
    // }
}
