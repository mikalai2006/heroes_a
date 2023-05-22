using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellBless", menuName = "Game/Attribute/Spell/1_Bless")]
public class SpellBless : ScriptableAttributeSpell
{
    public async override UniTask AddEffect(ArenaEntity entity, EntityHero heroRunSpell, Player player = null)
    {
        await base.AddEffect(entity, heroRunSpell);

        if (entity.Hero != null)
        {
            ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
            int levelSSkill = entity.Hero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                ? entity.Hero.Data.SSkills[baseSSkill.TypeTwoSkill].level
                : 0;
            var dataCurrent = LevelData[levelSSkill];
            entity.Data.damageMin = entity.Data.damageMax;

            if (!entity.Data.DamageModificators.ContainsKey(this))
            {
                entity.Data.DamageModificators.Add(this, dataCurrent.Effect);
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
            await UniTask.Delay(1000);
            Addressables.Release(asset);
        }
    }

    public async override UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
    {
        entity.Data.damageMin = ((ScriptableAttributeCreature)entity.Entity.ScriptableDataAttribute).CreatureParams.DamageMin;
        entity.Data.DamageModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
