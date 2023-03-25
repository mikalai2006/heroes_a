using UnityEngine;

[CreateAssetMenu(fileName = "NewGameMode", menuName = "Game/Instance/New GameMode")]
public class ScriptableGameMode : ScriptableObject
{
    public string idObject;


    public DataGameMode GameModeData;
}

