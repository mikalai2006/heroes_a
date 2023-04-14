using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewBuildingTown", menuName = "Game/Building/New Town")]
public class ScriptableBuildTown : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    // public int level;
    public AssetReference Prefab;
    public Sprite Bg;
    [SerializeField] public List<ScriptableBuildBase> Builds;
    // [SerializeField] public List<ScriptableBuildArmy> BuildsArmy;
    // [SerializeField] public AssetReferenceScriptableTown TownMap;
    public List<BuildLevelItem> StartProgressBuilds;
}

// [System.Serializable]
// public class AssetReferenceScriptableTown : AssetReferenceT<ScriptableTown>
// {
//     public AssetReferenceScriptableTown(string guid) : base(guid)
//     {
//     }
// }

[System.Serializable]
public struct BuildLevelItem
{
    public ScriptableBuildBase Build;
    public int level;
}