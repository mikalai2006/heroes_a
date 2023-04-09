using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AttributeArtifact", menuName = "Game/Attribute/Artifact")]
public class ScriptableAttributeArtifact : ScriptableAttribute
{
    public Sprite spriteMap;
    public int Cost;
    public List<ArtifactItemSkill> PrimarySkills;
    public List<ArtifactItemSkill> SecondarySkills;
}
[System.Serializable]
public struct ArtifactItemSkill
{
    public string id;
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}