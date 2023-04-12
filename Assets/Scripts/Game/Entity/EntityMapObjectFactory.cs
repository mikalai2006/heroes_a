public class EntityMapObjectFactory
{
    public BaseEntity CreateMapObject(
        TypeMapObject TypeMapObject,
        ScriptableEntity configData
        )
    {
        switch (TypeMapObject)
        {
            case TypeMapObject.Dwelling:
                return new EntityDwelling((ScriptableEntityDwelling)configData);
            case TypeMapObject.Mine:
                return new EntityMine((ScriptableEntityMine)configData);
            case TypeMapObject.Explore:
                return new EntityExpore((ScriptableEntityExplore)configData);
            case TypeMapObject.Portal:
                return new EntityMonolith((ScriptableEntityPortal)configData);
            case TypeMapObject.Skills:
                return new EntityMapObject((ScriptableEntityMapObject)configData);
            case TypeMapObject.Resources:
                return new EntityMapObject((ScriptableEntityMapObject)configData);
            case TypeMapObject.Artifact:
                return new EntityArtifact((ScriptableEntityArtifact)configData);
            default:
                return null;
        }
    }
}