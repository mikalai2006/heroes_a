using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ArenaQueue
{
    public List<ArenaEntity> ListEntities = new List<ArenaEntity>();
    // LinkedList<ArenaEntity> queueEntity = new LinkedList<ArenaEntity>();
    public ArenaEntity activeEntity;

    public List<ArenaEntity> SetActiveEntity(ArenaEntity arenaEntity)
    {
        activeEntity = arenaEntity;
        return ListEntities;
    }

    public void NextCreature()
    {
        if (this.activeEntity != null)
        {
            ListEntities.Remove(activeEntity);
            ListEntities.Add(activeEntity);
        }
        else
        {

        }
        activeEntity = ListEntities.ElementAt(0);
    }

    public void AddEntity(ArenaEntity gridGameObject)
    {
        ListEntities.Add(gridGameObject);
        ListEntities = ListEntities
            .OrderBy(t => -((ScriptableAttributeCreature)t.Entity.ScriptableDataAttribute).CreatureParams.Speed)
            .ToList();
    }
}