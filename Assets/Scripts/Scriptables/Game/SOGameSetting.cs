using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "GameSetting", menuName = "HeroesA/GameSetting")]
public class SOGameSetting : ScriptableObject
{
    public string idObject;

    [Header("Setting Players")]
    [Space(5)]
    public int maxPlayer;
    public List<Color> colors;
    public Color neutralColor;
    [Range(0.5f, 1f)] public float alphaOverlay;
    public List<ItemPlayerType> TypesPlayer;
    public List<SOComplexity> Complexities;
    public List<SOStartBonus> StartBonuses;
    public List<SOStrenghtMonsters> StrenghtMonsters;
    public List<SOProtectionIndex> ProtectionIndices;
    [SerializeField] public List<CostEntity> CostHero;
    [SerializeField] public List<Sprite> SpriteHall;
    [SerializeField] public List<Sprite> SpriteCastle;
    public int maxCountHero = 5;

    [Header("Setting System")]
    [Space(5)]
    [Range(0.3f, 1f)] public float deltaDoubleClick;
    [Range(10, 1000)] public int timeDelayDoBot;
    [Range(.05f, 1f)] public float speedHero;
    public AnimationCurve probabilityExperience;
    public int countBuildPerDay;
    public int countRecoveryManaPerDay;

    [Header("Setting Movement")]
    [Space(5)]
    public int baseMovementValue;
    public List<ItemDependencyCreatureOnMove> DependencyCreatureOnMove;
    public int countCellClearSky;

    [Header("Setting Arena")]
    [Space(5)]
    [Range(.05f, 1f)] public float speedArenaAnimation;
    public bool paintAllowAttackNode;
    public Color colorAllowAttackNode;
    public Color colorNodeRunSpell;
    public bool paintAllowAttackCreature;
    public Color colorAllowAttackCreature;
    public bool paintActiveCreature;
    public Color colorActiveCreature;

    public SOGameAudio AudioGeneral;
}

[System.Serializable]
public class ItemDependencyCreatureOnMove
{
    public int levelCreature;
    public int movementValue;
}

[System.Serializable]
public class ItemPlayerType
{
    public LocalizedString title;
    public PlayerType TypePlayer;
}