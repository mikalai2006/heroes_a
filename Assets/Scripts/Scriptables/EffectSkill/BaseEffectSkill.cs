using UnityEngine;

[System.Serializable]
public struct ResultEffectSkill
{
    public bool ok;
    public bool no;
    public int value;
}

public abstract class BaseEffectSkill : ScriptableObject
{
    public string idEffect;
    public virtual void RunEffect(Player player, BaseEntity entity)
    {
    }
}

