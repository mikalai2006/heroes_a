using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class CreateRoadOperation : ILoadingOperation
{
    private readonly MapManager _root;
    public string Description => "Create roads ...";
    public CreateRoadOperation(MapManager generator)
    {
        _root = generator;
    }
    public async UniTask Load(Action<float> onProgress)
    {
        List<Area> listArea = LevelManager.Instance.Level.listArea.Where(t =>
            t.portal != null
            || t.town != null
        ).ToList();

        for (int i = 0; i < listArea.Count - 1; i++)
        {

            Area area = listArea[i];
            Area areaNext = listArea[i + 1];

            await _root.OnDrawRoad(
                new Vector3Int(area.startPosition.x, area.startPosition.y, 0),
                new Vector3Int(areaNext.startPosition.x, areaNext.startPosition.y, 0)
            );

            if (listArea[i].portal != null && listArea[i].town != null)
            {
                await _root.OnDrawRoad(
                    new Vector3Int(listArea[i].town.Position.x, listArea[i].town.Position.y, 0),
                    new Vector3Int(listArea[i].portal.Position.x, listArea[i].portal.Position.y, 0)
                );
            }
            if (listArea[i + 1].portal != null && listArea[i + 1].town != null)
            {
                // TODO None town.
                await _root.OnDrawRoad(
                    new Vector3Int(listArea[i + 1].town.Position.x, listArea[i + 1].town.Position.y, 0),
                    new Vector3Int(listArea[i + 1].portal.Position.x, listArea[i + 1].portal.Position.y, 0)
                );
            }
        }

        Area areaFirst = listArea[0];
        Area areaLast = listArea[^1];
        await _root.OnDrawRoad(
            new Vector3Int(areaFirst.startPosition.x, areaFirst.startPosition.y, 0),
            new Vector3Int(areaLast.startPosition.x, areaLast.startPosition.y, 0)
        );
        if (listArea[0].portal != null && listArea[0].town != null)
        {
            await _root.OnDrawRoad(
                new Vector3Int(listArea[0].town.Position.x, listArea[0].town.Position.y, 0),
                new Vector3Int(listArea[0].portal.Position.x, listArea[0].portal.Position.y, 0)
            );
        }

        await UniTask.Delay(1);
    }
}
