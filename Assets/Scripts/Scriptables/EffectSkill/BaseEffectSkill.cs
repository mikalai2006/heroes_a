using Cysharp.Threading.Tasks;

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
    public async virtual UniTask RunEffect(Player player, BaseEntity entity)
    {
        await UniTask.Delay(1);
    }
}

