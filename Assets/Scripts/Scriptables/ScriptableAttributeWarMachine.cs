using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "NewAttributeWarMachine", menuName = "Game/Attribute/WarMachine")]
public class ScriptableAttributeWarMachine : ScriptableAttributeCreature
{
    public TypeWarMachine TypeWarMachine;

    // public virtual async UniTask AddEffect(GridArenaNode node, EntityHero heroRunAttack, ArenaManager arenaManager, Player player = null)
    // {

    //     await UniTask.Delay(1);
    // }

    public virtual async UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        await UniTask.Delay(1);
        return new();
    }

    public virtual async UniTask RunEffect(ArenaManager arenaManager, GridArenaNode node, GridArenaNode nodeForAttack, Player player = null)
    {
        await UniTask.Yield();
    }

    public virtual async UniTask RunEffectByGameObject(ArenaManager arenaManager, GridArenaNode node, GameObject gameObject)
    {
        await UniTask.Delay(1);
    }
}

public struct ArenaResultChoose
{
    public ArenaTypeRunEffect TypeRunEffect;
    public List<GridArenaNode> ChoosedNodes;
}

public enum ArenaTypeRunEffect
{
    AutoChoose = 0,
    Choosed = 1,
    AutoAll = 2,
}


[System.Serializable]
public enum TypeWarMachine
{
    None = 0,
    AmmoCart = 1,
    Ballista = 2,
    FirstAidTent = 3,
    Catapult = 4,
}
