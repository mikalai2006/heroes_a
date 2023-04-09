using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/MapObject")]
public class ScriptableEntityMapObject : ScriptableEntityEffect, IEffected
{
    [Header("Options Effect")]

    // public List<ItemProbabiliti<EntityResoureItem>> Resourcess;
    // public void OnDoHero(ref Player player, BaseEntity entity)
    // {
    //     var result = Helpers.GetProbabilityItem<EntityResoureItem>(Resourcess);
    //     // player.ChangeResource(result.Resource.TypeResource, result.value);
    //     // Debug.Log($"PERK: {this.name}::: result[{result.Resource.name}-{result.value}] for {player.DataPlayer.id}");
    // }
    public List<BaseEffect> Effects;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var perk in Effects)
        {
            perk.OnDoHero(ref player, entity);
        }
    }
}

// [System.Serializable]
// public struct EntityResoureItem
// {
//     public string id;
//     public List<ScriptableResource> Resource;
//     public List<ScriptableAttributeArtifact> Artifact;
//     public int value;
//     // public int maxValue;
//     // public int step;
//     // public AnimationCurve Curve;
// }
