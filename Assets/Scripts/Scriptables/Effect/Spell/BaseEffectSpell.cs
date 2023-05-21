using Cysharp.Threading.Tasks;

using UnityEngine;

public abstract class BaseEffectSpell : ScriptableObject
{
    public string idEffect;
    public async virtual UniTask AddEffect(ArenaEntity entity, Player player = null)
    {
        await UniTask.Delay(1);
    }
    public async virtual UniTask RemoveEffect(ArenaEntity entity, Player player = null)
    {
        await UniTask.Delay(1);
    }
}

