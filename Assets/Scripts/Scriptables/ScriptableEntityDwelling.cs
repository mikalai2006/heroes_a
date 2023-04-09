using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityDwelling", menuName = "Game/Entity/Dwelling")]
public class ScriptableEntityDwelling : ScriptableEntityMapObject
{
    [Header("Options Dwelling")]
    // [Space(3)]
    public TypeFaction TypeFaction;
    [SerializeField] public TypeBuild TypeBuild;
    // public TypeWorkPerk TypeWork;
    // public AssetReferenceGameObject UpdatePrefab;
    [SerializeField] public TypeBuild[] RequiredBuilds;
    // [SerializeField] public LangBuild Text;
    [SerializeField] public List<BuildCostResource> CostResource;


    // public TypeMapObject TypeMapObject;
    // public List<GroupResource> Resources;
    // public List<GroupSkill> PrimarySkills;
    // public List<GroupTwoSkill> SecondarySkills;
    // public List<GroupTwoSkill> TwoSkills;
    // public List<GroupArtifact> Artifacts;
}
