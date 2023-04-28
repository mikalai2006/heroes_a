using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class MapEntityMine : BaseMapEntity, IDialogMapObjectOperation
{
    private SpriteRenderer _flag;

    protected override void Awake()
    {
        base.Awake();
        _flag = transform.Find("Flag")?.GetComponent<SpriteRenderer>();
    }

    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
        if (mapObject.Entity.Player != null)
        {
            SetPlayer(mapObject.Entity.Player);
        }
    }

    public void SetPlayer(Player player)
    {
        _flag.color = player.DataPlayer.color;
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        DataDialogMapObjectGroup _dialogData = new DataDialogMapObjectGroup()
        {
            Values = new List<DataDialogMapObjectGroupItem>()
        };
        EntityMine entity = (EntityMine)_mapObject.Entity;
        ScriptableEntityMine configData = (ScriptableEntityMine)_mapObject.ConfigData;
        foreach (var effect in configData.Effects[entity.Effects.index].Item.items)
        {
            effect.CreateDialogData(ref _dialogData, entity);
        }

        var description = configData.Effects[entity.Effects.index].Item.description.IsEmpty ?
            "" : configData.Effects[entity.Effects.index].Item.description.GetLocalizedString();

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.Text.title.GetLocalizedString(),
            Description = description,
            Sprite = configData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

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
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
            OnHeroGo(player);
        }
    }

    private void OnHeroGo(Player player)
    {
        var entity = (EntityMine)_mapObject.Entity;
        entity.SetPlayer(player);
        SetPlayer(player);
    }
}
