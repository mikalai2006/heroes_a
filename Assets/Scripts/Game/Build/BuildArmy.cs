using System;

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
}
