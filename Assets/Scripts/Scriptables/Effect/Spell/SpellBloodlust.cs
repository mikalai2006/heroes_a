using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "SpellBloodlust", menuName = "Game/Attribute/Spell/2_Bloodlust", order = 2)]
public class SpellBloodlust : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                && t.OccupiedUnit.TypeArenaPlayer == arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
                && ((EntityCreature)t.OccupiedUnit.Entity).ConfigAttribute.CreatureParams.Shoots == 0
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        var creatureArena = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (creatureArena.Hero != null)
        {
            int levelSSkill = creatureArena.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
            ? creatureArena.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
            : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Add mofification data.
        if (!creatureArena.Data.AttackModificators.ContainsKey(this))
        {
            creatureArena.Data.AttackModificators.Add(this, dataCurrent.Effect);
        }

        // Run effect.
        await creatureArena.ColorPulse(Color.red, 3);

        // Add duration.
        int countRound = heroRunSpell.Data.PSkills[TypePrimarySkill.Power];
        if (creatureArena.Data.SpellsState.ContainsKey(this))
        {
            creatureArena.Data.SpellsState[this] = countRound;
        }
        else
        {
            creatureArena.Data.SpellsState.Add(this, countRound);
        }
    }

    public async override UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        var entity = node.OccupiedUnit;
        entity.Data.AttackModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
