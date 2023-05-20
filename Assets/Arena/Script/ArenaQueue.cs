using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public struct QueueItem
{
    public ArenaEntity arenaEntity;
    public int round;
}
public class ArenaQueue
{

    public List<QueueItem> ListEntities = new List<QueueItem>();
    // LinkedList<ArenaEntity> queueEntity = new LinkedList<ArenaEntity>();
    public QueueItem activeEntity;

    public List<QueueItem> SetActiveEntity(QueueItem arenaEntity)
    {
        activeEntity = arenaEntity;
        return ListEntities;
    }

    public void NextCreature(bool wait)
    {
        if (this.activeEntity.arenaEntity != null)
        {
            ListEntities.Remove(activeEntity);
            if (wait)
            {
                var allRoundItems = ListEntities.Where(t => t.round == activeEntity.round);
                var indexLastInRound = allRoundItems.Count();
                ListEntities.Insert(indexLastInRound, activeEntity);
            }
            else
            {
                activeEntity.round += 1;
                ListEntities.Add(activeEntity);
            }
        }

        activeEntity = ListEntities.ElementAt(0);
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
}