using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DataGameMode
{
    public string title;

    [Header("Setting map")]
    [Space(10)]
    public int width;
    public int height;
    [Range(0.5f, 1.0f)] public float koofSizeArea;


    [Header("Mountains")]
    [Space(10)]
    [Range(0f, 0.2f)] public float noiseScaleMontain;
    [Range(0f, 0.6f)] public float koofMountains;

    [Header("Nature")]
    [Space(10)]
    [Range(0f, 0.2f)] public float koofNature;

    [Header("Mines")]
    [Space(10)]
    [Range(0f, 0.8f)] public float koofMines;

    [Header("Town")]
    [Space(10)]
    [Range(.01f, .2f)] public float koofMinTown;

    [Header("Resource")]
    [Space(10)]
    [Range(0f, 0.01f)] public float koofResource;
    [Range(0f, 0.05f)] public float koofFreeResource;

    [Header("Explore")]
    [Space(10)]
    [Range(0f, 0.01f)] public float koofExplore;

    [Header("Skill")]
    [Space(10)]
    [Range(0f, 0.1f)] public float koofSchoolSkills;

    [Header("Artifact")]
    [Space(10)]
    [Range(0f, 0.1f)] public float koofArtifacts;
}
