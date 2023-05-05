using UnityEngine;

[CreateAssetMenu(fileName = "ScoutingEffect", menuName = "Game/EffectSkill/EffectScouting")]
public class EffectScouting : BaseEffectSkill
{
    public override void RunEffect(Player player, BaseEntity entity)
    {
        var hero = ((EntityHero)entity);

        var secondarySkill = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Find(t => t.TypeTwoSkill == TypeSecondarySkill.Scouting);

        hero.Data.Qualities.scout = secondarySkill.Levels[hero.Data.SSkills[secondarySkill.TypeTwoSkill]].value;
    }
}
