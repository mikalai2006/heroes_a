using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellDispel", menuName = "Game/Attribute/Spell/5_Dispel", order = 5)]
public class SpellDispel : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t => t.OccupiedUnit != null && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded))
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        var entity = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        int levelSSkill = 0;
        if (entity.Hero != null)
        {
            levelSSkill = entity.Hero.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? entity.Hero.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Remove all effects.
        var spells = entity.Data.SpellsState.Keys.ToList();
        foreach (var spell in spells)
        {
            await spell.RemoveEffect(node, heroRunSpell);
            entity.Data.SpellsState.Remove(spell);
        }

        // Run effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               entity is ArenaCreature ?
                ((ArenaCreature)entity).ArenaMonoBehavior.transform
                : ((ArenaWarMachine)entity).ArenaWarMachineMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(500);
            Addressables.Release(asset);
        }

        // Remove spells of node
        List<GridArenaNode> spellsnodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t => t.SpellUnit != null)
            .ToList();
        if (levelSSkill >= 2 && spellsnodes.Count > 0)
        {
            foreach (var spellNode in spellsnodes)
            {
                if (spellNode.SpellUnit != null)
                {
                    await spellNode.SpellUnit.ConfigDataSpell.RemoveEffect(spellNode, heroRunSpell);
                }
            }
        }
    }
}
