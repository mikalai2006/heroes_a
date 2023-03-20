using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameMode", menuName = "Game/New Game Mode")]
public class ScriptableGameMode : ScriptableObject
{
    public string idObject;


    public DataGameMode GameModeData;
}

