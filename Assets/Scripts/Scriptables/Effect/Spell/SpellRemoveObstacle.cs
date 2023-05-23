using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "RemoveObstacle", menuName = "Game/Attribute/Spell/27_RemoveObstacle", order = 27)]
public class SpellRemoveObstacle : ScriptableAttributeSpell
{
    public async override UniTask<List<GridArenaNode>> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit == null
                && t.SpellsUnit != null
            )
            .ToList();

        await UniTask.Delay(1);
        return nodes;
    }
    public async override UniTask AddEffect(GridArenaNode node, EntityHero heroRunSpell, ArenaManager arenaManager, Player player = null)
    {
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

        // Run effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               node.SpellsUnit.ArenaSpellMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(500);
            Addressables.Release(asset);
        }

        // Remove spell of node
        if (levelSSkill >= 2 && node.SpellsUnit != null)
        {
            await node.SpellsUnit.ConfigDataSpell.RemoveEffect(node, heroRunSpell);
        }
    }
}
