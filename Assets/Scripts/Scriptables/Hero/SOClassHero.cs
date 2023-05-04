using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ClassHero", menuName = "HeroesA/ClassHero", order = 0)]
public class SOClassHero : ScriptableObject
{
    public string idObject;
    [SerializeField] public LangEntity Text;
    [Range(0.8f, 1.2f)] public float levelAgression; // Aggression ≥ Neutral_army_strength ÷ Hero_army_strength
    public int startAttack;
    public int startDefense;
    public int startPower;
    public int startKnowlenge;

    public List<ProbabilityPrimaryAttribute> ChancesPrimarySkill;
    public List<ItemProbabiliti<ScriptableAttributeSecondarySkill>> ChancesSecondarySkill;
}


[System.Serializable]
public struct ProbabilityPrimaryAttribute
{
    public int minlevel;
    public int maxlevel;
    public ItemProbabiliti<ScriptableAttributePrimarySkill> Item;
}
