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

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {

            DataResultDialog result = await OnTriggeredHero();

            if (result.isOk)
            {
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

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var dialogData = new DataDialogMapObject()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    private void OnHeroGo(Player player)
    {
        var entity = (EntityMine)MapObjectClass;
        entity.SetPlayer(player);
        SetPlayer(player);
    }
}
