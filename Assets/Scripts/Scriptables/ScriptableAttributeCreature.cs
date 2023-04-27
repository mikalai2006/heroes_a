using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewAttributeCreature", menuName = "Game/Attribute/Creature")]
public class ScriptableAttributeCreature : ScriptableAttribute
{
    [Header("Options Creature")]
    public TypeFaction TypeFaction;
    // [SerializeField] public new AssetReferenceGameObject MapPrefab;
    [SerializeField] public AssetReferenceGameObject ArenaModel;
    [SerializeField] public DataCreatureParams CreatureParams;
}
[System.Serializable]
public struct DataCreatureParams
{
    public int Level;
    public int Attack;
    public int Defense;
    public int DamageMin;
    public int DamageMax;
    public int HP;
    public int Speed;
    public int Shoots;
    public int Growth;
    public int Size;
    public MovementType Movement;
    public int AI;
    public List<CostEntity> Cost;
}

[System.Serializable]
public enum MovementType
{
    Ground = 0,
    Flying = 10
}

[System.Serializable]
public struct CostEntity
{
    public ScriptableAttributeResource Resource;
    public int Count;
}