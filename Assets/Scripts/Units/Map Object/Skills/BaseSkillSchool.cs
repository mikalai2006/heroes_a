using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillSchool : BaseMapObject
{
    public override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        Debug.LogWarning("Show dialog school!");
    }
}
