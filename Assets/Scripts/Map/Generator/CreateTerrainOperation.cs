using Cysharp.Threading.Tasks;

using Loader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Localization;

using Random = UnityEngine.Random;

public class CreateTerrainOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateTerrainOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + "terrain ...");

        for (int x = 0; x < _root.gameModeData.width; x++)
        {
            for (int y = 0; y < _root.gameModeData.height; y++)
            {
                GridTileNode tileNode = _root.gridTileHelper.GridTile.GetGridObject(new Vector3Int(x, y));

                RuleTile drawRule = _root._dataTypeGround[tileNode.TypeGround].tileRule;

                Vector3Int pos = new Vector3Int(x, y, 0);

                _root._tileMap.SetTile(pos, drawRule);

                // tileNode.SetState(TypeStateNode.Enable);
                // tileNode.AddStateNode(StateNode.Empty);

                if (x == 0 || y == 0 || x == _root.gameModeData.width - 1 || y == _root.gameModeData.height - 1)
                {
                    tileNode.isEdge = true;
                }

                Color c = Color.blue;

                c.a = (tileNode.KeyArea * .5f) * (tileNode.KeyArea * .2f);

                // _root.SetColorForTile(tileNode.position, c);

                _root._tileMap.SetTile(pos, drawRule);
            }
        }

        await NormalizeArea();

        await UniTask.Delay(1);

    }

    /// <summary>
    /// Normalize area, remove small area from list area.
    /// </summary>
    /// <returns></returns>
    private async UniTask NormalizeArea()
    {
        _root.countArea = LevelManager.Instance.Level.listArea.Count;

        List<Area> areaList = LevelManager.Instance.Level.listArea.Where(t => t.countNode < 30).ToList();

        while (areaList.Count > 0)
        {
            Area area = areaList[0];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t => t.KeyArea == area.id).ToList();

            for (int y = 0; y < nodes.Count; y++)
            {
                GridTileNode node = nodes[y];
                _root._tileMap.SetTile(node.position, _root._dataTypeGround[node.TypeGround].tileRule);

                List<TileNature> listTileForDraw = ResourceSystem.Instance.GetNature().Where(t =>
                    ((t.typeGround & node.TypeGround) == node.TypeGround)
                    && !t.isWalkable
                ).ToList();

                TileNature tileForDraw = listTileForDraw[Random.Range(0, listTileForDraw.Count)];

                _root._tileMapNature.SetTile(node.position, tileForDraw);

                _root._listNatureNode.Add(new GridTileNatureNode(node, tileForDraw.idObject, false, tileForDraw.name));

                //root.SetColorForTile(node._position, Color.green);

                _root.gridTileHelper.SetDisableNode(node, tileForDraw.listTypeNoPath, Color.black);

            }

            LevelManager.Instance.RemoveArea(area);

            areaList.RemoveAt(0);

        }

        _root.countArea = LevelManager.Instance.Level.listArea.Count;
        await UniTask.Delay(1);
    }
}
