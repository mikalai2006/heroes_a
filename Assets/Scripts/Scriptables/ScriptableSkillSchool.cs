using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a scriptable map object 
/// </summary>
[CreateAssetMenu(fileName = "NewSkillSchool", menuName = "Units/New Skill School")]
public class ScriptableSkillSchool : ScriptableMapObject {
    public TypeWork TypeWork;
    public List<ItemSkillSchool> ListSkill;
    public AnimationCurve Curve;

}

[System.Serializable]
public struct ItemSkillSchool
{
    public TypeSkillSchool typeSkillSchool;
    public int[] listValue;
}

[System.Serializable]
public enum TypeSkillSchool
{
    Force = 10,
    Protection = 20,
    MagicForce = 30,
    MagicValue = 40,
    Experience = 50,
    TwoSkill = 60
}

[System.Serializable]
public enum TypeTwoSkillSchool
{
    Water = 10,
    Fire = 20,
    Hearth = 30,
    Windy = 40,
    Tactical = 50,
    Medicine = 60
}

