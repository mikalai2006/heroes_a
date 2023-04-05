using Cysharp.Threading.Tasks;

using UnityEngine;

[System.Serializable]
public struct DataArtifact
{

}
public class BaseArtifact : BaseMapObject, IDataPlay, IDialogMapObjectOperation
{
    public Transform _model;
    public DataArtifact Data;

    protected override void Awake()
    {
        base.Awake();
        _model = transform.Find("Model");
    }

    public override void InitUnit(ScriptableEntity data, Vector3Int pos)
    {
        base.InitUnit(data, pos);

        ScriptableEntityArtifact dataArtifact = (ScriptableEntityArtifact)data;

        if (dataArtifact.spriteMap != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = dataArtifact.spriteMap;

        }

    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // LocalizedString myLocalizedString = new LocalizedString("ADVENTUREVENT", "artifact_yes")
        // {
        //     { "name", new StringVariable { Value = this.ScriptableData.name } },
        // };
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = this.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = this.ScriptableData.MenuSprite
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public override async void OnGoHero(Player player)
    {
        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            // Set artifact for hero.
            Destroy(gameObject);
        }
        else
        {
            // Click cancel.
        }
    }

    public void LoadDataPlay(DataPlay data)
    {
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.artifacts.Add(sdata);
    }
}
