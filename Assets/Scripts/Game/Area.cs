using UnityEngine;

[System.Serializable]
public class Area
{
    public int id;
    public int countNode;
    public Vector3Int startPosition;
    public TypeGround typeGround;
    public bool isFraction;
    public UnitBase town;
    public UnitBase hero;
    public UnitBase portal;
    public int countMine;
    public int countMountain;
    public AreaStat Stat;

    public Area()
    {
        Stat = new AreaStat();
    }
}

[SerializeField]
public struct AreaStat
{
    public int countMine;
    public int countMineN;
    public int countEveryResource;
    public int countEveryResourceN;
    public int countFreeResource;
    public int countFreeResourceN;
    public int countForce;
    public int countForceN;
    public int countExplore;
    public int countExploreN;
    public int countSkillSchool;
    public int countSkillSchoolN;
    public int countArtifactN;
    public int countArtifact;

    public override string ToString()
    {
        return string.Format(" -Mine=[{4}]{0}\n" +
            " -EveryResource=[{5}]{1}\n" +
            " -FreeResource=[{6}]{2}\n" +
            " -Force=[{7}]{3}\n" +
            " -Explore=[{8}]{9} \n" +
            " -SkillSchool=[{10}]{11} \n" +
            " -Artifact=[{12}]{13} \n",
            countMine,
            countEveryResource,
            countFreeResource,
            countForce,
            countMineN,
            countEveryResourceN,
            countFreeResourceN,
            countForceN,

            countExploreN,
            countExplore,

            countSkillSchoolN,
            countSkillSchool,

            countArtifactN,
            countArtifact
            );
    }
}