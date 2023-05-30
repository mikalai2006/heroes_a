using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "Ballista", menuName = "Game/Attribute/WarMachine/Ballista")]
public class WarMachineBallista : ScriptableAttributeWarMachine
{
    public async override UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
            )
            .ToList();
        //arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack = TypeAttack.AttackWarMachine;

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
            ChoosedNodes = nodes,
            TypeRunEffect = typeRunEffect
        };
    }

    public async override UniTask RunEffect(ArenaManager arenaManager, GridArenaNode node, GridArenaNode nodeToAttack, Player player = null)
    {
        // Calculate damage.
        int baseDamage = Random.Range(CreatureParams.DamageMin, CreatureParams.DamageMin)
            * (arenaManager.ArenaQueue.ActiveHero.Data.PSkills[TypePrimarySkill.Attack] + 1);

        // Check Artillery and Archery.
        int levelSSkill = 0;
        int dopDamage = 0;
        int countAttack = 1;
        if (arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Artillery)
                ? arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Artillery].level + 1
                : 0;
            if (levelSSkill > 0)
            {
                var pSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Artillery];
                dopDamage = ((baseDamage * pSkill.value) / 100);
                // Calculate count attack.
                if (levelSSkill > 0)
                {
                    countAttack = 2;
                }
            }
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Archery)
                ? arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Archery].level + 1
                : 0;
            if (levelSSkill > 0)
            {
                var pSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Archery];
                dopDamage = dopDamage + ((baseDamage * pSkill.value) / 100);
            }
        }

        // run attack.
        for (int i = 0; i < countAttack; i++)
        {
            await ((ArenaWarMachine)node.OccupiedUnit).ArenaWarMachineMonoBehavior.RunAttackShoot(nodeToAttack);
            nodeToAttack.OccupiedUnit.SetDamage(baseDamage + dopDamage);
        }

        await UniTask.Delay(1);
    }

}

