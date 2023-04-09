using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityArtifact", menuName = "Game/Entity/Artifact")]
public class ScriptableEntityArtifact : ScriptableEntity, IEffected
{
    [Header("Options Artifact")]
    public Sprite spriteMap;
    // public AnimationCurve Curve;
    // public Sprite sprite;
    public int Cost;

    [Space(10)]
    [Header("Options Perks")]
    public List<ArtifactItemSkill> Skills;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var primarySkill in Skills)
        {
            Debug.Log($"Increment hero skills {primarySkill.PrimarySkill.name}[{primarySkill.value}]");
        }
    }
}

[System.Serializable]
public struct ArtifactItemSkill
{
    public string id;
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}