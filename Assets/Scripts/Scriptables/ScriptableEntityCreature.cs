using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityCreature", menuName = "Game/Entity/Creature")]
public class ScriptableEntityCreature : ScriptableEntityMapObject
{
    [Header("Options Creature")]
    public TypeFaction TypeFaction;
    [SerializeField] public AssetReferenceGameObject ArenaModel;
    [SerializeField] public DataCreatureParams CreatureParams;
}