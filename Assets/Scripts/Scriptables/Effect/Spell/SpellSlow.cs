using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellSlow", menuName = "Game/Attribute/Spell/11_Slow", order = 11)]
public class SpellSlow : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                && t.OccupiedUnit.TypeArenaPlayer != arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }

    public async override UniTask AddEffect(GridArenaNode node, ArenaHeroEntity heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
        await base.AddEffect(node, heroRunSpell, arenaManager);

        var creatureArena = node.OccupiedUnit;

        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (creatureArena.Hero != null)
        {
            int levelSSkill = heroRunSpell.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Entity.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Add modification data.
        var newSpeed = -System.Math.Ceiling(creatureArena.Data.speed * (100 - dataCurrent.Effect) / 100.0);
        if (!creatureArena.Data.SpeedModificators.ContainsKey(this))
        {
            creatureArena.Data.SpeedModificators.Add(this, (int)newSpeed);
        }

        // Add duration.
        int countRound = heroRunSpell.Entity.Data.PSkills[TypePrimarySkill.Power];
        if (creatureArena.Data.SpellsState.ContainsKey(this))
        {
            creatureArena.Data.SpellsState[this] = countRound;
        }
        else
        {
            creatureArena.Data.SpellsState.Add(this, countRound);
        }

        // Run Effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               creatureArena is ArenaCreature ?
                ((ArenaCreature)creatureArena).ArenaMonoBehavior.transform
                : ((ArenaWarMachine)creatureArena).ArenaWarMachineMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(1000);
            Addressables.Release(asset);
        }

    }

    public async override UniTask RemoveEffect(GridArenaNode node, ArenaHeroEntity hero, Player player = null)
    {
        var entity = node.OccupiedUnit;
        entity.Data.SpeedModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
