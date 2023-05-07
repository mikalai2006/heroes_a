using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    public string idEffect;
    public async virtual UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();
        await UniTask.Delay(1);
        return result;
    }

    public virtual void RunOne(ref Player player, BaseEntity entity)
    {

    }
    public virtual void RunEveryDay(Player player, BaseEntity entity)
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
public struct EffectResult
{
    public bool ok;
    public bool no;
}

[System.Serializable]
public struct EffectType
{
    public List<BaseEffect> One;
    public List<BaseEffect> EveryDay;
    public List<BaseEffect> EveryWeek;
    public List<BaseEffect> RunHero;
}