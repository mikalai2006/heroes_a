using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AttributeArtifact", menuName = "Game/Attribute/Artifact")]
public class ScriptableAttributeArtifact : ScriptableAttribute
{
    // public string idObject;
    // public Sprite MenuSprite;
    // [SerializeField] public LangEntity Text;
    // public TypeResource TypeResource;
    // public int maxValue;
    // public int step;
    // public AnimationCurve Curve;
    public Sprite spriteMap;
    public int Cost;
    public List<ArtifactItemSkill> PrimarySkills;
    public List<ArtifactItemSkill> SecondarySkills;
}
