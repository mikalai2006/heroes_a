using Cysharp.Threading.Tasks;

using Loader;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Localization;

using Random = UnityEngine.Random;

public class CreateArtifactOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateArtifactOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " artifacts ...");

        //Get all attributes artifacts.
        List<ScriptableAttributeArtifact> listArtifacts
            = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
            // .Where(t => t.TypeMapObject == TypeMapObject.Artifact)
            .ToList();

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.StateNode.HasFlag(StateNode.Empty)
                && !t.StateNode.HasFlag(StateNode.Road)
                && !t.StateNode.HasFlag(StateNode.Protected)
                // t.Empty
                // && t.Enable
                // && !t.Protected
                && t.KeyArea == area.id
                // && _root.gridTileHelper.CalculateNeighbours(t) <= 3
                // && _root.gridTileHelper.GetDisableNeighbours(t).count == 2
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCount = Mathf.CeilToInt(LevelManager.Instance.Level.GameModeData.koofArtifacts * area.countNode);

                area.Stat.countArtifactN = maxCount;

                int countCreated = 0;

                while (countCreated < maxCount && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];
                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    ScriptableEntityMapObject configDataArtifact
                        = ResourceSystem.Instance
                        .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.Other)
                        .First();
                    ScriptableEntityOther configData = (ScriptableEntityOther)configDataArtifact;
                    if (
                        nodeWarrior != null
                        && currentNode != null
                        && _root.gridTileHelper.GetAllowInsertObjectToNode(currentNode, configData)
                        )
                    {
                        // Generate artefact attribute.
                        ScriptableAttributeArtifact configArtifact
                            = listArtifacts[Random.Range(0, listArtifacts.Count)];

                        EntityArtifact entity = new EntityArtifact(configData, configArtifact);
                        // var entityArtifact = (ScriptableEntityArtifact)entity.ScriptableData;
                        UnitManager.SpawnEntityMapObjectToNode(currentNode, entity);

                        // Generate protection creature.

                        MapObject warrior = UnitManager.SpawnEntityCreature(nodeWarrior, currentNode, 3, configArtifact.RMGValue);

                        // nodeWarrior.SetProtectedNeigbours(warrior, currentNode);
                        // currentNode.SetProtectedNode(warrior);
                        // _root.SetColorForTile(nodeWarrior.position, Color.blue);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countArtifact++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                        //onProgress?.Invoke((float)countCreated / (float)maxCount);
                    }
                    else
                    {
                        nodes.Remove(currentNode);
                    }
                }

            }
        }
        await UniTask.Delay(1);
    }
}
