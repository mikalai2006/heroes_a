public class BuildFactory
{
    public BuildGeneralBase CreateBuild(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        Player player
        )
    {
        switch (configData.TypeBuild)
        {
            case TypeBuild.Tavern:
                return new BuildTavern(level, configData, town, player);
            // case TypeBuild.Tavern:
            //     return new BuildTavern(level, configData, town, player);
            default:
                return new BuildGeneral(level, configData, town, player);
        }
    }
}