using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AttributeResource", menuName = "Game/Attribute/Resource")]
public class ScriptableAttributeResource : ScriptableAttribute
{
    public TypeResource TypeResource;
    public int min;
    public int max;
}
