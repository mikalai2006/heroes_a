using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class UnitManager
{
    public static Dictionary<string, BaseEntity> Entities = new Dictionary<string, BaseEntity>();
    public static List<string> IdsExistsHeroes = new List<string>();
    // public static EntityTown SpawnTownAsync(GridTileNode gridNode, int keyArea)
    // {
    //     Player player = LevelManager.Instance.GetPlayer(keyArea);

    //     Area area = LevelManager.Instance.GetArea(keyArea);

    //     // Spawn town.
    //     EntityTown createdTown = null;
    //     createdTown = new EntityTown(gridNode, TypeGround.None);
    //     SpawnEntityToNode(gridNode, createdTown);
    //     area.town = createdTown;
    //     area.startPosition = createdTown.Position;
    //     if (player != null)
    //     {
    //         createdTown.SetPlayer(player);
    //     }

    //     // Spawn hero.
    //     if (player != null)
    //     {
    //         for (int i = 0; i < 4; i++)
    //         {
    //             EntityTown entityTown = (EntityTown)createdTown;
    //             ScriptableEntityTown scriptbaleEntity = (ScriptableEntityTown)entityTown.ScriptableData;

    //             var hero = scriptbaleEntity.heroes[Random.Range(0, scriptbaleEntity.heroes.Count)];
    //             if (scriptbaleEntity != null)
    //             {
    //                 EntityHero newEntity = new EntityHero(gridNode, TypeFaction.Neutral);
    //                 SpawnEntityToNode(gridNode, newEntity);
    //                 // gridNode.SetOcuppiedUnit(createdHero);
    //                 LevelManager.Instance.GetArea(keyArea).hero = newEntity;

    //                 newEntity.SetPlayer(player);
    //                 // player.AddHero(newEntity);
    //             }
    //         }
    //     }

    //     return createdTown;
    // }
    // public async Task<BaseMapEntity> SpawnUnitByTypeUnitAsync(GridTileNode node, TypeMapObject typeUnit)
    // {
    //     List<ScriptableMapObjectBase> list = ResourceSystem.Instance.GetUnitsByType<ScriptableMapObjectBase>(typeUnit);
    //     ScriptableMapObjectBase unit = list[Random.Range(0, list.Count)];
    //     BaseMapEntity createdUnit = await SpawnUnitToNode(unit, node);
    //     //node.OccupiedUnit = createdUnit;
    //     return createdUnit;
    // }

    // public async Task<BaseMapEntity> SpawnResourceAsync(GridTileNode node, List<TypeWorkPerk> typeWork, int level = 0)
    // {
    //     List<ScriptableMapResource> listObject = ResourceSystem.Instance.GetUnitsByType<ScriptableMapResource>(TypeMapObject.Resource).Where(t =>
    //         (t.typeGround == node.TypeGround
    //         || t.typeGround == TypeGround.None)
    //         && typeWork.Contains(t.TypeWorkMapObject)
    //         ).ToList();
    //     ScriptableMapObjectBase unit = listObject[Random.Range(0, listObject.Count)];
    //     // if (unit.name == "MillWater")
    //     // {
    //     //     GameManager.Instance.MapManager.CreateCreeks(node);
    //     // }
    //     BaseMapEntity createdUnit = await SpawnUnitToNode(unit, node);
    //     //node.OccupiedUnit = createdUnit;
    //     return createdUnit;
    // }

    // public async Task<BaseMapEntity> SpawnMapObjectToPositionAsync(GridTileNode node, TypeMapObject typeMapObject, int level = 0)
    // {
    //     List<ScriptableMapObject> listMapObject = ResourceSystem.Instance.GetUnitsByType<ScriptableMapObject>(typeMapObject).Where(t =>
    //         (
    //             t.typeGround == node.TypeGround
    //             || t.typeGround == TypeGround.None
    //         )
    //         && t.TypeMapObject == typeMapObject
    //         ).ToList();
    //     ScriptableMapObjectBase unit = listMapObject[Random.Range(0, listMapObject.Count)];
    //     // Debug.Log($"MapObject {unit.name}");
    //     if (unit.name == "MillWater")
    //     {
    //         GameManager.Instance.MapManager.CreateCreeks(node);
    //     }
    //     BaseMapEntity createdUnit = await SpawnUnitToNode(unit, node);
    //     //node._noPath = true;
    //     //node.OccupiedUnit = createdUnit;
    //     return createdUnit;
    // }
    // public static BaseEntity SpawnEntityCreature(GridTileNode node, SaveDataUnit<DataCreature> saveData)
    // {
    //     EntityCreature newEntity = new EntityCreature(null, saveData);
    //     Entities.Add(newEntity.IdEntity, newEntity);
    //     node.SetOcuppiedUnit(newEntity);
    //     newEntity.SetOccupiedNode(node);
    //     GameManager.Instance.MapManager.SetColorForTile(node.position, Color.magenta);
    //     newEntity.CreateMapGameObject(node);
    //     return newEntity;
    // }
    public static BaseEntity SpawnEntityCreature(GridTileNode nodeCreature, GridTileNode protectedNode, int levelMin = 1, int cost = 1, SaveDataUnit<DataCreature> saveData = null)
    {
        //if (node == null) return null;
        EntityCreature newEntity;
        ScriptableEntityCreature configNewEntity;
        if (saveData != null)
        {
            newEntity = new EntityCreature(null, saveData);
            configNewEntity = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                .Where(t => t.idObject == saveData.idObject)
                .First();
        }
        else
        {
            List<ScriptableEntityCreature> listWarriors = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
                .Where(t => t.CreatureParams.Level >= levelMin)
                .ToList();
            configNewEntity = listWarriors[Random.Range(0, listWarriors.Count)];
            newEntity = new EntityCreature(configNewEntity);
        }
        Entities.Add(newEntity.IdEntity, newEntity);
        nodeCreature.SetOcuppiedUnit(newEntity);
        newEntity.SetOccupiedNode(nodeCreature);
        GameManager.Instance.MapManager.SetColorForTile(nodeCreature.position, Color.magenta);
        // SpawnEntityToNode(node, newEntity);
        newEntity.CreateMapGameObject(nodeCreature);

        nodeCreature.SetProtectedNeigbours(newEntity, protectedNode);
        newEntity.SetValueCreature(configNewEntity.CreatureParams.AI > 0 ? cost / configNewEntity.CreatureParams.AI : 10);

        // node.OccupiedUnit = createdUnit;
        return newEntity;
    }

    #region Spawn entity
    public static BaseEntity SpawnEntityMapObjectToNode(GridTileNode node, BaseEntity entity)
    {
        Vector3Int pos = node.position;

        // entity.OccupiedNode = node;
        node.SetOcuppiedUnit(entity);
        // Debug.Log($"add entity {entity.ScriptableData.name}");
        Entities.Add(entity.IdEntity, entity);
        entity.CreateMapGameObject(node);

        // GameManager.Instance.MapManager.SetColorForTile(pos, Color.magenta);
        // node.SetState(TypeStateNode.Disabled);

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

        return entity;
    }

    public static EntityHero CreateHero(
        TypeFaction typeFaction,
        GridTileNode node,
        ScriptableEntityHero heroData = null
    )
    {
        // var entity = new EntityHero(TypeFaction.Neutral, activeHeroData);
        EntityHero newEntity = new EntityHero(typeFaction, heroData);
        node.SetAsGuested(newEntity);
        newEntity.CreateMapGameObject(node);
        Entities.Add(newEntity.IdEntity, newEntity);
        // UnitManager.IdsExistsHeroes.Add(heroData.idObject);
        // LevelManager.Instance.GetArea(area.id).hero = newEntity;
        return newEntity;
    }

    public static void Reset()
    {

        Entities.Clear();
        IdsExistsHeroes.Clear();

    }
    #endregion
}