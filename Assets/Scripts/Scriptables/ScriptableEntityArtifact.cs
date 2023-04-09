using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityArtifact", menuName = "Game/Entity/MapObject/Artifact")]
public class ScriptableEntityArtifact : ScriptableEntityMapObject, IEffected
{
    [Header("Options Artifact")]
    public ScriptableAttributeArtifact Artifact;
    // public Sprite spriteMap;
    // public AnimationCurve Curve;
    // public Sprite sprite;
    // public int Cost;

    // [Space(10)]
    // [Header("Options Perks")]
    // public List<ArtifactItemSkill> Skills;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        // foreach (var primarySkill in Skills)
        // {
        //     Debug.Log($"Increment hero skills {primarySkill.PrimarySkill.name}[{primarySkill.value}]");
        // }
    }
}

