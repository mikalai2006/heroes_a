using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityExplore", menuName = "Game/Entity/Explore")]
public class ScriptableEntityExplore : ScriptableEntity, IPerked
{
    // [Space(10)]
    // [Header("Options Explore")]

    [Space(10)]
    [Header("Options Perk")]
    public int countNosky;
    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        List<GridTileNode> noskyNodes = GameManager.Instance
            .MapManager.DrawSky(entity.OccupiedNode, countNosky);

        player.SetNosky(noskyNodes);
    }
}
