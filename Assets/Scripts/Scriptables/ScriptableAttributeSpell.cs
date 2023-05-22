
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AttributeSpell", menuName = "Game/Attribute/Spell")]
public class ScriptableAttributeSpell : ScriptableAttribute
{
    public Sprite MapSprite;
    public ScriptableAttributeSchoolMagic SchoolMagic;
    public AssetReferenceGameObject AnimatePrefab;
    public AssetReferenceT<AudioClip> AnimateSound;
    public int level;
    public TypeSpell typeSpell;
    public TypeSpellDuration typeSpellDuration;
    public TypeSpellAchievement typeAchievement;
    public List<SpellItem> LevelData;
    public int power;
    public List<ItemProbabiliti<TypeFaction>> ChanceToGain;
    public List<TypeFaction> unAvailability;
    public BaseEffectSpell Effect;

}

[System.Serializable]
public enum TypeSpellAchievement
{
    Friendly = 0,
    Enemy = 1,
    All = 2
}

[System.Serializable]
public enum TypeSpellDuration
{
    Instant = 0,
    Round = 1,
    EndCombat = 2,
}
[System.Serializable]
public enum TypeSpell
{
    Combat = 0,
    Adventure = 1,
}

[System.Serializable]
public struct SpellItem
{
    // public int level;
    public int cost;
    public int Effect;
    public int AI;
    [SerializeField] public LangEntity Text;
}
