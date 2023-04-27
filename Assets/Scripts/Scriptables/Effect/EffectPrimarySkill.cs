using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "PrimarySkillEffect", menuName = "Game/Effect/EffectPrimarySkill")]
public class EffectPrimarySkill : BaseEffect, IEffected
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;

    public override void RunHero(Player player, BaseEntity entity)
    {
        // base.RunHero(player, entity);
        Debug.Log($"Start effect skill - {LevelManager.Instance.ActivePlayer.ActiveHero}");

        var currentEffectData = entity.Effects.Effects.Find(t => t.ide == idEffect);

        player.ActiveHero.ChangePrimarySkill(PrimarySkill.TypeSkill, currentEffectData.value);
    }

    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        entity.Effects.Effects.Add(new DataEntityEffectsBase()
        {
            value = value,
            Effect = this,
            ide = idEffect,
            // ido = PrimarySkill.idObject
        });
    }

    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);

        dialogData.TypeEntity = TypeEntity.MapObject;
        var currentEffect = entity.Effects.Effects.Find(t => t.ide == idEffect);
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = PrimarySkill.MenuSprite,
            Value = currentEffect.value,
        });
    }
}
