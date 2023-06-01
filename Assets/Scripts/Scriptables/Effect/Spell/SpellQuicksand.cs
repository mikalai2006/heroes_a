using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "Quicksand", menuName = "Game/Attribute/Spell/26_Quicksand", order = 26)]
public class SpellQuicksand : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Disable)
                && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                && !t.StateArenaNode.HasFlag(StateArenaNode.Spellsed)
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = LevelData[0];
        int levelSSkill = 0;
        if (heroRunSpell != null)
        {
            levelSSkill = heroRunSpell.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        List<GridArenaNode> allowNodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Spellsed)
            )
            .OrderBy(t => Random.value)
            .ToList();
        int countCreated = 0;
        while (countCreated < dataCurrent.Effect)
        {
            var nodeForEffect = allowNodes[0];
            if (!nodeForEffect.StateArenaNode.HasFlag(StateArenaNode.Spellsed))
            {
                ArenaObstacle newEntity = new ArenaObstacle(nodeForEffect, this, heroRunSpell, arenaManager);
                newEntity.CreateMapGameObject();
                nodeForEffect.SpellsState.Add(this, 1000);
                nodeForEffect.SetSpellsStatus(true);
                countCreated++;
            }
            else
            {
                allowNodes.Remove(nodeForEffect);
            }
        }

        await UniTask.Delay(1);
    }

    public async override UniTask RunEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, GridArenaNode nodeWithSpell, Player player = null)
    {
        var entity = (ArenaCreature)node.OccupiedUnit;

        if (entity.Hero != nodeWithSpell.SpellUnit.Hero)
        {
            entity.SetPath(null);
        }

        await UniTask.Delay(1);
    }

    public async override UniTask RemoveEffect(GridArenaNode node, ArenaHeroEntity hero, Player player = null)
    {
        node.SpellsState.Remove(this);
        node.SpellUnit.DestroyMapObject();
        await UniTask.Delay(1);
    }
}
