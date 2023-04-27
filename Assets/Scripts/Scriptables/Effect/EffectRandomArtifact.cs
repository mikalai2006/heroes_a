using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "RandomArtifactEffect", menuName = "Game/Effect/EffectRandomArtifact")]
public class EffectRandomArtifact : BaseEffect, IEffected
{
    public List<ScriptableAttributeArtifact> Artifacts;

    public override void RunHero(Player player, BaseEntity entity)
    {
        var RandomArtifact = Artifacts[Random.Range(0, Artifacts.Count)];

        Debug.Log($"EffectRandomArtifact::: Run {RandomArtifact.name} run!");
    }
    public override void SetData(BaseEntity entity)
    {
        base.SetData(entity);
        var RandomArtifact = Artifacts[Random.Range(0, Artifacts.Count)];
        entity.Effects.Effects.Add(new DataEntityEffectsBase()
        {
            Effect = this,
            ide = idEffect,
            ido = RandomArtifact.idObject
        });
        Debug.Log($"EffectRandomArtifact::: Set Data {RandomArtifact.idObject}");
    }

    public override void CreateDialogData(ref DataDialogMapObjectGroup dialogData, BaseEntity entity)
    {
        base.CreateDialogData(ref dialogData, entity);
        var currentEffect = entity.Effects.Effects.Find(t => t.ide == idEffect);
        var configArtifact = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
            .Find(t => t.idObject == currentEffect.ido);

        dialogData.TypeEntity = TypeEntity.MapObject;
        dialogData.Values.Add(new DataDialogMapObjectGroupItem()
        {
            Sprite = configArtifact.MenuSprite,
        });
    }
}
