using System.Collections.Generic;
using UnityEngine;
using Loader;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.Localization;

public class CreateResourceOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateResourceOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " resources ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.StateNode.HasFlag(StateNode.Empty)
                && !t.StateNode.HasFlag(StateNode.Protected)
                // t.Empty
                // && t.Enable
                // && !t.Protected
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) > 2
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 5
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(
                    LevelManager.Instance.Level.GameModeData.koofFreeResource * area.countNode);

                area.Stat.countFreeResourceN = maxCountResource;

                int countCreated = 0;

                while (countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    List<TypeWorkObject> typeWork = new List<TypeWorkObject>() {
                            TypeWorkObject.One
                        };
                    List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                        .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                        .Where(t =>
                                t.TypeMapObject == TypeMapObject.Resources
                                &&
                            (t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                            && typeWork.Contains(t.TypeWorkObject)
                            )
                        .ToList();
                    var configData = list[UnityEngine.Random.Range(0, list.Count)];
                    if (configData != null)
                    {
                        var factory = new EntityMapObjectFactory();
                        BaseEntity entity = factory.CreateMapObject(
                            TypeMapObject.Resources,
                            configData
                        );
                        UnitManager.SpawnEntityMapObjectToNode(currentNode, entity);
                        // _root.UnitManager
                        //     .SpawnMapObjectAsync(currentNode, TypeMapObject.Resource, new List<TypeWorkPerk>() { TypeWorkPerk.One });

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countFreeResource++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);

                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                    }
                }

            }
        }

        await UniTask.Delay(1);
    }
}
