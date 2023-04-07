using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityTown", menuName = "Game/Entity/Town")]
public class ScriptableEntityTown : ScriptableEntity
{
    [Header("Options Town")]
    public AssetReferenceT<ScriptableBuildTown> BuildTown; // ScriptableBuildTown
    public TypeBuild[] StartProgressBuilds;
    public List<ScriptableEntityHero> heroes;
    public List<ScriptableEntityMine> mines;
    public List<ScriptableEntityCreature> creatures;
}

