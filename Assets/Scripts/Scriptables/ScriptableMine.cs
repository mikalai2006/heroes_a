using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a scriptable mine
/// /// </summary>
[CreateAssetMenu(fileName = "New Mine", menuName = "Units/New Mine")]
public class ScriptableMine : ScriptableUnitBase
{
    public TypeMine typeMine;
}

public enum TypeMine
{
    Town = 1,
    Free = 2,
}
