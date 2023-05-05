using UnityEngine;

[CreateAssetMenu(fileName = "PathFindingEffect", menuName = "Game/EffectSkill/EffectPathFinding")]
public class EffectPathFinding : BaseEffectSkill
{
    public override void RunEffect(Player player, BaseEntity entity)
    {
        var hero = ((EntityHero)entity);

        var secondarySkill = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Find(t => t.TypeTwoSkill == TypeSecondarySkill.Pathfinding);

        hero.Data.Qualities.movepen = secondarySkill.Levels[hero.Data.SSkills[secondarySkill.TypeTwoSkill]].value;
    }
}
