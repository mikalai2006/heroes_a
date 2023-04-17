using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Localization;

public class CreateEdgesOperation : ILoadingOperation
{
    private Action<string> _onSetNotify;
    private int countEdgeTile = 3;

    private readonly MapManager _root;

    public CreateEdgesOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        _onSetNotify = onSetNotify;
        var task1 = CreateEdges();

        await UniTask.WhenAll(task1);

        //await UniTask.Delay(1);
    }

    /// <summary>
    /// Create edge borders map
    /// </summary>
    /// <returns>UniTask</returns>
    private async UniTask CreateEdges()
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        _onSetNotify(t + " edges ...");

        for (int x = -countEdgeTile; x < _root.gameModeData.width + countEdgeTile; x++)
        {
            for (int y = -countEdgeTile; y < _root.gameModeData.height + countEdgeTile; y++)
            {
                if (x < 0 || y < 0 || x > _root.gameModeData.width - 1 || y > _root.gameModeData.height - 1)
                {
                    _root._tileMapEdge.SetTile(new Vector3Int(x, y), _root._tileEdge);
                }
            }
        }

        await UniTask.Delay(1);
    }



}
