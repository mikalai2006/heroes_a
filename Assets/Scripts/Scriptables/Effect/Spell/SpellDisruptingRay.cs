using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellDisruptingRay", menuName = "Game/Attribute/Spell/19_DisruptingRay", order = 19)]
public class SpellDisruptingRay : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        await base.AddEffect(node, heroRunSpell, arenaManager);

        var creatureArena = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (creatureArena.Hero != null)
        {
            int levelSSkill = heroRunSpell.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Add modification data.
        var newModifDefense = -dataCurrent.Effect;
        var currentDefense = creatureArena.Data.defense + creatureArena.Data.DefenseModificators.Values.Sum();
        if (currentDefense > 0)
        {
            if (currentDefense - dataCurrent.Effect <= 0)
            {
                newModifDefense = -(dataCurrent.Effect + (currentDefense - dataCurrent.Effect));
            }

            if (!creatureArena.Data.DefenseModificators.ContainsKey(this))
            {
                creatureArena.Data.DefenseModificators.Add(this, newModifDefense);
            }
            else
            {
                creatureArena.Data.DefenseModificators[this] += newModifDefense;
            }

            // Add duration.
            if (creatureArena.Data.SpellsState.ContainsKey(this))
            {
                creatureArena.Data.SpellsState[this] = 1000;
            }
            else
            {
                creatureArena.Data.SpellsState.Add(this, 1000);
            }
        }

        // Run effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               creatureArena.ArenaMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(1000);
            Addressables.Release(asset);
        }
    }

    public override UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        var entity = node.OccupiedUnit;

        // Not remove effect.

        return base.RemoveEffect(node, hero, player);
    }
}
