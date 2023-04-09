public class EntityMapObjectFactory
{
    public BaseEntity CreateMapObject(
        TypeMapObject TypeMapObject,
        GridTileNode node,
        ScriptableEntity configData
        )
    {
        switch (TypeMapObject)
        {
            case TypeMapObject.Mine:
                return new EntityMine(node, (ScriptableEntityMine)configData);
            case TypeMapObject.Explore:
                return new EntityExpore(node, (ScriptableEntityExplore)configData);
            case TypeMapObject.Portal:
                return new EntityMonolith(node, (ScriptableEntityPortal)configData);
            case TypeMapObject.Skills:
            case TypeMapObject.Resources:
                return new EntityMapObject(node, (ScriptableEntityMapObject)configData);
            case TypeMapObject.Artifact:
                return new EntityArtifact(node, (ScriptableEntityArtifact)configData);
            default:
                return null;
        }
    }
}