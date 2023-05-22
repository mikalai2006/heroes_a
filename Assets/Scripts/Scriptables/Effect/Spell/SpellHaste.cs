using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellHaste", menuName = "Game/Attribute/Spell/6_Haste")]
public class SpellHaste : ScriptableAttributeSpell
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

            if (!entity.Data.SpeedModificators.ContainsKey(this))
            {
                entity.Data.SpeedModificators.Add(this, dataCurrent.Effect);
            }
            else
            {
                entity.Data.SpeedModificators[this] = dataCurrent.Effect;
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

    public async override UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
    {
        entity.Data.SpeedModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
