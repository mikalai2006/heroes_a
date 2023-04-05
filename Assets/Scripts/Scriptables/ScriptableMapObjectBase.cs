using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewMapObject", menuName = "Game/Map/New MapObject")]
public class ScriptableMapObjectBase : ScriptableObject
{

    public string idObject;

    [Header("General Options")]
    public ScriptableEntity Entity;
    public TypeGround typeGround;
    public TypeInput typeInput;
    public TypeMapObject TypeMapObject;
    // public MapObjectType TypeMapObject;
    public TypeWorkPerk TypeWorkMapObject;
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;

    // Used in game
    [SerializeField] public AssetReferenceGameObject MapPrefab;
    [SerializeField] public BaseMapEntity Prefab;
    // public Sprite MenuSprite;
    [SerializeField] public List<LangItem> Locale;

    // [Header("Options Effects")]
    // public List<GroupResource> Resources;
    // public List<GroupSkill> PrimarySkills;
    // public List<GroupTwoSkill> SecondarySkills;
    // public List<ScriptableEntityArtifact> Artifacts;
    // public List<GroupArtifact> Artifacts;
}

[System.Serializable]
public struct LangItem
{
    public Locale Language;
    public LocaleItem Text;
}

[System.Serializable]
public struct LocaleItem
{
    public string title;
    [TextArea] public string description;
    [TextArea] public string visit_ok;
    [TextArea] public string visit_no;
    [TextArea] public string visit_noresource;
}


// [Serializable]
// public enum TypeMapObject
// {
//     Hero = 0,
//     Town = 1,
//     Portal = 2,
//     MapObject = 3,
//     Monolith = 4,
//     Resource = 5,
//     Mine = 6,
//     Warrior = 7,
// }



// [System.Serializable]
// public struct GroupArtifact
// {
//     public string id;
//     public List<ItemArtifact> ListVariant;
// }

// [System.Serializable]
// public struct ItemArtifact
// {
//     public ScriptableEntityArtifact Artifact;
// }


