using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SpellCure", menuName = "Game/Attribute/Spell/3_Cure", order = 3)]
public class SpellCure : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
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
    public async override UniTask AddEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        var creature = node.OccupiedUnit;
        EntityCreature creatureEntity = (EntityCreature)creature.Entity;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (creature.Hero != null)
        {
            int levelSSkill = creature.Hero.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? creature.Hero.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Cure.
        var maxValueCure = (creature.Data.HP * creature.Data.quantity) - creature.Data.totalHP;
        var possibleValueCure = dataCurrent.Effect + (5 * heroRunSpell.Entity.Data.PSkills[TypePrimarySkill.Power]);
        var currentValueCure = possibleValueCure <= maxValueCure ? possibleValueCure : maxValueCure;
        creature.Data.totalHP += currentValueCure;
        if (creature.Data.totalHP > creature.Data.maxHP)
        {
            creature.Data.totalHP = creature.Data.maxHP;
        }

        // add info stat.
        var dataSmart = new Dictionary<string, object> {
            { "subject", Text.title.GetLocalizedString() },
            { "hp", currentValueCure },
            { "name", Helpers.GetNameByValue(creatureEntity.ConfigAttribute.Text.title, 5) }
            };
        var arguments = new[] { dataSmart };
        var textSmart = Helpers.GetLocalizedPluralString(
            new LocalizedString(Constants.LanguageTable.LANG_STAT, "cure_stat"),
            arguments,
            dataSmart
            );
        arenaManager.ArenaStat.AddItem(textSmart);

        // Remove  negative effects.
        var spells = creature.Data.SpellsState.Keys.ToList();
        foreach (var spell in spells)
        {
            if (spell.typeAchievement != TypeSpellAchievement.Friendly)
            {
                await spell.RemoveEffect(node, heroRunSpell);
                creature.Data.SpellsState.Remove(spell);
            }
        }

        // Run effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               creature is ArenaCreature ?
                ((ArenaCreature)creature).ArenaMonoBehavior.transform
                : ((ArenaWarMachine)creature).ArenaWarMachineMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(500);
            Addressables.Release(asset);
        }
    }
}
