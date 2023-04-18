using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSetting", menuName = "Game/New GameSetting")]
public class ScriptableGameSetting : ScriptableObject
{
    public string idObject;


    public List<Complexity> Complexities;
}

[System.Serializable]
public struct Complexity
{
    public int value;
    public Sprite sprite;
}