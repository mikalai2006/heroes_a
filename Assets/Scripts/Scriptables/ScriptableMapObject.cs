using System;
using UnityEngine;

/// <summary>
/// Create a scriptable map object 
/// </summary>
[CreateAssetMenu(fileName = "New Object", menuName = "Units/New map object")]
public class ScriptableMapObject : ScriptableUnitBase {
    public MapObjectType mapObjectType;

}

//[Serializable]
public enum MapObjectType
{
    Enemy = 0,
    Artifact = 1,

    Explore = 20,

    SkillSchool = 30,
}

