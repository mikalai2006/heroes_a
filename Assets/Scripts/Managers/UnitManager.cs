//using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitManager : MonoBehaviour
{
    [Header("Unit tilemap settings")]
    [Space(10)]
    [SerializeField] private Tilemap _tileMapUnits;

    private void Awake()
    {
        ResetUnitManager();
    }

    public void ResetUnitManager()
    {
        Helpers.DestroyChildren(_tileMapUnits.transform);
    }

    public ScriptableTown SpawnTown(GridTileNode gridNode, int keyArea)
    {
        Player player = LevelManager.Instance.GetPlayer(keyArea);

        Area area = LevelManager.Instance.GetArea(keyArea);

        // Spawn town.
        UnitBase createdTown = null;
        ScriptableTown town = ResourceSystem.Instance.GetUnitsByType<ScriptableTown>(TypeUnit.Town)
            .Where(t => t.typeGround == gridNode.TypeGround).First();

        if (town != null)
        {
            createdTown = SpawnUnitToNode(town, gridNode);
            area.town = createdTown;
            area.startPosition = createdTown.Position;
            if (player != null)
            {
                player.AddTown(createdTown);
            }
            //_townsList.Add(createdTown);
            //gridNode.OccupiedUnit = createdTown;
        }

        // Spawn hero.
        if (player != null)
        {
            for (int i = 0; i < 4; i++)
            {
                ScriptableHero randomHero = town.heroes[Random.Range(0, town.heroes.Count)];
                if (randomHero != null)
                {
                    Hero createdHero = (Hero)SpawnUnitToNode(randomHero, gridNode);
                    gridNode.SetOcuppiedUnit(createdHero); // .OccupiedUnit = createdHero;
                    LevelManager.Instance.GetArea(keyArea).hero = createdHero;

                    player.AddHero(createdHero);
                    // player.SetActiveHero((Hero)createdHero);
                    // if (i == 0) createdHero.SetHeroAsActive();
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

        return town;
    }
    public UnitBase SpawnUnitByTypeUnit(GridTileNode node, TypeUnit typeUnit)
    {
        List<ScriptableUnitBase> list = ResourceSystem.Instance.GetUnitsByType<ScriptableUnitBase>(typeUnit);
        ScriptableUnitBase unit = list[Random.Range(0, list.Count)];
        UnitBase createdUnit = SpawnUnitToNode(unit, node);
        //node.OccupiedUnit = createdUnit;
        return createdUnit;
    }

    public UnitBase SpawnResource(GridTileNode node, List<TypeWork> typeWork, int level = 0)
    {
        List<ScriptableResource> listObject = ResourceSystem.Instance.GetUnitsByType<ScriptableResource>(TypeUnit.Resource).Where(t =>
            (t.typeGround == node.TypeGround
            || t.typeGround == TypeGround.None)
            && typeWork.Contains(t.TypeWork)
            ).ToList();
        ScriptableUnitBase unit = listObject[Random.Range(0, listObject.Count)];
        // if (unit.name == "MillWater")
        // {
        //     GameManager.Instance.MapManager.CreateCreeks(node);
        // }
        UnitBase createdUnit = SpawnUnitToNode(unit, node);
        //node.OccupiedUnit = createdUnit;
        return createdUnit;
    }

    public UnitBase SpawnMapObjectToPosition(GridTileNode node, MapObjectType typeMapObject, int level = 0)
    {
        List<ScriptableMapObject> listMapObject = ResourceSystem.Instance.GetUnitsByType<ScriptableMapObject>(TypeUnit.MapObject).Where(t =>
            (
                t.typeGround == node.TypeGround
                || t.typeGround == TypeGround.None
            )
            && t.TypeMapObject == typeMapObject
            ).ToList();
        ScriptableUnitBase unit = listMapObject[Random.Range(0, listMapObject.Count)];
        Debug.Log($"MapObject {unit.name}");
        if (unit.name == "MillWater")
        {
            GameManager.Instance.MapManager.CreateCreeks(node);
        }
        UnitBase createdUnit = SpawnUnitToNode(unit, node);
        //node._noPath = true;
        //node.OccupiedUnit = createdUnit;
        return createdUnit;
    }

    public BaseWarriors SpawnWarrior(GridTileNode node, TypeGround typeGroud = TypeGround.None, int level = 1)
    {
        //if (node == null) return null;

        List<ScriptableWarriors> listWarriors = ResourceSystem.Instance.GetUnitsByType<ScriptableWarriors>(TypeUnit.Warrior);
        ScriptableUnitBase unit = listWarriors[Random.Range(0, listWarriors.Count)];
        BaseWarriors createdUnit = (BaseWarriors)SpawnUnitToNode(unit, node);

        // node.OccupiedUnit = createdUnit;
        return createdUnit;
    }
    public UnitBase SpawnMine(GridTileNode node, TypeMine typeMine)
    {
        List<ScriptableMine> listMine = ResourceSystem.Instance.GetUnitsByType<ScriptableMine>(TypeUnit.Mine);
        ScriptableUnitBase unit = listMine.Where(t =>
            t.typeMine == typeMine
            && (node.TypeGround == t.typeGround || t.typeGround == TypeGround.None)
        ).OrderBy(t => Random.value).First();
        UnitBase createdUnit = SpawnUnitToNode(unit, node);
        //node.OccupiedUnit = createdUnit;
        return createdUnit;
    }

    public UnitBase SpawnUnitToNode(ScriptableUnitBase unit, GridTileNode node)
    {
        Vector3Int pos = node.position;

        if (unit.Prefab.OccupiedNode != null)
        {
            unit.Prefab.OccupiedNode = null;
        }
        if (node.OccupiedUnit != null)
        {
            node.SetOcuppiedUnit(null); // OccupiedUnit = null;
        }

        UnitBase spawnedUnit = Instantiate(unit.Prefab, pos, Quaternion.identity, _tileMapUnits.transform);

        spawnedUnit.InitUnit(unit, pos);
        spawnedUnit.OccupiedNode = node;
        node.SetOcuppiedUnit(spawnedUnit);

        if (unit.typeInput == TypeInput.None)
        {
            // GameManager.Instance.mapManager.SetNotPath(node);
        }
        else
        {

            //List<GridTileNode> list = new List<GridTileNode>();

            //if (unit.RulesDraw.Count > 0)
            //{
            //    list = GameManager.Instance.mapManager.GetNodeListAsNoPath(node, unit.RulesDraw);

            //    //GameManager.Instance.mapManager.SetNodeAsNoWalkable(oneNode, list);

            //    //if (list.Count > 0)
            //    //{
            //    //    foreach (GridTileNode oneNode in list)
            //    //    {
            //    //        GameManager.Instance.mapManager.SetNodeAsNoWalkable(oneNode);
            //    //        GameManager.Instance.mapManager.SetColorForTile(oneNode._position, Color.magenta);
            //    //    }
            //    //}
            //}
            if (unit.RulesDraw.Count > 0)
            {
                GameManager.Instance.MapManager.GridTileHelper().SetDisableNode(node, unit.RulesDraw, Color.red);
            }

            GameManager.Instance.MapManager.SetColorForTile(pos, Color.magenta);
        }

        return spawnedUnit;
    }

}