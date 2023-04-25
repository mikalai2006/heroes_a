using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Complexity", menuName = "HeroesA/Complexity", order = 0)]
public class SOComplexity : ScriptableObject
{
    public int value;
    public Sprite sprite;
    public ComplexityItem Player;
    public ComplexityItem Computer;

}

[System.Serializable]
public struct StartResourceItem
{
    public ScriptableAttributeResource Resource;
    public int value;
}

[System.Serializable]
public struct ComplexityItem
{
    public List<StartResourceItem> Resources;
    public int goldin;
}