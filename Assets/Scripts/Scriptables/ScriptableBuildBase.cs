using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewBuild", menuName = "Game/Build/New Build")]
public class ScriptableBuildBase : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    public int level;
    [SerializeField] public List<Build> Builds;
    [SerializeField] public List<ScriptableBuildBase> RequireBuilds;
    [SerializeField] public List<BuildCostResource> RequireResource;
}

[System.Serializable]
public struct Build
{
    public Sprite MenuSprite;
    public BuildBase Prefab;
    [SerializeField] public List<LangItem> Locale;
}

[System.Serializable]
public struct BuildCostResource
{
    public ScriptableResource Resource;
    public int Count;
}

[System.Serializable]
public enum TypeFaction
{
    Castle = 0,
    Stronghold = 1,
}
