using UnityEngine;

[CreateAssetMenu(fileName = "PrimarySkillEffect", menuName = "Game/Effect/EffectPrimarySkill")]
public class EffectPrimarySkill : BaseEffect, IEffected
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;

    public override void RunHero(ref Player player, BaseEntity entity)
    {
        base.RunHero(ref player, entity);

        var currentEffectData = entity.DataEffects.Effects.Find(t => t.ide == idEffect);

        player.ActiveHero.ChangePrimarySkill(PrimarySkill.TypeSkill, currentEffectData.value);
    }

    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        entity.DataEffects.Effects.Add(new DataEntityEffectsBase()
        {
            value = value,
            Effect = this,
            ide = idEffect
        });
    }

    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);

        dialogData.TypeEntity = TypeEntity.MapObject;
        var currentEffect = entity.DataEffects.Effects.Find(t => t.ide == idEffect);
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = PrimarySkill.MenuSprite,
            Value = currentEffect.value,
        });
    }
}
