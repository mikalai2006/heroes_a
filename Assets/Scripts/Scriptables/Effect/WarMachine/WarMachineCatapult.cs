using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "Catapult", menuName = "Game/Attribute/WarMachine/Catapult")]
public class WarMachineCatapult : ScriptableAttributeWarMachine
{
    public async override UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        // List<GridArenaNode> nodes = arenaManager
        //     .GridArenaHelper
        //     .GetAllGridNodes()
        //     .Where(t =>
        //         t.OccupiedUnit != null
        //         && t.OccupiedUnit.TypeArenaPlayer != arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
        //     )
        //     .ToList();
        // arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack = TypeAttack.AttackWarMachine;
        Debug.Log($"arenaManager.town.ArenaEntityTownMonobehavior._fortifications={arenaManager.town.ArenaEntityTownMonobehavior._fortifications.Count}");
        arenaManager.clickedFortification = arenaManager.town.ArenaEntityTownMonobehavior._fortifications[3].gameObject;
        // Checktype run.
        int levelSSkill = 0;
        if (arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Ballistics)
                ? arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Ballistics].level + 1
                : 0;
        }
        ArenaTypeRunEffect typeRunEffect = ArenaTypeRunEffect.AutoChoose;
        switch (levelSSkill)
        {
            case 0:
                typeRunEffect = ArenaTypeRunEffect.AutoChoose;
                break;
            case 1:
            case 2:
            case 3:
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
        }

        await UniTask.Delay(1);
        return new ArenaResultChoose()
        {
            ChoosedNodes = null,
            TypeRunEffect = typeRunEffect
        };
    }

    // public async override UniTask RunEffect(ArenaManager arenaManager, GridArenaNode node, GridArenaNode nodeToAttack, Player player = null)
    // {
    //     // arenaManager.clickedFortification.
    //     await node.OccupiedUnit.ArenaMonoBehavior.RunAttackShoot(nodeToAttack);

    //     // Calculate damage.


    //     await UniTask.Delay(1);
    // }
    public async override UniTask RunEffectByGameObject(ArenaManager arenaManager, GridArenaNode node, GameObject gameObject)
    {
        await node.OccupiedUnit.ArenaMonoBehavior.RunAttackShoot(null, gameObject.transform);
        Debug.Log($"Run effect by gameObject {gameObject.name}");
        await UniTask.Delay(1);
        // return base.RunEffectByGameObject(arenaManager, node, gameObject);
    }
}
