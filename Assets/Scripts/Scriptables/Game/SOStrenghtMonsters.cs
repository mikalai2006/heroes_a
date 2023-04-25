using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "StrenghtMonsters", menuName = "HeroesA/StrenghtMonsters", order = 0)]
public class SOStrenghtMonsters : ScriptableObject
{
    public LocalizedString title;
    public Sprite sprite;
    public StrenghtMonster strenghtMonster;
    public int monstersStrengthZone;
    public int monstersStrengthMap;
    public int minimalValue1;
    [Range(0f, 1.5f)] public float koof1;
    public int minimalValue2;
    [Range(0f, 1.5f)] public float koof2;
}

[System.Serializable]
public enum StrenghtMonster
{
    Weak = 1,
    Normal = 2,
    Strong = 3,
}