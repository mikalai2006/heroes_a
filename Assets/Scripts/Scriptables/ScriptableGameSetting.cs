using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewGameSetting", menuName = "Game/New GameSetting")]
public class ScriptableGameSetting : ScriptableObject
{
    public string idObject;
    public int maxPlayer;
    public List<Color> colors;
    public List<Complexity> Complexities;

    public Sprite randomTown;

    public List<StartBonusItem> StartBonuses;
    public List<TypePlayerItem> TypesPlayer;
}

[System.Serializable]
public struct StartBonusItem
{
    public TypeStartBonus bonus;
    public Sprite sprite;
    public LocalizedString title;
}


[System.Serializable]
public struct Complexity
{
    public int value;
    public Sprite sprite;
}

[System.Serializable]
public class TypePlayerItem
{
    public string title;
    public PlayerType TypePlayer;
}