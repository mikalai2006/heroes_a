
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

// [CreateAssetMenu(fileName = "AttributeSpell", menuName = "Game/Attribute/Spell")]
public class ScriptableAttributeSpell : ScriptableAttribute
{
    public Sprite MapSprite;
    public ScriptableAttributeSchoolMagic SchoolMagic;
    public AssetReferenceGameObject AnimatePrefab;
    public AssetReferenceT<AudioClip> AnimateSound;
    public int level;
    public TypeSpell typeSpell;
    public TypeSpellDuration typeSpellDuration;
    public TypeSpellRun typeSpellRun;
    public TypeSpellTarget typeSpellTarget;
    public TypeSpellAchievement typeAchievement;
    public List<SpellItem> LevelData;
    public int power;
    public List<ItemProbabiliti<TypeFaction>> ChanceToGain;
    public List<TypeFaction> unAvailability;
    public ScriptableAttributeSpell CounterSpell;
    // public BaseEffectSpell Effect;

    /// <summary>
    /// Run effect spell for entity
    /// </summary>
    /// <param name="entity">current entity</param>
    /// <param name="heroRunSpell">hero run spell</param>
    /// <param name="player"></param>
    /// <returns></returns>
    public async virtual UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        if (CounterSpell == this && node.OccupiedUnit.Data.SpellsState.ContainsKey(CounterSpell))
        {
            await CounterSpell.RemoveEffect(node, heroRunSpell);
            node.OccupiedUnit.Data.SpellsState.Remove(CounterSpell);
        }
        await UniTask.Delay(1);
    }

    public async virtual UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        await UniTask.Delay(1);
        return new();
    }

    public async virtual UniTask RunEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        await UniTask.Delay(1);
    }

    public async virtual UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        await UniTask.Delay(1);
    }

}

[System.Serializable]
public enum TypeSpellAchievement
{
    Friendly = 0,
    Enemy = 1,
    All = 2
}

[System.Serializable]
public enum TypeSpellRun
{
    Collective = 0,
    Individual = 1,
}
[System.Serializable]
public enum TypeSpellTarget
{
    Creature = 0,
    Node = 1,
}
[System.Serializable]
public enum TypeSpellDuration
{
    Instant = 0,
    Round = 1,
    EndCombat = 2,
    RoundOrAction = 3,
}
[System.Serializable]
public enum TypeSpell
{
    Combat = 0,
    Adventure = 1,
}

[System.Serializable]
public struct SpellItem
{
    // public int level;
    public int cost;
    public int Effect;
    public int AI;
    [SerializeField] public LangEntity Text;
}
