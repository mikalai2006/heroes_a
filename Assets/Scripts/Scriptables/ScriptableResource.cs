using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

// [CreateAssetMenu(fileName = "NewResource", menuName = "Game/Instance/New Resource")]
public class ScriptableResource : ScriptableMapObject
{
    [Header("Options resource")]
    public TypeResource TypeResource;
}


