
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AttributeSpell", menuName = "Game/Attribute/Spell")]
public class ScriptableAttributeSpell : ScriptableAttribute
{
    public Sprite MapSprite;
    public ScriptableAttributeSchoolMagic SchoolMagic;
    public int level;
    public TypeSpell typeSpell;
    public TypeSpellDuration typeSpellDuration;
    public List<SpellItem> LevelData;
    public int power;
    public List<ItemProbabiliti<TypeFaction>> ChanceToGain;
    public List<TypeFaction> unAvailability;
    public BaseEffect Effect;

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
