using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellCurse", menuName = "Game/Attribute/Spell/4_Curse")]
public class SpellCurse : ScriptableAttributeSpell
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
        int newDamageMax = creatureArena.Data.damageMin - dataCurrent.Effect;
        creatureArena.Data.damageMax = newDamageMax < 1 ? 1 : newDamageMax;
        if (creatureArena.Data.damageMin > creatureArena.Data.damageMax)
        {
            creatureArena.Data.damageMin = creatureArena.Data.damageMax;
        }

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
        entity.Data.damageMin = ((ScriptableAttributeCreature)entity.Entity.ScriptableDataAttribute).CreatureParams.DamageMin;
        entity.Data.damageMax = ((ScriptableAttributeCreature)entity.Entity.ScriptableDataAttribute).CreatureParams.DamageMax;
        // entity.Data.DamageModificators.Remove(this);

        return base.RemoveEffect(node, hero, player);
    }
}
