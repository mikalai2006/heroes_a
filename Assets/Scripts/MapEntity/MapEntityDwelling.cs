using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityDwelling : BaseMapEntity, IDialogMapObjectOperation
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
        EntityDwelling entity = (EntityDwelling)_mapObject.Entity;
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)_mapObject.ConfigData;

        var dataPlural = new Dictionary<string, int> { { "value", 0 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            configData.Creature[entity.Data.level].Text.title,
            arguments,
            dataPlural
            );

        LocalizedString description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "dwelling_d")
        {
            { "name", new StringVariable { Value = " <color=#FFFFAB>" + titlePlural + "</color>" } },
        };

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.Text.title.GetLocalizedString(),
            Description = description.GetLocalizedString(),
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.OnlyOk,
            TypeWorkEffect = configData.TypeWorkEffect,
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
                DataResultDialogDwelling resultDwelling = await OnShowDialogDwelling();
                if (resultDwelling.isOk)
                {

                }
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
        _mapObject.DoHero(player);
        SetPlayer(player);
    }


    public async UniTask<DataResultDialogDwelling> OnShowDialogDwelling()
    {
        EntityDwelling dwelling = (EntityDwelling)_mapObject.Entity;
        var dialogWindow = new DialogDwellingProvider(new DataDialogDwelling()
        {
            Creatures = LevelManager.Instance.ActivePlayer.ActiveHero.Data.Creatures,
            dwelling = dwelling
        });
        return await dialogWindow.ShowAndHide();
    }
}
