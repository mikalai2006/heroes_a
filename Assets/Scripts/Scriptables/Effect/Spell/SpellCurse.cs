using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellCurse", menuName = "Game/Attribute/Spell/4_Curse")]
public class SpellCurse : ScriptableAttributeSpell
{
    public async override UniTask AddEffect(ArenaEntity entity, EntityHero heroRunSpell, Player player = null)
    {
        await base.AddEffect(entity, heroRunSpell);

        if (heroRunSpell != null)
        {
            ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
            int levelSSkill = heroRunSpell.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? heroRunSpell.Data.SSkills[baseSSkill.TypeTwoSkill].level
                : 0;
            var dataCurrent = LevelData[levelSSkill];

            int newDamageMax = entity.Data.damageMin - dataCurrent.Effect;
            entity.Data.damageMax = newDamageMax < 1 ? 1 : newDamageMax;
            if (entity.Data.damageMin > entity.Data.damageMax)
            {
                entity.Data.damageMin = entity.Data.damageMax;
            }

            // if (!entity.Data.DamageModificators.ContainsKey(this))
            // {
            //     entity.Data.DamageModificators.Add(this, dataCurrent.Effect);
            // }
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
            await UniTask.Delay(1000);
            Addressables.Release(asset);
        }
    }

    public override UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
    {
        entity.Data.damageMin = ((ScriptableAttributeCreature)entity.Entity.ScriptableDataAttribute).CreatureParams.DamageMin;
        entity.Data.damageMax = ((ScriptableAttributeCreature)entity.Entity.ScriptableDataAttribute).CreatureParams.DamageMax;
        // entity.Data.DamageModificators.Remove(this);

        return base.RemoveEffect(entity, hero, player);
    }
}
