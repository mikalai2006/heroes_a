using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "AmmoCart", menuName = "Game/Attribute/WarMachine/AmmoCart")]
public class WarMachineAmmoCart : ScriptableAttributeWarMachine
{
    public async override UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer == arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
                && ((EntityCreature)t.OccupiedUnit.Entity).ConfigAttribute.CreatureParams.Shoots != 0
            )
            .ToList();
        arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack = TypeAttack.AttackShoot;

        await UniTask.Delay(1);
        return new ArenaResultChoose()
        {
            ChoosedNodes = nodes,
            TypeRunEffect = ArenaTypeRunEffect.AutoAll
        };
    }

    public async override UniTask RunEffect(ArenaManager arenaManager, GridArenaNode node, GridArenaNode nodeToAction, Player player = null)
    {
        // TODO Effect recovery shoots for shootes creatures.
        nodeToAction.OccupiedUnit.Data.shoots
            = ((EntityCreature)nodeToAction.OccupiedUnit.Entity).ConfigAttribute.CreatureParams.Shoots;

        await UniTask.Delay(1);
    }

}

