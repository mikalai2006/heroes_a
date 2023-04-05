using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityResource", menuName = "Game/Entity/New Resource")]
public class ScriptableEntityResource : ScriptableEntity
{
    [Header("Options Entity Resource")]
    public TypeResource TypeResource;
    public int maxValue;
    public int step;
    public AnimationCurve Curve;
}

[System.Serializable]
public enum TypeResource
{
    Gold = 10,
    Ore = 20,
    Wood = 30,
    Mercury = 40,
    Crystal = 50,
    Gems = 60,
    Sulfur = 70
}
