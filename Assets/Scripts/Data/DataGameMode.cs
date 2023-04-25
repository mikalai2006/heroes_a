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
    [Range(0f, 0.2f)] public float noiseScaleMontain;
    [Range(0f, 0.6f)] public float koofMountains;
    [Range(0f, 0.2f)] public float koofNature;
    [Range(0f, 0.8f)] public float koofMines;
    [Range(.01f, .2f)] public float koofMinTown;
    [Range(0f, 0.01f)] public float koofResource;
    [Range(0f, 0.05f)] public float koofFreeResource;
    [Range(0f, 0.01f)] public float koofExplore;
    [Range(0f, 0.1f)] public float koofSchoolSkills;
    [Range(0f, 0.1f)] public float koofArtifacts;
    [Range(0f, 0.1f)] public float koofDwelling;
}

[System.Serializable]
public class DataGameSetting
{
    public int compexity;
    public StrenghtMonster strenghtMonster;
    public int countPlayer;
    public int countCommand;
    public int countBot;
    public int countBotCommand;
    public bool isWater;
    public TypeGame TypeGame;
}

[System.Serializable]
public enum TypeGame
{
    Single = 0,
    MultipleOneDevice = 1,
}