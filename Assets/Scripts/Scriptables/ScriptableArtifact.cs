using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Game/Units/New Artifact")]
public class ScriptableArtifact : ScriptableMapObject
{
    // public List<ItemSkillArtifact> Skills;
    public AnimationCurve Curve;
    public Sprite sprite;
    public Sprite spriteMap;

    public int Cost;

}

// [System.Serializable]
// public struct ItemSkillArtifact
// {
//     public ScriptableSkill Skill;
//     public int Value;
// }