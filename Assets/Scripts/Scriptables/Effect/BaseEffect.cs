using System.Collections.Generic;

using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    public virtual void RunHero(ref Player player, BaseEntity entity)
    {
        // throw new System.NotImplementedException();
    }

    public virtual void RunOne(ref Player player, BaseEntity entity)
    {

    }
    public virtual void RunEveryDay(ref Player player, BaseEntity entity)
    {

    }
    public virtual void RunEveryWeek(ref Player player, BaseEntity entity)
    {

    }
}

[System.Serializable]
public struct EffectType
{
    public List<BaseEffect> One;
    public List<BaseEffect> EveryDay;
    public List<BaseEffect> EveryWeek;
    public List<BaseEffect> RunHero;
}