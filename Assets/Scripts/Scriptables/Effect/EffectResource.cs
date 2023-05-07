using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEffect", menuName = "Game/Effect/EffectResource")]
public class EffectResource : BaseEffect
{
    public ScriptableAttributeResource Resource;
    public int min;
    public int max;
    public int step;
    public List<TypeAttribute> Attrs;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();

        var _entity = (MapObject)entity.MapObject;
        Debug.Log($"entity.DataEffects.Effects={entity.Effects.Effects.Count}");
        var currentEffect = entity.Effects.Effects.Find(t => t.ide == idEffect);
        player.ChangeResource(Resource.TypeResource, currentEffect.value);
        Debug.Log($"EffectResource::: Run {entity.Effects.index}[{_entity.ConfigData.name}-player-{player.DataPlayer.id}]!");

        await UniTask.Delay(1);
        return result;
    }
    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        var _entity = (MapObject)entity.MapObject;

        var index = Helpers.GenerateValueByRangeAndStep(min, max, step);

        entity.Effects.Effects.Add(new DataEntityEffectsBase()
        {
            value = index,
            Effect = this,
            ide = idEffect,
            ido = Resource.idObject
        });
        // Debug.Log($"EffectResource::: Set data {entity.Effects.index}[{_entity.ConfigData.name}]!");
    }
    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);

        dialogData.TypeEntity = TypeEntity.MapObject;

        var currentEffect = entity.Effects.Effects.Find(t => t.ide == idEffect);

        if (currentEffect == null) return;

        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = Resource.MenuSprite,
            value = currentEffect.value,
        });
    }
}
