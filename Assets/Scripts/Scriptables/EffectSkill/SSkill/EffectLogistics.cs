using UnityEngine;

[CreateAssetMenu(fileName = "LogisticsEffect", menuName = "Game/EffectSkill/EffectLogistics")]
public class EffectLogistics : BaseEffectSkill
{
    public override void RunEffect(Player player, BaseEntity entity)
    {
        var hero = ((EntityHero)entity);

        var secondarySkill = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Find(t => t.TypeTwoSkill == TypeSecondarySkill.Logistics);

        hero.Data.Qualities.mp = secondarySkill.Levels[hero.Data.SSkills[secondarySkill.TypeTwoSkill]].value;
    }
}
