using System;
using System.Collections.Generic;
using System.Linq;

public class ArenaStat
{
    public static event Action<string> OnAddStat;
    private ArenaManager _arenaManager;
    public List<string> ListStat = new List<string>();
    public Dictionary<ArenaEntityBase, int> DeadedCreatures = new();
    // public Dictionary<ArenaHeroEntity, int> TotalExperiencies = new();
    public Dictionary<ArenaEntityBase, int> DeadedCreaturesAttacked => DeadedCreatures
        .Where(t => t.Key.Hero == _arenaManager.heroLeft)
        .ToDictionary(t => t.Key, t => t.Value);
    public int totalExperienceEnemy => DeadedCreaturesDefendied.Select(t => t.Key.Data.HP * t.Value).Sum();

    public Dictionary<ArenaEntityBase, int> DeadedCreaturesDefendied => DeadedCreatures
        .Where(t => t.Key.Hero != _arenaManager.heroLeft)
        .ToDictionary(t => t.Key, t => t.Value);
    public int totalExperienceHero => DeadedCreaturesDefendied.Select(t => t.Key.Data.HP * t.Value).Sum();

    public void Init(ArenaManager arenaManager)
    {
        _arenaManager = arenaManager;
    }

    public void AddItem(string text)
    {
        ListStat.Add(text);

        string lastStatItem = ListStat[ListStat.Count - 1];
        OnAddStat?.Invoke(lastStatItem);
    }

    public void AddDeadedCreature(ArenaEntityBase arenaEntity, int quantity)
    {
        DeadedCreatures[arenaEntity] = quantity;
    }
}