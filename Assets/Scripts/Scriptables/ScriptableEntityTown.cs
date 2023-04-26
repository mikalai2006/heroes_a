using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewEntityTown", menuName = "Game/Entity/Town")]
public class ScriptableEntityTown : ScriptableEntityMapObject
{
    [Header("Options Town")]
    public TypeFaction TypeFaction;
    public List<Sprite> LevelSprites;
    public ScriptableBuildTown BuildTown; // AssetReferenceT<ScriptableBuildTown>
    // public TypeBuild[] StartProgressBuilds;
    public List<ScriptableEntityHero> heroes;
    public List<ScriptableEntityMine> mines;
    public List<ScriptableEntityCreature> creatures;
}

