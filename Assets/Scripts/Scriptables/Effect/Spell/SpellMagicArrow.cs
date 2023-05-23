using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellMagicArrow", menuName = "Game/Attribute/Spell/7_MagicArrow")]
public class SpellMagicArrow : ScriptableAttributeSpell
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

        var entity = node.OccupiedUnit;

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

        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 0, 0),
               Quaternion.identity
           );
            var obj = await asset.Task;
            var arenaHeroEntity = arenaManager.ArenaQueue.ActiveHero.ArenaHeroEntity;
            Vector3 startPosition = arenaHeroEntity.ArenaHeroMonoBehavior.transform.position;
            Vector3 endPosition = entity.OccupiedNode.center;
            float elapsedTime = 0;
            float time = .3f;
            while (elapsedTime < time)
            {
                obj.gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }
            obj.gameObject.transform.position = endPosition;

            Addressables.Release(asset);
        }

        await entity.ArenaMonoBehavior.RunGettingHitSpell();
        entity.SetDamage(totalDamage);
    }
}
