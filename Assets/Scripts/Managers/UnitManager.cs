using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class UnitManager
{
    public static Dictionary<string, BaseEntity> Entities = new Dictionary<string, BaseEntity>();
    public static Dictionary<string, MapObject> MapObjects = new Dictionary<string, MapObject>();
    public static List<string> IdsExistsHeroes = new List<string>();

    public static MapObject SpawnEntityCreature(GridTileNode nodeCreature, GridTileNode protectedNode, int levelMin = 1, int rmgvalue = 0, SaveDataUnit<DataCreature> saveData = null)
    {
        //if (node == null) return null;
        EntityCreature newEntity = null;
        ScriptableAttributeCreature configNewEntity;
        if (saveData != null)
        {
            newEntity = new EntityCreature(null, saveData);
            configNewEntity = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeCreature>(TypeAttribute.Creature)
                .Where(t => t.idObject == saveData.idObject)
                .First();
        }
        else
        {
            var protectionIndex = LevelManager.Instance.CurrentProtectionIndex;
            float totalAIValue
                = ((rmgvalue > protectionIndex.minimalValue1) ? (rmgvalue - protectionIndex.minimalValue1) * protectionIndex.koof1 : 0)
                + ((rmgvalue > protectionIndex.minimalValue2) ? (rmgvalue - protectionIndex.minimalValue2) * protectionIndex.koof2 : 0);


            List<ScriptableAttributeCreature> listWarriors = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeCreature>(TypeAttribute.Creature)
                .Where(t =>
                    t.CreatureParams.Level >= levelMin
                    && t.CreatureParams.AI <= totalAIValue
                    )
                .ToList();
            if (listWarriors.Count > 0)
            {
                configNewEntity = listWarriors[Random.Range(0, listWarriors.Count)];
                newEntity = new EntityCreature(configNewEntity);

                var countCreatures = Mathf.FloorToInt(totalAIValue / configNewEntity.CreatureParams.AI);
                if (countCreatures > 4)
                {
                    countCreatures += Random.Range(0, (countCreatures / 4));
                }
                // Debug.Log(
                //     $"protected entity= {protectedNode.OccupiedUnit.ScriptableData.name}:::"
                //     + $"\r\n rmgvalue={rmgvalue}::: totalAIValue={totalAIValue},"
                //     + $"\r\n AI={configNewEntity.CreatureParams.AI}[confName{configNewEntity.name}]"
                //     + $"\r\n total_AI_Value{totalAIValue} = (rmgvalue{rmgvalue} > protectionIndex.minimalValue1{protectionIndex.minimalValue1} ? (rmgvalue{rmgvalue} - protectionIndex.minimalValue1{protectionIndex.minimalValue1}) * protectionIndex.koof1{protectionIndex.koof1} : 0) + (rmgvalue{rmgvalue} > protectionIndex.minimalValue2{protectionIndex.minimalValue2} ? (rmgvalue{rmgvalue} - protectionIndex.minimalValue2{protectionIndex.minimalValue2}) * protectionIndex.koof2{protectionIndex.koof2} : 0)"
                //     + $"\r\n countCreatures={countCreatures}");
                newEntity.SetValueCreature(countCreatures);
            }
            else
            {
                // Debug.Log($"Not created creature for {protectedNode.OccupiedUnit.ScriptableData.name}, totalAIValue={totalAIValue}, rmgvalue={rmgvalue}");
            }
        }

        MapObject entityMapObject = null;
        if (newEntity != null)
        {
            entityMapObject = SpawnEntityMapObjectToNode(nodeCreature, newEntity);
            // Entities.Add(newEntity.IdEntity, newEntity);

            // entityMapObject = new MapObject();
            // MapObjects.Add(entityMapObject.IdEntity, entityMapObject);
            // entityMapObject.SetEntity(newEntity, nodeCreature);

            // nodeCreature.SetOcuppiedUnit(entityMapObject);

            // entityMapObject.CreateMapGameObject(nodeCreature);
            GameManager.Instance.MapManager.SetColorForTile(nodeCreature.position, Color.magenta);
            nodeCreature.SetProtectedNeigbours(entityMapObject, protectedNode);
        }

        return entityMapObject;
    }

    #region Spawn entity
    public static void SpawnEnity(BaseEntity entity)
    {
        Entities.Add(entity.IdEntity, entity);
    }

    public static MapObject SpawnEntityMapObjectToNode(GridTileNode node, BaseEntity entity, MapObject mapObject = null)
    {
        // Entities.Add(entity.IdEntity, entity);

        Vector3Int pos = node.position;

        MapObject newMapObject;
        if (mapObject == null)
        {
            newMapObject = new MapObject();
            SpawnEnity(entity);
        }
        else
        {
            newMapObject = mapObject;
        }
        newMapObject.SetEntity(entity, node);
        // entity.OccupiedNode = node;
        node.SetOcuppiedUnit(newMapObject);
        // Debug.Log($"add entity {entity.ScriptableData.name}");
        newMapObject.CreateMapGameObject(node);

        // GameManager.Instance.MapManager.SetColorForTile(pos, Color.magenta);
        // node.SetState(TypeStateNode.Disabled);
        // Debug.Log($"entity.ScriptableData={entity.ScriptableData.name}");
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)entity.ScriptableData;
        if (configData.RulesDraw.Count > 0)
        {
            // GameManager.Instance.MapManager
            //     .GridTileHelper().SetOccupiedNodes(node, configData.RulesDraw, Color.red);
            List<GridTileNode> list
                = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(node, configData.RulesDraw);
            if (list.Count > 0)
            {
                foreach (GridTileNode nodePath in list)
                {
                    GameManager.Instance.MapManager.SetColorForTile(nodePath.position, Color.black);
                    nodePath.SetDisableNode();
                    // nodePath.SetOcuppiedUnit(entity);
                    // SetDisableNode(nodePath, null, color);
                }
            }
        }
        if (configData.RulesInput.Count > 0)
        {
            List<GridTileNode> list
                = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(node, configData.RulesInput);
            if (list.Count > 0)
            {
                foreach (GridTileNode nodex in list)
                {
                    // GameManager.Instance.MapManager.SetColorForTile(nodex.position, Color.green);
                    nodex.SetAsInputPoint(node);
                }
            }
        }

        MapObjects.Add(newMapObject.IdEntity, newMapObject);
        return newMapObject;
    }

    public static EntityHero CreateHero(
        TypeFaction typeFaction,
        GridTileNode node,
        ScriptableEntityHero heroData = null
    )
    {
        // ScriptableEntityHero configData
        //     = ResourceSystem.Instance
        //     .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
        //     .First();
        // var entity = new EntityHero(TypeFaction.Neutral, activeHeroData);
        EntityHero newEntity = new EntityHero(typeFaction, heroData);

        Entities.Add(newEntity.IdEntity, newEntity);
        var entityMapObject = new MapObject();
        MapObjects.Add(entityMapObject.IdEntity, entityMapObject);

        entityMapObject.SetEntity(newEntity, node);
        node.SetAsGuested(entityMapObject);
        entityMapObject.CreateMapGameObject(node);

        // UnitManager.IdsExistsHeroes.Add(heroData.idObject);
        // LevelManager.Instance.GetArea(area.id).hero = newEntity;
        return newEntity;
    }

    public static void Reset()
    {

        Entities.Clear();
        MapObjects.Clear();
        IdsExistsHeroes.Clear();

    }
    #endregion
}