using Cysharp.Threading.Tasks;

using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class BuildArmy : BaseBuild
{
    public DataBuildArmy Data = new DataBuildArmy();

    public BuildArmy(
        int level,
        ScriptableBuildBase configData,
        SaveDataBuild<DataBuildArmy> saveData = null
        )
    {
        if (saveData == null)
        {
            ConfigData = configData;
            // ResourceSystem.Instance.GetBuildTowns()
            //     .Where(t => t.TypeFaction == typeFaction && t.typ)
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }

        base.Init(configData, level);
    }
}
