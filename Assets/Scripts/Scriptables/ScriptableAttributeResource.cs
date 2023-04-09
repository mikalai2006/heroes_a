using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AttributeResource", menuName = "Game/Attribute/Resource")]
public class ScriptableAttributeResource : ScriptableAttribute
{
    public TypeResource TypeResource;
    public int min;
    public int max;
    public int step;
}

[System.Serializable]
public enum TypeResource
{
    Gold = 10,
    Ore = 20,
    Wood = 30,
    Mercury = 40,
    Crystal = 50,
    Gems = 60,
    Sulfur = 70
}
