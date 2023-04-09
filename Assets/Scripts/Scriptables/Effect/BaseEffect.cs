using System.Collections.Generic;

using UnityEngine;

public abstract class BaseEffect : ScriptableObject, IEffected
{
    public virtual void OnDoHero(ref Player player, BaseEntity entity)
    {
        // throw new System.NotImplementedException();
    }
}
