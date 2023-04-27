using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    public string idEffect;
    public virtual void RunHero(Player player, BaseEntity entity)
    {

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
    public virtual void SetData(BaseEntity entity)
    {

    }

    public virtual void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
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