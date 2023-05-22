using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "SpellBloodlust", menuName = "Game/Attribute/Spell/2_Bloodlust")]
public class SpellBloodlust : ScriptableAttributeSpell
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

            if (!entity.Data.AttackModificators.ContainsKey(this))
            {
                entity.Data.AttackModificators.Add(this, dataCurrent.Effect);
            }
        }

        await entity.ArenaMonoBehavior.ColorPulse(Color.red, 3);

        Debug.Log($"Bloodlust::: for {entity.Entity.ScriptableDataAttribute.name}");

    }

    public async override UniTask RemoveEffect(ArenaEntity entity, EntityHero hero, Player player = null)
    {
        entity.Data.AttackModificators.Remove(this);

        await UniTask.Delay(1);
    }
}
