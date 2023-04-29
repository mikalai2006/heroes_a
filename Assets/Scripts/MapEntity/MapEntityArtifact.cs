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

    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);

        ScriptableEntityMapObject dataArtifact = (ScriptableEntityMapObject)_mapObject.ConfigData;
        EntityArtifact entityArtifact = (EntityArtifact)_mapObject.Entity;

        if (entityArtifact.ConfigAttribute != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = entityArtifact.ConfigAttribute.spriteMap;
        }

    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // LocalizedString myLocalizedString = new LocalizedString("ADVENTUREVENT", "artifact_yes")
        // {
        //     { "name", new StringVariable { Value = this.ScriptableData.name } },
        // };
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var configData = (EntityArtifact)_mapObject.Entity;
        var dialogData = new DataDialogMapObject()
        {
            Header = configData.ConfigAttribute.Text.title.GetLocalizedString(),
            Description = configData.ConfigAttribute.textOk.GetLocalizedString(),
            Sprite = configData.ConfigAttribute.MenuSprite
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
        MapObject.DoHero(player);

        Destroy(gameObject);
    }
}
