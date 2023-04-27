using System;

[Serializable]
public class BuildGeneral : BaseBuild
{
    public DataBuildGeneral Data = new DataBuildGeneral();
    public BuildGeneral(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        Player player,
        SaveDataBuild<DataBuildGeneral> saveData = null
        )
    {
        base.Init(level, town, player);

        if (saveData == null)
        {
            ConfigData = configData;
            // OnRunEffects();
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }
    }
}
