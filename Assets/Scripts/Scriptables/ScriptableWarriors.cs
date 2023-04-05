using UnityEngine;

[CreateAssetMenu(fileName = "NewMapObjectWarrior", menuName = "Game/MapObject/New MapObject Warrior")]
public class ScriptableWarriors : ScriptableMapObjectBase
{
    public DataUnitWarrior DataUnitWarrior;
}


[System.Serializable]
public struct DataUnitWarrior
{

    public int Health;
    public int Attack;

    public int Protection;
    public int Damage;
    public int MaxDamage;

    public int Ammunition;

    public int Speed;

}