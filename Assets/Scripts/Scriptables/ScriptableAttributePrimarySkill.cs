using UnityEngine;

[CreateAssetMenu(fileName = "AttributePrimarySkill", menuName = "Game/Attribute/Primary Skill")]
public class ScriptableAttributePrimarySkill : ScriptableAttribute
{
    public TypePrimarySkill TypeSkill;

}

[System.Serializable]
public enum TypePrimarySkill
{
    Attack = 10,
    Defense = 20,
    Power = 30,
    Knowledge = 40,
    // Experience = 50,
}

