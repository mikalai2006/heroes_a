using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercuryMine : BaseMines
{
    public override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
        if (newState == GameState.StepNextPlayer)
        {
            Player player = LevelManager.Instance.ActivePlayer;
            if (Data.idPlayer == player.DataPlayer.id)
            {
                player.ChangeResource(TypeResource.Mercury, 4);
            }
        }
    }
}
