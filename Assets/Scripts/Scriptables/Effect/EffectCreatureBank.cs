using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "CreatureBankEffect", menuName = "Game/Effect/EffectCreatureBank")]
public class EffectCreatureBank : BaseEffect
{
    public List<BankCreatureProtected> creatures;
    public List<BankResource> resources;
    public List<BankArtifact> artifacts;
    public List<BankPrimarySkill> primarySkills;
    public LocalizedString textWin;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var _processCompletionSource = new TaskCompletionSource<EffectResult>();
        var resultEffect = new EffectResult();

        var _activePlayer = LevelManager.Instance.ActivePlayer;
        // var dialogData = new DataDialogHelp()
        // {
        //     Header = "Arena",
        //     Description = "Utopia",
        // };
        // var dialogWindow = new DialogHelpProvider(dialogData);
        // var result = await dialogWindow.ShowAndHide();

        // Get setting for arena.
        var arenaSetting = LevelManager.Instance.ConfigGameSettings.ArenaSettings
            .Where(t => t.NativeGround.typeGround == entity.MapObject.OccupiedNode.TypeGround)
            .ToList();
        // TODO ARENA
        var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
        {
            heroAttacking = player.ActiveHero,
            creature = null,
            creaturesBank = (EntityMapObject)entity,
            town = null,
            ArenaSetting = arenaSetting[Random.Range(0, arenaSetting.Count())]
        });
        var result = await loadingOperations.ShowHide();

        if (result.isWinLeftHero)
        {
            resultEffect.ok = result.isWinLeftHero;
            // Show Dialog.
            if (_activePlayer.DataPlayer.playerType != PlayerType.Bot)
            {
                var _dialogData = new DataDialogMapObjectGroup()
                {
                    Values = new List<DataDialogMapObjectGroupItem>()
                };

                var groups = new List<DataDialogMapObjectGroup>();
                groups.Add(_dialogData);

                // var description = configData.Effects[_mapObject.Entity.Effects.index].Item.description.IsEmpty
                //     ? ""
                //     : configData.Effects[_mapObject.Entity.Effects.index].Item.description.GetLocalizedString();

                var dialogResultData = new DataDialogMapObject()
                {
                    Header = entity.ScriptableData.Text.title.GetLocalizedString(),
                    // Description = description,
                    TypeCheck = TypeCheck.OnlyOk,
                    TypeWorkEffect = TypeWorkAttribute.All,
                    Groups = groups
                };

                var dialogResultWindow = new DialogMapObjectProvider(dialogResultData);
                await dialogResultWindow.ShowAndHide();
            }

            // Add Resources.
            foreach (var res in resources)
            {
                _activePlayer.ChangeResource(res.Resource.TypeResource, res.value);
            }

            // Add Artifacts.
            if (artifacts.Count > 0)
            {
                foreach (var art in artifacts)
                {
                    var artifact = ResourceSystem.Instance
                        .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
                        .Where(t => t.ClassArtifact == art.classArtifact)
                        .OrderBy(t => Random.value)
                        .ToList();
                    for (int i = 0; i < art.value; i++)
                    {
                        if (artifact.Count() < i)
                        {
                            EntityArtifact newArt = new EntityArtifact(artifact[i]);
                            _activePlayer.ActiveHero.AddArtifact(newArt);
                        }
                    }
                }
            }
            _processCompletionSource.SetResult(resultEffect);
        }
        else
        {
            resultEffect.ok = result.isWinRightHero;
            _processCompletionSource.SetResult(resultEffect);
        }

        Debug.Log("EffectCreatureBank::: Run!");

        return await _processCompletionSource.Task;
    }

    public override void SetData(BaseEntity entity)
    {
        // Debug.Log($"EffectRandomArtifact::: Set Data {RandomArtifact.idObject}");
        entity.SetDefenders(creatures);
    }
}


[System.Serializable]
public struct BankCreatureProtected
{
    public ScriptableAttributeCreature creature;
    public int value;
}

[System.Serializable]
public struct BankResource
{
    public ScriptableAttributeResource Resource;
    public int value;
}

[System.Serializable]
public struct BankPrimarySkill
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}

[System.Serializable]
public struct BankArtifact
{
    public ClassArtifact classArtifact;
    public int value;
}
