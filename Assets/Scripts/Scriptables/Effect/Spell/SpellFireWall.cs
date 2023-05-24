using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "SpellFireWall", menuName = "Game/Attribute/Spell/20_FireWall", order = 20)]
public class SpellFireWall : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && t.SpellUnit == null
                && t.Neighbours().Count == 6
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        var entity = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        int levelSSkill = 0;
        if (heroRunSpell != null)
        {
            levelSSkill = heroRunSpell.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        ArenaEntitySpell newEntity = new ArenaEntitySpell(node, this, heroRunSpell, arenaManager);
        newEntity.CreateMapGameObject();
        node.SpellsState.Add(this, 2);
        node.SetSpellsStatus(true);

        var relatedEntity = new ArenaEntitySpell(node.LeftTopNode, this, heroRunSpell, arenaManager);
        relatedEntity.CreateMapGameObject();
        newEntity.AddRelatedNode(node.LeftTopNode);
        node.LeftTopNode.SpellsState.Add(this, 2);

        if (levelSSkill >= 2)
        {
            relatedEntity = new ArenaEntitySpell(node.LeftBottomNode, this, heroRunSpell, arenaManager);
            relatedEntity.CreateMapGameObject();
            newEntity.AddRelatedNode(node.LeftBottomNode);
            node.LeftBottomNode.SpellsState.Add(this, 2);
        }

        await UniTask.Delay(1);
    }

    public async override UniTask RunEffect(GridArenaNode node, EntityHero heroForSpell, GridArenaNode nodeWithSpell, Player player = null)
    {
        var entity = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (nodeWithSpell.SpellUnit.Hero != null)
        {
            int levelSSkill = nodeWithSpell.SpellUnit.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
            ? nodeWithSpell.SpellUnit.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
            : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        int totalDamage = dataCurrent.Effect + (nodeWithSpell.SpellUnit.Hero.Data.PSkills[TypePrimarySkill.Power] * 10);

        await entity.ArenaMonoBehavior.RunGettingHitSpell();
        entity.SetDamage(totalDamage);
    }

    public async override UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        // Remove related units from related nodes.
        foreach (var relatedNode in node.SpellUnit.RelatedNodes)
        {
            relatedNode.SpellsState.Remove(this);
            relatedNode.SpellUnit.DestroyMapObject();
        }
        // Remove unit.
        node.SpellsState.Remove(this);
        node.SpellUnit.DestroyMapObject();
        await UniTask.Delay(1);
    }
}
