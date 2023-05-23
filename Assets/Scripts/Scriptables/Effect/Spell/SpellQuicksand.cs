using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "Quicksand", menuName = "Game/Attribute/Spell/26_Quicksand", order = 26)]
public class SpellQuicksand : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && t.SpellsUnit == null
                && t.Neighbours().Count == 6
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        ArenaEntitySpell newEntity = new ArenaEntitySpell(node, this, heroRunSpell, arenaManager);
        newEntity.CreateMapGameObject();
        node.SpellsState.Add(this, 2);

        await UniTask.Delay(1);
    }

    public async override UniTask RunEffect(GridArenaNode node, EntityHero heroRunSpell, Player player = null)
    {
        var entity = node.OccupiedUnit;
        Debug.Log($"RunEffect::: {name}");
        // var sp = node.SpellsState.Get[this];

        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (entity.Hero != null)
        {
            int levelSSkill = heroRunSpell.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
            ? heroRunSpell.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
            : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        int totalDamage = dataCurrent.Effect + (heroRunSpell.Data.PSkills[TypePrimarySkill.Power] * 10);

        await UniTask.Delay(1);
    }

    public async override UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        node.SpellsState.Remove(this);
        node.SpellsUnit.DestroyMapObject();
        await UniTask.Delay(1);
    }
}
