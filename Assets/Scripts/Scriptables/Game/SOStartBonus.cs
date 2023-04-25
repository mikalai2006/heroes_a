using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "StartBonus", menuName = "HeroesA/StartBonus", order = 0)]
public class SOStartBonus : ScriptableObject
{
    public int value;
    public Sprite sprite;
    public TypeStartBonus TypeBonus;
    public LocalizedString title;
}

[System.Serializable]
public enum TypeStartBonus
{
    Random = 0,
    Gold = 1,
    Artifact = 2,
    Resources = 3,
}