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
    [SerializeField] public DataCreatureParams CreatureParams;
}
[System.Serializable]
public struct DataCreatureParams
{
    public int Health;
    public int Attack;
    public int Protection;
    public int Damage;
    public int MaxDamage;
    public int Ammunition;
    public int Speed;
}