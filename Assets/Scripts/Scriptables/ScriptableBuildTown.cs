using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewTown", menuName = "Game/Build/New Town")]
public class ScriptableBuildTown : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    // public int level;
    public AssetReference Prefab;
    public Sprite Bg;
    [SerializeField] public List<ScriptableBuildBase> Builds;
    [SerializeField] public AssetReferenceScriptableTown TownMap;
}

[System.Serializable]
public class AssetReferenceScriptableTown : AssetReferenceT<ScriptableTown>
{
    public AssetReferenceScriptableTown(string guid) : base(guid)
    {
    }
}

// [System.Serializable]
// public struct BuildItem
// {
//     public ScriptableBuildBase Build;

// }