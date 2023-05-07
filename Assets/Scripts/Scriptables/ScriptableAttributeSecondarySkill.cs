using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AttributeSecondarySkill", menuName = "Game/Attribute/Secondary Skill")]
public class ScriptableAttributeSecondarySkill : ScriptableAttribute
{
    public TypeSecondarySkill TypeTwoSkill;

    public List<SecondarySkillIem> Levels;
    public List<BaseEffectSkill> Effects;
    public async virtual UniTask RunEffects(Player player, BaseEntity entity)
    {
        foreach (var effect in Effects)
        {
            await effect.RunEffect(player, entity);
        }
    }
}

[System.Serializable]
public struct SecondarySkillIem
{
    public Sprite Sprite;
    public LocalizedString Title;
    public LocalizedString Description;
    public int value;
}

[System.Serializable]
public enum TypeSecondarySkill
{
    AirMagic = 5,
    Archery = 10,
    Armorer = 15,
    Artillery = 20,
    Ballistics = 25,
    Diplomacy = 30,
    EagleEye = 35,
    EarthMagic = 40,
    Estates = 45,

    FireMagic = 47,
    FirstAid = 48,
    Intelligence = 49,
    Leadership = 50,
    Learning = 52,
    Logistics = 54,
    Luck = 55,
    Mysticism = 56,
    Navigation = 57,
    Necromancy = 59,
    Offense = 61,
    Pathfinding = 63,
    Resistance = 64,
    Scholar = 65,
    Scouting = 67,
    Sorcery = 69,
    Tactics = 71,
    WaterMagic = 73,
    Wisdom = 75
}


