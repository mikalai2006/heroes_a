using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a scriptable map object 
/// </summary>
[CreateAssetMenu(fileName = "NewArtifact", menuName = "Units/New Artifact")]
public class ScriptableArtifact : ScriptableMapObject {
    public ItemSkillArtifact Skill;
    public AnimationCurve Curve;
    public Sprite sprite;
    public Sprite spriteMap;


}

[System.Serializable]
public struct ItemSkillArtifact
{
    public List<SkillArtifact> listSkills;
}

[System.Serializable]
public struct SkillArtifact
{
    public TypeSkillSchool typeSkill;
    public int valueSkill;
}
