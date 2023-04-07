using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityResource", menuName = "Game/Entity/Group Resource")]
public class ScriptableEntityResource : ScriptableEntity, IPerked
{
    [Header("Options Perk")]
    public List<ItemProbabiliti<EntityResoureItem>> Resources;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        var result = Helpers.GetProbabilityItem<EntityResoureItem>(Resources);
        Debug.Log($"PERK: {this.name}::: result[{result.Resource.name}-{result.value}] for {player.DataPlayer.id}");
    }
}

[System.Serializable]
public struct EntityResoureItem
{
    public string id;
    public ScriptableResource Resource;
    public int value;
    // public int maxValue;
    // public int step;
    // public AnimationCurve Curve;
}