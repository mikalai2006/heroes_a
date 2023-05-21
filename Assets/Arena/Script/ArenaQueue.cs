using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct QueueItem
{
    public ArenaEntity arenaEntity;
    public int round;
}

public class ArenaQueue
{
    public static event Action OnNextRound;
    public static event Action OnNextStep;
    public List<QueueItem> ListEntities = new List<QueueItem>();
    // LinkedList<ArenaEntity> queueEntity = new LinkedList<ArenaEntity>();
    public QueueItem activeEntity;
    public int ActiveRound;
    public EntityHero ActiveHero => activeEntity.arenaEntity.Hero;

    public List<QueueItem> SetActiveEntity(QueueItem arenaEntity)
    {
        activeEntity = arenaEntity;
        return ListEntities;
    }

    public void NextCreature(bool wait)
    {
        if (this.activeEntity.arenaEntity != null && !this.activeEntity.arenaEntity.Death)
        {
            ListEntities.Remove(activeEntity);
            if (wait)
            {
                activeEntity.arenaEntity.Data.isDefense = false;
                var allRoundItems = ListEntities.Where(t => t.round == activeEntity.round);
                var indexLastInRound = allRoundItems.Count();
                ListEntities.Insert(indexLastInRound, activeEntity);
            }
            else
            {
                activeEntity.arenaEntity.Data.isDefense = true;
                activeEntity.round += 1;
                ListEntities.Add(activeEntity);
            }
        }

        activeEntity = ListEntities.ElementAt(0);

        if (ActiveRound != activeEntity.round)
        {
            OnNextRound?.Invoke();
        }
        ActiveRound = activeEntity.round;

        OnNextStep?.Invoke();
    }

    public void AddEntity(ArenaEntity entity)
    {
        QueueItem item = new QueueItem()
        {
            arenaEntity = entity,
            round = 1
        };
        ListEntities.Add(item);
        ListEntities = ListEntities
            .OrderBy(t => -((ScriptableAttributeCreature)t.arenaEntity.Entity.ScriptableDataAttribute).CreatureParams.Speed)
            .ToList();
    }

    internal void RemoveEntity(ArenaEntity arenaEntity)
    {
        var index = ListEntities.FindIndex(t => t.arenaEntity == arenaEntity);
        if (index != -1)
        {
            ListEntities.RemoveAt(index);
        }
    }
}