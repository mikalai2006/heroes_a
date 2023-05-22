
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
    public async virtual UniTask AddEffect(ArenaEntity entity, EntityHero heroRunSpell, Player player = null)
    {
        if (CounterSpell == this && entity.Data.SpellsState.ContainsKey(CounterSpell))
        {
            await CounterSpell.RemoveEffect(entity, heroRunSpell);
            entity.Data.SpellsState.Remove(CounterSpell);
        }
        await UniTask.Delay(1);
    }
    public async virtual UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
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
public enum TypeSpellDuration
{
    Instant = 0,
    Round = 1,
    EndCombat = 2,
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
