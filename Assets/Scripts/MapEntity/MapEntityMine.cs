using Cysharp.Threading.Tasks;

using UnityEngine;

public class MapEntityMine : BaseMapEntity, IDialogMapObjectOperation
{
    private SpriteRenderer _flag;
    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

    }
    protected override void Awake()
    {
        base.Awake();
        _flag = transform.Find("Flag")?.GetComponent<SpriteRenderer>();
    }
    protected override void Start()
    {
        base.Start();
    }

    public void SetPlayer(Player player)
    {
        _flag.color = player.DataPlayer.color;
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            // player.AddMines(this);
            var entity = (EntityMine)MapObjectClass;
            entity.SetPlayer(player);
            SetPlayer(player);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var dialogData = new DataDialog()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
