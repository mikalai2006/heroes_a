using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public struct QueueItem
{
    public ArenaEntityBase arenaEntity;
    public int round;
}

public class ArenaQueue
{
    public static event Action OnNextRound;
    public static event Action OnNextStep;
    public List<QueueItem> ListEntities = new List<QueueItem>();
    public List<QueueItem> ListEntitiesPhaseWait = new List<QueueItem>();
    // LinkedList<ArenaEntity> queueEntity = new LinkedList<ArenaEntity>();
    public QueueItem activeEntity;
    public int ActiveRound;
    public EntityHero ActiveHero => activeEntity.arenaEntity.Hero;
    private int nextTick = 1;

    public List<QueueItem> SetActiveEntity(QueueItem arenaEntity)
    {
        activeEntity = arenaEntity;
        return ListEntities;
    }

    public void NextCreature(bool wait, bool def)
    {
        if (this.activeEntity.arenaEntity != null && !this.activeEntity.arenaEntity.Death)
        {
            ListEntities.Remove(activeEntity);

            if (def)
            {
                activeEntity.arenaEntity.Data.isDefense = true;
            }
            else
            {
                activeEntity.arenaEntity.Data.isDefense = false;
            }

            if (wait)
            {
                // activeEntity.arenaEntity.Data.isDefense = false;
                activeEntity.arenaEntity.Data.waitTick = nextTick;
                nextTick++;
                var allRoundItems = ListEntities.Where(t => t.round == activeEntity.round);
                var indexLastInRound = allRoundItems.Count();
                ListEntities.Insert(indexLastInRound, activeEntity);
            }
            else
            {
                // activeEntity.arenaEntity.Data.isDefense = false;
                activeEntity.round += 1;
                activeEntity.arenaEntity.Data.waitTick = 0;
                ListEntities.Add(activeEntity);
            }

        }

        Refresh();
        activeEntity = ListEntities.ElementAt(0);

        if (ActiveRound != activeEntity.round)
        {
            OnNextRound?.Invoke();
        }
        ActiveRound = activeEntity.round;

        OnNextStep?.Invoke();
    }

    public void AddEntity(ArenaEntityBase entity)
    {
        QueueItem item = new QueueItem()
        {
            arenaEntity = entity,
            round = 1
        };
        ListEntities.Add(item);
        // ListEntities = ListEntities
        //     .OrderBy(t => -((ScriptableAttributeCreature)t.arenaEntity.Entity.ScriptableDataAttribute).CreatureParams.Speed)
        //     .ToList();
        Refresh();
    }

    internal void RemoveEntity(ArenaEntityBase arenaEntity)
    {
        var index = ListEntities.FindIndex(t => t.arenaEntity == arenaEntity);
        if (index != -1)
        {
            ListEntities.RemoveAt(index);
        }
        Refresh();
    }

    public List<QueueItem> Refresh()
    {
        ListEntities = ListEntities
            .OrderBy(t => t.round)
            .ThenBy(t => t.arenaEntity.Data.waitTick)
            .ThenBy(t => -t.arenaEntity.Speed)
            .ToList();
        return ListEntities;
    }
}