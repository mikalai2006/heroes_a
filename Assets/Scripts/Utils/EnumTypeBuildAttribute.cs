using System;

using UnityEngine;

public class EnumTypeBuildAttribute : PropertyAttribute
{
    public readonly string[] names;

    // [SerializeField] public TestTypeBuild TestTypeBuild;

    public EnumTypeBuildAttribute(Type enumType)
    {
        names = Enum.GetNames(enumType);
    }
}

public enum TestTypeBuild
{
    None = 1,
    Tavern = 2,
    Town_1 = 3,
    Town_2 = 4,
    Town_3_ = 5,
    Town_4 = 6,
    Castle_1 = 7,
    Castle_2 = 8,
    Castle_3 = 9,
    Castle_4 = 10,
    Blacksmith = 11,
    MarketPlace = 12,
    ResourceSilo = 13,
    MageGuild_1 = 14,
    MageGuild_2 = 15,
    MageGuild_3 = 16,
    MageGuild_4 = 17,
    MageGuild_5 = 18,

    Army_1_1 = 19,
    Army_1_2 = 20,
    Army_2_1 = 21,
    Army_2_2 = 22,
    Army_3_1 = 23,
    Army_3_2 = 24,
    Army_4_1 = 25,
    Army_4_2 = 26,
    Army_5_1 = 27,
    Army_5_2 = 28,
    Army_6_1 = 29,
    Army_6_2 = 30,
    Army_7_1 = 31,
    Army_7_2 = 32,

    Specific_1_1 = 33,
    Specific_1_2 = 34,
    Specific_2_1 = 35,
    Specific_2_2 = 36,
    Specific_3_1 = 37,
    Specific_3_2 = 38,
    Specific_4_1 = 39,
    Specific_4_2 = 40,
    Specific_5_1 = 41,
    Specific_5_2 = 42,
    Specific_6_1 = 43,
    Specific_6_2 = 44,


}