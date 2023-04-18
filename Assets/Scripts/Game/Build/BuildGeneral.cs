using System;

using UnityEngine;

[Serializable]
public class BuildGeneral : BaseBuild
{
    public DataBuildGeneral Data = new DataBuildGeneral();

    public BuildGeneral(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        SaveDataBuild<DataBuildGeneral> saveData = null
        )
    {
        base.Init(level, town);

        if (saveData == null)
        {
            ConfigData = configData;
            OnRunEffects();
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }
    }
    public void OnRunEffects()
    {
        ((ScriptableBuilding)ConfigData).BuildLevels[level].OnAddEffect(ref _player, Town);
    }
}
