using UnityEngine;

[System.Serializable]
public class EntityCreature : BaseEntity
{
    [SerializeField] public DataCreature Data = new DataCreature();
    public ScriptableEntityCreature ConfigData => (ScriptableEntityCreature)ScriptableData;

    public EntityCreature(
        ScriptableEntityCreature configData,
        SaveDataUnit<DataCreature> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            idObject = ScriptableData.idObject;
            Data.value = 1;
        }
        else
        {
            if (saveData.idObject != "")
            {
                ScriptableData = ResourceSystem.Instance
                    .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                    .Find(t => t.idObject == saveData.idObject);
            }
            Data = saveData.data;
            idObject = saveData.idObject;
            idUnit = saveData.idUnit;
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
        // ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ScriptableData;
        ScriptableEntityCreature configData = (ScriptableEntityCreature)ScriptableData;
        // configData.OnDoHero(ref player, this);
        MapObjectGameObject.DestroyGameObject();
        DestroyMapGameObject();
    }

    public void SetOccupiedNode(GridTileNode node)
    {
        OccupiedNode = node;
    }

    public void SetProtectedNode(GridTileNode protectedNode)
    {
        ProtectedNode = protectedNode;
        Data.protectedNode = protectedNode.position;
    }

    #region SaveLoadData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.creatures.Add(sdata);
    }

    #endregion
}
