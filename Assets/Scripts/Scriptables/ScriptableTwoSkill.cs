using UnityEngine;

[CreateAssetMenu(fileName = "NewTwoSkill", menuName = "Game/Instance/New TwoSkill")]
public class ScriptableTwoSkill : ScriptableObject
{
    public string idObject;
    public TypeTwoSkill TypeTwoSkill;
    public int maxValue;
    public int step;
    public Sprite SpriteMenu;
    public Sprite SpriteMap;

}

[System.Serializable]
public enum TypeTwoSkill
{
    Water = 10,
    Fire = 20,
    Hearth = 30,
    Windy = 40,
    Tactical = 50,
    Medicine = 60
}


