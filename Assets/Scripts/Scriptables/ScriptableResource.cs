using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a scriptable map object 
/// </summary>
[CreateAssetMenu(fileName = "New Resource", menuName = "Units/New Resource")]
public class ScriptableResource : ScriptableUnitBase {
    public TypeWork TypeWork;
    public List<ItemResource> ListResource;
    public AnimationCurve Curve;

}

[System.Serializable]
public struct ItemResource
{
    public TypeResource TypeResource;
    public int[] listValue;
}

[System.Serializable]
public enum TypeResource
{
    Gold = 10,
    Iron = 20,
    Wood = 30,
    Mercury = 40,
    Diamond = 50,
    Gem = 60,
    Sulfur = 70
}

[System.Serializable]
public enum TypeWork
{
    One = 1,
    EveryDay = 2,
    EveryWeek = 3,
}

