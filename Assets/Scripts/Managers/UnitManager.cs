//using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class UnitManager : MonoBehaviour
{
    [Header("Unit tilemap settings")]
    [Space(10)]
    [SerializeField] public Tilemap _tileMapUnits;

    private void Awake()
    {
        ResetUnitManager();
    }

    public void ResetUnitManager()
    {
        Helpers.DestroyChildren(_tileMapUnits.transform);
    }

    public EntityTown SpawnTownAsync(GridTileNode gridNode, int keyArea)
    {
        Player player = LevelManager.Instance.GetPlayer(keyArea);

        Area area = LevelManager.Instance.GetArea(keyArea);

        // Spawn town.
        EntityTown createdTown = null;
        // ScriptableEntityTown scriptbaleEntityTown = ResourceSystem.Instance
        //     .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
        //     .Where(t => t.typeGround == gridNode.TypeGround)
        //     .First();

        // if (scriptbaleEntityTown != null)
        // {
        createdTown = new EntityTown(gridNode);
        SpawnEntityToNode(gridNode, createdTown);
        area.town = createdTown;
        area.startPosition = createdTown.Position;
        if (player != null)
        {
            // player.AddTown(createdTown);
            createdTown.SetPlayer(player);
        }
        //_townsList.Add(createdTown);
        //gridNode.OccupiedUnit = createdTown;
        // }

        // Spawn hero.
        if (player != null)
        {
            for (int i = 0; i < 4; i++)
            {
                EntityTown entityTown = (EntityTown)createdTown;
                ScriptableEntityTown scriptbaleEntity = (ScriptableEntityTown)entityTown.ScriptableData;

                var hero = scriptbaleEntity.heroes[Random.Range(0, scriptbaleEntity.heroes.Count)];
                if (scriptbaleEntity != null)
                {
                    EntityHero newEntity = new EntityHero(gridNode);
                    SpawnEntityToNode(gridNode, newEntity);
                    // gridNode.SetOcuppiedUnit(createdHero);
                    LevelManager.Instance.GetArea(keyArea).hero = newEntity;

                    newEntity.SetPlayer(player);
                    // player.AddHero(newEntity);
                }
            }
        }

        // Spawn mines.
        //for (int i = 0; i < town.mines.Count; i++)
        //{
        //    var listNodes = GameManager.Instance.mapManager.GridTileHelper().GetAllGridNodes().Where(t =>
        //    t.keyArea == keyArea
        //    && t.Empty
        //    && t.Enable
        //    && GameManager.Instance.mapManager.GridTileHelper().GetDistanceBetweeenPoints(t._position, gridNode._position) >= 5
        //    && GameManager.Instance.mapManager.GridTileHelper().GetDistanceBetweeenPoints(t._position, gridNode._position) <= 10
        //    && GameManager.Instance.mapManager.GridTileHelper().CalculateNeighboursByArea(t) == 8
        //   ).ToList();
        //    GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();
        //    //GridTileNode node = GameManager.Instance.mapManager.GetNodeForUnitSpawn(gridNode._position,  keyArea, 5, 10);
        //    if (nodeForSpawn != null)
        //    {
        //        UnitBase createdMine = SpawnUnitToNode(town.mines[i], nodeForSpawn);
        //    }
        //}

        return createdTown;
    }
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

    public BaseEntity SpawnWarriorAsync(GridTileNode node, TypeGround typeGroud = TypeGround.None, int level = 1)
    {
        //if (node == null) return null;

        List<ScriptableEntityCreature> listWarriors = ResourceSystem.Instance.GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature);
        ScriptableEntityCreature scriptbaleEntity = listWarriors[Random.Range(0, listWarriors.Count)];
        EntityCreature newEntity = new EntityCreature(node);
        SpawnEntityToNode(node, newEntity);

        // node.OccupiedUnit = createdUnit;
        return newEntity;
    }
    // public async Task<BaseMapEntity> SpawnMineAsync(GridTileNode node, TypeMine typeMine)
    // {
    //     List<ScriptableEntityMapObject> listMine = ResourceSystem.Instance.GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject);
    //     ScriptableEntityMapObject unit = listMine.Where(t =>
    //         t.TypeMapObject == TypeMapObject.Mine
    //     // && (node.TypeGround == t.typeGround || t.typeGround == TypeGround.None)
    //     ).OrderBy(t => Random.value).First();
    //     BaseMapEntity createdUnit = await SpawnEntityToNode(unit, node);
    //     //node.OccupiedUnit = createdUnit;
    //     return createdUnit;
    // }
    // public BaseEntity SpawnArtifactAsync(GridTileNode node)
    // {
    //     List<ScriptableEntityArtifact> list = ResourceSystem.Instance
    //         .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Artifact)
    //         .ToList();
    //     if (list.Count == 0) return null;
    //     ScriptableEntityArtifact scriptbaleEntity = list[Random.Range(0, list.Count)];
    //     EntityArtifact newEntity = new EntityArtifact(scriptbaleEntity, node);
    //     SpawnEntityToNode(node, newEntity);
    //     return newEntity;
    // }
    // public BaseEntity SpawnMapObjectAsync(GridTileNode node, TypeMapObject type)
    // {
    //     List<ScriptableEntityMapObject> list = ResourceSystem.Instance
    //         .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
    //         .Where(t => t.TypeMapObject == type)
    //         .ToList();
    //     if (list.Count == 0) return null;
    //     ScriptableEntityMapObject scriptbaleEntity = list[Random.Range(0, list.Count)];
    //     EntityResource newEntity = new EntityResource(scriptbaleEntity, node);
    //     SpawnEntityToNode(node, newEntity);
    //     return newEntity;
    // }

    // public BaseEntity SpawnMapObjectAsync(GridTileNode node, TypeMapObject type, List<TypeWorkPerk> typeWork)
    // {
    //     List<ScriptableEntityMapObject> list = ResourceSystem.Instance
    //         .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
    //         .Where(t => t.TypeMapObject == type && typeWork.Contains(t.TypeWorkPerk))
    //         .ToList();
    //     if (list.Count == 0) return null;
    //     ScriptableEntityMapObject scriptbaleEntity = list[Random.Range(0, list.Count)];
    //     EntityResource newEntity = new EntityResource(scriptbaleEntity, node);
    //     SpawnEntityToNode(node, newEntity);
    //     return newEntity;
    // }
    // public BaseEntity CreateEntity<T>(GridTileNode node, TypeEntity typeEntity)
    // {
    //     List<ScriptableEntity> list = ResourceSystem.Instance
    //         .GetEntityByType<ScriptableEntity>(typeEntity).ToList();
    //     if (list.Count == 0) return null;

    //     // switch (typeEntity)
    //     // {
    //     //     case TypeEntity.Artifact:
    //     //         ScriptableEntityArtifact scriptbaleEntity = (ScriptableEntityArtifact)list[Random.Range(0, list.Count)];
    //     //         newEntity = new EntityResource(scriptbaleEntity, node);
    //     //         SpawnEntityToNode(scriptbaleEntity, node, newEntity);
    //     //         break;
    //     //     case TypeEntity.Creature:
    //     //         ScriptableEntityCreature scriptbaleEntityCreature = (ScriptableEntityCreature)list[Random.Range(0, list.Count)];
    //     //         newEntity = new EntityResource(scriptbaleEntityCreature, node);
    //     //         SpawnEntityToNode(scriptbaleEntityCreature, node, newEntity);
    //     //         break;
    //     //     case TypeEntity.Resource:
    //     //         ScriptableEntityResource scriptbaleEntityResource = (ScriptableEntityResource)list[Random.Range(0, list.Count)];
    //     //         newEntity = new EntityResource(scriptbaleEntityResource, node);
    //     //         SpawnEntityToNode(scriptbaleEntityResource, node, newEntity);
    //     //         break;
    //     //     case TypeEntity.Hero:
    //     //         ScriptableEntityHero scriptbaleEntityHero = (ScriptableEntityHero)list[Random.Range(0, list.Count)];
    //     //         newEntity = new EntityResource(scriptbaleEntityHero, node);
    //     //         SpawnEntityToNode(scriptbaleEntityHero, node, newEntity);
    //     //         break;
    //     //     case TypeEntity.Town:
    //     //         ScriptableEntityTown scriptbaleEntityTown = (ScriptableEntityTown)list[Random.Range(0, list.Count)];
    //     //         newEntity = new EntityResource(scriptbaleEntityTown, node);
    //     //         SpawnEntityToNode(scriptbaleEntityTown, node, newEntity);
    //     //         break;
    //     //     case TypeEntity.MapObject:
    //     //         break;
    //     // }
    //     return newEntity;
    // }
    // private async UniTask<BaseMapEntity> CreateAsync(ScriptableMapObjectBase unit, GridTileNode node)
    // {
    //     if (unit.MapPrefab.RuntimeKeyIsValid())
    //     {
    //         AsyncOperationHandle<GameObject> operationHandle = Addressables.InstantiateAsync(
    //             unit.MapPrefab,
    //             node.position,
    //             Quaternion.identity,
    //             GameManager.Instance.MapManager.UnitManager._tileMapUnits.transform
    //             );

    //         await operationHandle.Task;
    //         if (operationHandle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             var r_asset = operationHandle.Result;
    //             var _asset = r_asset.GetComponent<BaseMapEntity>();
    //             // _asset.InitUnit(unit, node.position);
    //             Debug.Log($"Spawn UNIT1::: {unit.TypeMapObject}");
    //             return _asset;
    //         }
    //         else
    //         {
    //             Debug.LogError($"Error Load prefab: {operationHandle.Status}");
    //         }

    //     }
    //     return null;
    // }

    // public async UniTask<BaseMapEntity> SpawnUnitToNode(ScriptableMapObjectBase unit, GridTileNode node)
    // {
    //     Vector3Int pos = node.position;

    //     if (unit.Prefab.OccupiedNode != null)
    //     {
    //         unit.Prefab.OccupiedNode = null;
    //     }
    //     if (node.OccupiedUnit != null)
    //     {
    //         node.SetOcuppiedUnit(null); // OccupiedUnit = null;
    //     }

    //     BaseMapEntity spawnedUnit = null;
    //     if (unit.MapPrefab.RuntimeKeyIsValid())
    //     {
    //         // var test = new BaseCharacter(unit, pos);
    //         spawnedUnit = await CreateAsync(unit, node);
    //     }
    //     else
    //     {
    //         spawnedUnit = Instantiate(unit.Prefab, pos, Quaternion.identity, _tileMapUnits.transform);
    //         // spawnedUnit.InitUnit(unit, pos);
    //     }

    //     spawnedUnit.OccupiedNode = node;
    //     node.SetOcuppiedUnit(spawnedUnit);

    //     Debug.Log($"Spawn UNIT2::: {spawnedUnit.OccupiedNode.position}- {node.OccupiedUnit.name}");
    //     if (unit.typeInput == TypeInput.None)
    //     {
    //         // GameManager.Instance.mapManager.SetNotPath(node);
    //     }
    //     else
    //     {
    //         if (unit.RulesDraw.Count > 0)
    //         {
    //             GameManager.Instance.MapManager.GridTileHelper().SetDisableNode(node, unit.RulesDraw, Color.red);
    //         }

    //         GameManager.Instance.MapManager.SetColorForTile(pos, Color.magenta);
    //     }

    //     return spawnedUnit;
    // }


    #region Spawn entity
    // private BaseMapEntity CreateEntityAsync(ScriptableEntity entity, GridTileNode node)
    // {
    //     if (entity.MapPrefab.RuntimeKeyIsValid())
    //     {
    //         AsyncOperationHandle<GameObject> operationHandle = Addressables.InstantiateAsync(
    //             entity.MapPrefab,
    //             node.position,
    //             Quaternion.identity,
    //             GameManager.Instance.MapManager.UnitManager._tileMapUnits.transform
    //             );

    //         // await operationHandle.Task;
    //         if (operationHandle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             var r_asset = operationHandle.Result;
    //             var _asset = r_asset.GetComponent<BaseMapEntity>();
    //             _asset.InitUnit(entity, node.position);
    //             // Debug.Log($"Spawn Entity::: {entity.name}");
    //             return _asset;
    //         }
    //         else
    //         {
    //             Debug.LogError($"Error Load prefab: {operationHandle.Status}");
    //         }

    //     }
    //     return null;
    // }
    public BaseEntity SpawnEntityToNode(GridTileNode node, BaseEntity entity)
    {
        Vector3Int pos = node.position;

        // if (unit.Prefab.OccupiedNode != null)
        // {
        //     unit.Prefab.OccupiedNode = null;
        // }
        // if (node.OccupiedUnit != null)
        // {
        //     node.SetOcuppiedUnit(null); // OccupiedUnit = null;
        // }

        // BaseEntity spawnedUnit = null;
        // if (scriptableEntity.MapPrefab.RuntimeKeyIsValid())
        // {
        //     // CreateEntityAsync(entity, node);
        // }
        // else
        // {
        //     Debug.LogError($"Error spawn entity: {scriptableEntity.name}");
        // }

        entity.OccupiedNode = node;
        node.SetOcuppiedUnit(entity);

        // Debug.Log($"Spawn UNIT2::: {spawnedUnit.OccupiedNode.position}- {node.OccupiedUnit.name}");
        if (entity.ScriptableData.typeInput == TypeInput.None)
        {
            // GameManager.Instance.mapManager.SetNotPath(node);
        }
        else
        {
            if (entity.ScriptableData.RulesDraw.Count > 0)
            {
                GameManager.Instance.MapManager.GridTileHelper().SetDisableNode(node, entity.ScriptableData.RulesDraw, Color.red);
            }

            GameManager.Instance.MapManager.SetColorForTile(pos, Color.magenta);
        }

        return entity;
    }
    #endregion
}