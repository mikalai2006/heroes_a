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

        if (dataArtifact.Artifact != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = dataArtifact.Artifact.spriteMap;
        }

    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // LocalizedString myLocalizedString = new LocalizedString("ADVENTUREVENT", "artifact_yes")
        // {
        //     { "name", new StringVariable { Value = this.ScriptableData.name } },
        // };
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialogMapObject()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public override async void OnGoHero(Player player)
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

            OnHeroGo(player);
        }
    }

    private void OnHeroGo(Player player)
    {
        MapObjectClass.SetPlayer(player);
        Destroy(gameObject);
    }
}
