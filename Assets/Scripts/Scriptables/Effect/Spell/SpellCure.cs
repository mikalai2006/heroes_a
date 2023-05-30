using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellCure", menuName = "Game/Attribute/Spell/3_Cure", order = 3)]
public class SpellCure : ScriptableAttributeSpell
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
        if (entity.Hero != null)
        {
            int levelSSkill = entity.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? entity.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Cure.
        entity.Data.totalHP += dataCurrent.Effect + (5 * heroRunSpell.Data.PSkills[TypePrimarySkill.Power]);
        if (entity.Data.totalHP > entity.Data.maxHP)
        {
            entity.Data.totalHP = entity.Data.maxHP;
        }

        // Remove  negative effects.
        var spells = entity.Data.SpellsState.Keys.ToList();
        foreach (var spell in spells)
        {
            if (spell.typeAchievement != TypeSpellAchievement.Friendly)
            {
                await spell.RemoveEffect(node, heroRunSpell);
                entity.Data.SpellsState.Remove(spell);
            }
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
    }
}
