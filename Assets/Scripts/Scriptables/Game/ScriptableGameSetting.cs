using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewGameSetting", menuName = "Game/New GameSetting")]
public class ScriptableGameSetting : ScriptableObject
{
    public string idObject;

    [Header("Setting Players")]
    [Space(5)]
    public int maxPlayer;
    public List<Color> colors;
    [Range(0.5f, 1f)] public float alphaOverlay;
    public List<ItemPlayerType> TypesPlayer;
    public List<SOComplexity> Complexities;
    public List<SOStartBonus> StartBonuses;
    [SerializeField] public List<CostEntity> CostHero;
    public int maxCountHero = 5;

    [Header("Setting System")]
    [Space(5)]
    [Range(0.3f, 1f)] public float deltaDoubleClick;

}

[System.Serializable]
public class ItemPlayerType
{
    public LocalizedString title;
    public PlayerType TypePlayer;
}