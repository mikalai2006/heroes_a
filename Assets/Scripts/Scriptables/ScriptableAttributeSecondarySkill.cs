using UnityEngine;

[CreateAssetMenu(fileName = "NewAttributeSecondarySkill", menuName = "Game/Attribute/New Secondary Skill")]
public class ScriptableAttributeSecondarySkill : ScriptableAttribute
{
    public TypeSecondarySkill TypeTwoSkill;

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
}


