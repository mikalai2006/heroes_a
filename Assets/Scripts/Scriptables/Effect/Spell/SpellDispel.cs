using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellDispel", menuName = "Game/Attribute/Spell/5_Dispel")]
public class SpellDispel : ScriptableAttributeSpell
{
    public async override UniTask AddEffect(ArenaEntity entity, EntityHero heroRunSpell, Player player = null)
    {
        if (entity.Hero != null)
        {
            ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
            int levelSSkill = entity.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? entity.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level
                : 0;
            var dataCurrent = LevelData[levelSSkill];

            // Cure.
            entity.Data.totalHP += dataCurrent.Effect + (5 * heroRunSpell.Data.PSkills[TypePrimarySkill.Power]);
            if (entity.Data.totalHP > entity.Data.maxHP)
            {
                entity.Data.totalHP = entity.Data.maxHP;
            }

            // Remove all effects.
            var spells = entity.Data.SpellsState.Keys.ToList();
            foreach (var spell in spells)
            {
                await spell.RemoveEffect(entity, heroRunSpell);
                entity.Data.SpellsState.Remove(spell);
            }
        }

        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 1, 0),
               Quaternion.identity,
               entity.ArenaMonoBehavior.transform
           );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(500);
            Addressables.Release(asset);
        }
    }
}
