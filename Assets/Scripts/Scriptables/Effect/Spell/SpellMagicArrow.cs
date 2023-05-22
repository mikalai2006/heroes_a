using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpellMagicArrow", menuName = "Game/Attribute/Spell/7_MagicArrow")]
public class SpellMagicArrow : ScriptableAttributeSpell
{
    public async override UniTask AddEffect(ArenaEntity entity, EntityHero heroRunSpell, Player player = null)
    {
        await base.AddEffect(entity, heroRunSpell);

        ScriptableAttributeSecondarySkill baseSSkill = SchoolMagic.BaseSecondarySkill;
        int levelSSkill = heroRunSpell.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
            ? heroRunSpell.Data.SSkills[baseSSkill.TypeTwoSkill].level
            : 0;
        var dataCurrent = LevelData[levelSSkill];

        int totalDamage = dataCurrent.Effect + (heroRunSpell.Data.PSkills[TypePrimarySkill.Power] * 10);

        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
               AnimatePrefab,
               new Vector3(0, 0, 0),
               Quaternion.identity
           );
            var obj = await asset.Task;
            Vector3 startPosition = new Vector3(0, 0, 0);
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

        entity.SetDamage(totalDamage);
        await entity.ArenaMonoBehavior.RunGettingHitSpell();
    }

    //     public override UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
    //     {
    //         return base.RemoveEffect(entity, hero, player);
    //     }
}
