using UnityEngine;

[CreateAssetMenu(fileName = "NewMine", menuName = "Game/Units/New Mine")]
public class ScriptableMine : ScriptableUnitBase
{
    public TypeMine typeMine;
}

public enum TypeMine
{
    Town = 1,
    Free = 2,
}
