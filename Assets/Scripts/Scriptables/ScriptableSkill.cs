using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Instance/New Skill")]
public class ScriptableSkill : ScriptableObject
{
    public string idObject;
    public TypeSkill TypeSkill;
    public int maxValue;
    public int step;
    // public AnimationCurve Curve;
    public Sprite SpriteMenu;
    // public Sprite SpriteMap;

}

[System.Serializable]
public enum TypeSkill
{
    Force = 10,
    Protection = 20,
    MagicForce = 30,
    MagicValue = 40,
    Experience = 50,
}

