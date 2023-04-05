using UnityEngine;

[CreateAssetMenu(fileName = "NewMapObjectMine", menuName = "Game/MapObject/New Mine")]
public class ScriptableMine : ScriptableMapObjectBase
{
    public TypeMine typeMine;
}

public enum TypeMine
{
    Town = 1,
    Free = 2,
}
