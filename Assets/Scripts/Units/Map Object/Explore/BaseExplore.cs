using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseExplore : BaseMapObject
{
    public override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        List<GridTileNode> noskyNode = GameManager.Instance.mapManager.DrawSky(OccupiedNode, 10);

        player.SetNosky(noskyNode);

    }
}
