using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityTown", menuName = "Game/Entity/New Town")]
public class ScriptableEntityTown : ScriptableEntity
{
    [Header("Options Town")]
    public AssetReferenceT<ScriptableBuildTown> BuildTown; // ScriptableBuildTown
    public TypeBuild[] StartProgressBuilds;
    public List<ScriptableEntityHero> heroes;
    public List<ScriptableEntityMapObject> mines;
    public List<ScriptableWarriors> warriors;
}

