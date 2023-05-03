using System;
using System.Linq;

using UnityEngine;

[Serializable]
public class BuildArmy : BaseBuild
{
    public DataBuildArmy Data = new DataBuildArmy();

    public EntityDwelling Dwelling;

    public BuildArmy(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        Player player,
        BuildArmy saveData = null
        )
    {
        base.Init(level, town, player);

        if (saveData == null)
        {
            ConfigData = configData;
            Dwelling = new EntityDwelling(((ScriptableBuildingArmy)configData).Dwelling);
            // Data.quantity =
            Dwelling.Data.value = ((ScriptableBuildingArmy)configData).Dwelling.Creature[0].CreatureParams.Growth;
            Dwelling.Data.level = level;
            Data.idEntity = Dwelling.Id;
            Dwelling.Data.idPlayer = Town.Data.idPlayer;
            UnitManager.Entities.Add(Dwelling.Id, Dwelling);
        }
        else
        {
            ConfigData = configData;
            Data = saveData.Data;

            var DwellingData = DataManager.Instance
                .DataPlay
                .entity
                .dwellings
                .Where(t => t.id == saveData.Data.idEntity)
                .First();
            Dwelling = new EntityDwelling(null, DwellingData);
            UnitManager.Entities.Add(DwellingData.id, Dwelling);
            // Dwelling.Data.value = saveData.data.quantity;
            // Dwelling.Data.level = level;
        }
    }

    public void RunGrowth()
    {
        Dwelling.Data.value += GetGrowth();
    }

    public int GetGrowth()
    {
        var growth = Dwelling.GetGrowth();
        return growth + Mathf.FloorToInt(Dwelling.Data.growth * Town.Data.koofcreature * 0.01f);
    }
}
