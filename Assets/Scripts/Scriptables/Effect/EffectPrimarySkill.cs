using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "PrimarySkillEffect", menuName = "Game/Effect/EffectPrimarySkill")]
public class EffectPrimarySkill : BaseEffect
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();
        var activeHero = player.ActiveHero;

        Debug.Log($"Start effect skill - {activeHero.Data.name}");

        var currentEffectData = entity.Effects.Effects.Find(t => t.ide == idEffect);

        if (PrimarySkill.TypeSkill == TypePrimarySkill.Experience)
        {
            await player.ActiveHero.ChangeExperience(currentEffectData.value);
        }
        else
        {
            await player.ActiveHero.ChangePrimarySkill(PrimarySkill.TypeSkill, currentEffectData.value);
        }

        await UniTask.Delay(1);
        return result;
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

        int value = currentEffect.value;

        // Effect Learning.
        if (PrimarySkill.TypeSkill == TypePrimarySkill.Experience)
        {
            EntityHero hero = LevelManager.Instance.ActivePlayer.ActiveHero;
            if (hero != null)
            {
                value = hero.GetExperience(currentEffect.value);
            }
        }
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = PrimarySkill.MenuSprite,
            value = value,
        });
    }
}
