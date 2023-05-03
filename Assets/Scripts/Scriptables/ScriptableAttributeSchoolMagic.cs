using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AttributeSchoolMagic", menuName = "Game/Attribute/SchoolMagic")]
public class ScriptableAttributeSchoolMagic : ScriptableAttribute
{
    public TypeSchoolMagic typeSchoolMagic;
    public List<Sprite> Sp;
}

[System.Serializable]
public enum TypeSchoolMagic
{
    SchoolofAirMagic = 0,
    SchoolofWaterMagic = 1,
    SchoolofFireMagic = 2,
    SchoolofEarthMagic = 3
}