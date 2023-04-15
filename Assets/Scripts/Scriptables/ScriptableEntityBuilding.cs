using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityBuilding", menuName = "Game/Entity/Building")]
public class ScriptableEntityBuilding : ScriptableEntity
{
    [Header("Options Building")]
    public TypeFaction TypeFaction;
}

