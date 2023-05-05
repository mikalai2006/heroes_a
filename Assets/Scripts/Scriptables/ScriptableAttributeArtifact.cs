using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AttributeArtifact", menuName = "Game/Attribute/Artifact")]
public class ScriptableAttributeArtifact : ScriptableAttribute
{
    public Sprite spriteMap;
    public int Cost;
    public int RMGValue;
    public ClassArtifact ClassArtifact;
    public SlotArtifact Slot;
    public List<ArtifactItemSkill> PrimarySkills;
    public List<ArtifactItemSkill> SecondarySkills;
    public LocalizedString textOk;
}

[System.Serializable]
public struct ArtifactItemSkill
{
    public string id;
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}

[System.Serializable]
public enum ClassArtifact
{
    Treasure = 0,
    Minor = 1,
    Major = 2,
    Relic = 3,
}

[System.Serializable]
public enum SlotArtifact
{
    Combo = 0,
    Cape = 1,
    Feet = 2,
    Helm = 3,
    Shield = 4,
    Misc = 5,
    Necklace = 6,
    Weapon = 7,
    Torso = 8,
    Ring = 9
}