using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEffect", menuName = "Game/Effect/EffectResource")]
public class EffectResource : BaseEffect, IEffected
{
    public ScriptableAttributeResource Resource;
    public int min;
    public int max;
    public int step;
    public List<TypeAttribute> Attrs;

    public override void RunHero(ref Player player, BaseEntity entity)
    {
        base.RunHero(ref player, entity);

        Debug.Log($"entity.DataEffects.Effects={entity.DataEffects.Effects.Count}");
        var currentEffect = entity.DataEffects.Effects.Find(t => t.ide == idEffect);
        player.ChangeResource(Resource.TypeResource, currentEffect.value);
        Debug.Log($"EffectResource::: Run {entity.DataEffects.index}[{entity.ScriptableData.name}-player-{player.DataPlayer.id}]!");
    }
    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);

        var index = Helpers.GenerateValueByRangeAndStep(min, max, step);

        entity.DataEffects.Effects.Add(new DataEntityEffectsBase()
        {
            value = index,
            Effect = this,
            ide = idEffect
        });
        Debug.Log($"EffectResource::: Set data {entity.DataEffects.index}[{entity.ScriptableData.name}]!");
    }
    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);

        dialogData.TypeEntity = TypeEntity.MapObject;

        var currentEffect = entity.DataEffects.Effects.Find(t => t.ide == idEffect);

        if (currentEffect == null) return;

        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = Resource.MenuSprite,
            Value = currentEffect.value,
        });
    }
}
