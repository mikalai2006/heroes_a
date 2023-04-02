using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewTownGeneral", menuName = "Game/Build/New Town General")]
public class ScriptableBuildGeneral : ScriptableObject
{
    public string idObject;
    public int countLevel;

    public BuildGeneral BuildGeneral;
}

[System.Serializable]
public struct BuildGeneral
{
    public Sprite MenuSprite;
    [SerializeField] public TypeBuild TypeBuild;
    [SerializeField] public TypeBuild RequiredBuilds;
    [SerializeField] public TypeBuildArmy RequiredBuildsArmy;
    public BuildBase UpdatePrefab;
    [SerializeField] public List<LangItem> Locale;
    [SerializeField] public List<BuildCostResource> CostResource;
}

// [System.Serializable]
// public struct BuildCostResource
// {
//     public ScriptableResource Resource;
//     public int Count;
// }

// [System.Serializable]
// public enum TypeFaction
// {
//     Castle = 0,
//     Stronghold = 1,
// }