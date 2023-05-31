using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellShield", menuName = "Game/Attribute/Spell/10_Shield", order = 10)]
public class SpellShield : ScriptableAttributeSpell
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
        var creatureArena = node.OccupiedUnit;
        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        SpellItem dataCurrent = new();
        if (creatureArena.Hero != null)
        {
            int levelSSkill = creatureArena.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? creatureArena.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level + 1
                : 0;
            dataCurrent = LevelData[levelSSkill];
        }

        // Add modification data.
        if (!creatureArena.Data.DefenseModificators.ContainsKey(this))
        {
            creatureArena.Data.DefenseModificators.Add(this, creatureArena.Data.defense - (creatureArena.Data.defense * dataCurrent.Effect / 100));
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

    public async override UniTask RemoveEffect(GridArenaNode node, EntityHero hero, Player player = null)
    {
        var entity = node.OccupiedUnit;
        entity.Data.DefenseModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
