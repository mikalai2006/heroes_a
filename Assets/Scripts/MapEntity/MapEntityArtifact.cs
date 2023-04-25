using Cysharp.Threading.Tasks;

using UnityEngine;

public class MapEntityArtifact : BaseMapEntity, IDialogMapObjectOperation
{
    public Transform _model;

    protected override void Awake()
    {
        base.Awake();
        _model = transform.Find("Model");
    }

    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);

        ScriptableEntityArtifact dataArtifact = (ScriptableEntityArtifact)MapObjectClass.ScriptableData;
        EntityArtifact entityArtifact = (EntityArtifact)MapObjectClass;

        if (entityArtifact.ConfigArtifact != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = entityArtifact.ConfigArtifact.spriteMap;
        }

    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // LocalizedString myLocalizedString = new LocalizedString("ADVENTUREVENT", "artifact_yes")
        // {
        //     { "name", new StringVariable { Value = this.ScriptableData.name } },
        // };
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var configData = (ScriptableEntityArtifact)MapObjectClass.ScriptableData;
        var dialogData = new DataDialogMapObject()
        {
            Header = configData.Artifact.Text.title.GetLocalizedString(),
            Description = configData.Artifact.textOk.GetLocalizedString(),
            Sprite = configData.Artifact.MenuSprite
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public override async UniTask OnGoHero(Player player)
    {
        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            DataResultDialog result = await OnTriggeredHero();

            if (result.isOk)
            {
                // Set artifact for hero.
                OnHeroGo(player);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
            OnHeroGo(player);
        }
    }

    private void OnHeroGo(Player player)
    {
        MapObjectClass.SetPlayer(player);
        // Destroy(gameObject);
    }
}
