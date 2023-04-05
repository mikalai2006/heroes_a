using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityCreature", menuName = "Game/Entity/New Creature")]
public class ScriptableEntityCreature : ScriptableEntity
{
    [Header("Options Creature")]
    public TypeFaction TypeFaction;
    [SerializeField] public AssetReferenceGameObject ArenaModel;

}
