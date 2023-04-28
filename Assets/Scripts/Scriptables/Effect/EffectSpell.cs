using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "SpellEffect", menuName = "Game/Effect/EffectSpell")]
public class EffectSpell : BaseEffect
{
    public int levelSpell;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();

        await UniTask.Delay(1);
        return result;
    }

    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        var randomSpell = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSpell>(TypeAttribute.Spell)
            .Find(t => t.level == levelSpell);
        entity.Effects.Effects.Add(new DataEntityEffectsBase()
        {
            // value = value,
            Effect = this,
            ide = idEffect,
            ido = randomSpell.idObject
        });
    }

    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);

        var currentEffect = (DataEntityEffectsBase)entity.Effects.Effects.Find(t => t.ide == idEffect);
        var randomSpell = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSpell>(TypeAttribute.Spell)
            .Find(t => t.idObject == currentEffect.ido);

        dialogData.TypeEntity = TypeEntity.MapObject;
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = randomSpell.MenuSprite,
            Value = currentEffect.value
        });
    }
}
