using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityTown", menuName = "Game/Entity/Town")]
public class ScriptableEntityTown : ScriptableEntityMapObject
{
    // [Header("Options Town")]
    // public TypeFaction TypeFaction;
    // public List<Sprite> LevelSprites;
    // public ScriptableAttributeTown BuildTown; // AssetReferenceT<ScriptableBuildTown>
    // // public TypeBuild[] StartProgressBuilds;
    // public List<ScriptableEntityHero> heroes;
    // public List<ScriptableEntityMine> mines;
    // public List<ScriptableAttributeCreature> creatures;

    [Header("Options Town")]
    // public string idObject;
    public TypeFaction TypeFaction;
    // public int level;
    // public AssetReference Prefab;
    public Sprite Bg;
    [SerializeField] public List<ScriptableBuilding> Builds;
    // [SerializeField] public List<ScriptableBuildArmy> BuildsArmy;
    // [SerializeField] public AssetReferenceScriptableTown TownMap;
    public List<BuildLevelItem> StartProgressBuilds;
    public List<string> TownNames;
    public List<Sprite> LevelSprites;
    // public ScriptableBuildTown BuildTown; // AssetReferenceT<ScriptableBuildTown>
    // public TypeBuild[] StartProgressBuilds;
    // public List<ScriptableEntityHero> heroes;
    public List<ScriptableEntityMine> mines;
}

[System.Serializable]
public struct BuildLevelItem
{
    public ScriptableBuilding Build;
    public int level;
}

