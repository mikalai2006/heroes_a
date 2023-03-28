using UnityEngine;

[CreateAssetMenu(fileName = "NewWarrior", menuName = "Game/Units/New Warrior")]
public class ScriptableWarriors : ScriptableUnitBase
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