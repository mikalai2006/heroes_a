using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "SpellFireWall", menuName = "Game/Attribute/Spell/20_FireWall", order = 20)]
public class SpellFireWall : ScriptableAttributeSpell
{
    public int countRound;
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        var activeTypePlayer = arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer;
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && t.SpellUnit == null
                // && t.Neighbours().Count == 6
                && (
                    (
                        activeTypePlayer == TypeArenaPlayer.Left
                        && t.LeftTopNode != null
                        && t.LeftTopNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                        && !t.LeftTopNode.StateArenaNode.HasFlag(StateArenaNode.Disable)
                        && t.LeftBottomNode != null
                        && t.LeftBottomNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                        && !t.LeftBottomNode.StateArenaNode.HasFlag(StateArenaNode.Disable)
                    )
                    ||
                    (
                        activeTypePlayer == TypeArenaPlayer.Right
                        && t.RightTopNode != null
                        && t.RightTopNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                        && !t.RightTopNode.StateArenaNode.HasFlag(StateArenaNode.Disable)
                        && t.RightBottomNode != null
                        && t.RightBottomNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                        && !t.RightBottomNode.StateArenaNode.HasFlag(StateArenaNode.Disable)
                    )
                )
            // && t.Neighbours().Where(t =>
            //     t.StateArenaNode.HasFlag(StateArenaNode.Disable)
            //     || t.StateArenaNode.HasFlag(StateArenaNode.Occupied)
            // ).Count() == 0
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        var activeTypePlayer = arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer;
        var entity = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        int levelSSkill = 0;
        if (heroRunSpell != null)
        {
            levelSSkill = heroRunSpell.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        ArenaObstacle newEntity = new ArenaObstacle(node, this, heroRunSpell, arenaManager);
        newEntity.CreateMapGameObject();
        node.SpellsState.Add(this, countRound);
        node.SetSpellsStatus(true);

        var nodeForRelated = activeTypePlayer == TypeArenaPlayer.Left
            ? node.LeftTopNode
            : node.RightTopNode;
        var relatedEntity = new ArenaObstacle(nodeForRelated, this, heroRunSpell, arenaManager);
        relatedEntity.CreateMapGameObject();
        newEntity.AddRelatedNode(nodeForRelated);
        nodeForRelated.SpellsState.Add(this, countRound);

        if (levelSSkill >= 2)
        {
            var secondNodeForRelated = activeTypePlayer == TypeArenaPlayer.Left
                ? node.LeftBottomNode
                : node.RightBottomNode;
            relatedEntity = new ArenaObstacle(secondNodeForRelated, this, heroRunSpell, arenaManager);
            relatedEntity.CreateMapGameObject();
            newEntity.AddRelatedNode(secondNodeForRelated);
            secondNodeForRelated.SpellsState.Add(this, countRound);
        }

        await UniTask.Delay(1);
    }

    public async override UniTask RunEffect(GridArenaNode node, ArenaHeroEntity heroForSpell, GridArenaNode nodeWithSpell, Player player = null)
    {
        var entity = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (nodeWithSpell.SpellUnit.Hero != null)
        {
            int levelSSkill = nodeWithSpell.SpellUnit.Hero.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
            ? nodeWithSpell.SpellUnit.Hero.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
            : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        int totalDamage = dataCurrent.Effect + (nodeWithSpell.SpellUnit.Hero.Entity.Data.PSkills[TypePrimarySkill.Power] * 10);

        await entity.RunGettingHitSpell();
        entity.SetDamage(totalDamage);
    }

    public async override UniTask RemoveEffect(GridArenaNode node, ArenaHeroEntity hero, Player player = null)
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
