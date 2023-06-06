using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityCreature : BaseEntity
{
    [SerializeField] public DataCreature Data = new DataCreature();
    public int totalAI => Data.value * ConfigAttribute.CreatureParams.AI;
    public ScriptableEntityMapObject ConfigData => (ScriptableEntityMapObject)ScriptableData;
    public ScriptableAttributeCreature ConfigAttribute => (ScriptableAttributeCreature)ScriptableDataAttribute;

    public EntityCreature(
        ScriptableAttributeCreature configCreature,
        SaveDataUnit<DataCreature> saveData = null)
    {
        base.Init();

        ScriptableData = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
            .Where(t => t.TypeMapObject == TypeMapObject.Creature)
            .First();

        if (saveData == null)
        {
            ScriptableDataAttribute = configCreature;
            _idEntity = ScriptableData.idObject;
            Data.value = 1;
            Data.idObject = ScriptableDataAttribute.idObject;
        }
        else
        {

            if (saveData.idEntity != "")
            {
                ScriptableDataAttribute = ResourceSystem.Instance
                    .GetAttributesByType<ScriptableAttributeCreature>(TypeAttribute.Creature)
                    .Find(t => t.idObject == saveData.data.idObject);
            }
            Data = saveData.data;
            _idEntity = saveData.idEntity;
            _id = saveData.id;
        }
    }

    public void SetValueCreature(int quantityCreature)
    {
        if (quantityCreature > 0)
        {
            Data.value = quantityCreature;
        }
        else
        {
            Data.value = 10;
        }
    }

    public override void SetPlayer(Player player)
    {
        // // ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        // ScriptableEntityCreature configData = (ScriptableEntityCreature)ScriptableData;
        // // configData.OnDoHero(ref player, this);
        // // MapObjectGameObject.DestroyGameObject();
        // DestroyEntity();
    }

    // public void SetOccupiedNode(GridTileNode node)
    // {
    //     OccupiedNode = node;
    // }

    // public void SetProtectedNode(GridTileNode protectedNode)
    // {
    //     ProtectedNode = protectedNode;
    //     Data.protectedNode = protectedNode.position;
    // }

    #region SaveLoadData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.creatures.Add(sdata);
    }

    #endregion
}
