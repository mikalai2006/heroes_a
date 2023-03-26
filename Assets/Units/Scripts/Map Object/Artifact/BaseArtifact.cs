using Cysharp.Threading.Tasks;

using UnityEngine;

public class BaseArtifact : BaseMapObject, IDialogMapObjectOperation
{
    public Transform _model;

    protected override void Awake()
    {
        base.Awake();
        _model = transform.Find("Model");
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);

        ScriptableArtifact dataArtifact = (ScriptableArtifact)data;

        if (dataArtifact.sprite != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = dataArtifact.spriteMap;

        }

    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var dialogData = new DataDialog()
        {
            Description = this.ScriptableData.name,
            Header = this.name,
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
        }
        else
        {
            // Click cancel.
        }
    }
}
