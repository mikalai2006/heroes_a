using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "RandomResourceEffect", menuName = "Game/Effect/EffectRandomResource")]
public class EffectRandomResource : BaseEffect
{
    public List<ScriptableAttributeResource> Resources;
    public int min;
    public int max;
    public int step;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();

        var currentEffectData = entity.Effects.Effects.Find(t => t.ide == idEffect);
        var currentResource = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeResource>(TypeAttribute.Resource)
            .Find(t => t.idObject == currentEffectData.ido);
        player.ChangeResource(currentResource.TypeResource, currentEffectData.value);
        Debug.Log($"EffectRandomResource::: Run {currentResource.name} run!");

        await UniTask.Delay(1);
        return result;
    }
    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        var randomResource = Resources[Random.Range(0, Resources.Count)];
        var index = Helpers.GenerateValueByRangeAndStep(min, max, step);
        entity.Effects.Effects.Add(new DataEntityEffectsBase()
        {
            value = index,
            Effect = this,
            ido = randomResource.idObject,
            ide = idEffect
        });
        // Debug.Log($"EffectRandomResource::: Set data {randomResource.idObject}");
    }

    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);
        var currentEffect = (DataEntityEffectsBase)entity.Effects.Effects.Find(t => t.ide == idEffect);
        var randomResource = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeResource>(TypeAttribute.Resource)
            .Find(t => t.idObject == currentEffect.ido);

        dialogData.TypeEntity = TypeEntity.MapObject;
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = randomResource.MenuSprite,
            value = currentEffect.value
        });
    }
}
