using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityDwelling : BaseMapEntity, IDialogMapObjectOperation
{
    private SpriteRenderer _flag;
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
        if (mapObject.Player != null)
        {
            SetPlayer(mapObject.Player);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        _flag = transform.Find("Flag")?.GetComponent<SpriteRenderer>();
    }

    public void SetPlayer(Player player)
    {
        _flag.color = player.DataPlayer.color;
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
        MapObjectClass.SetPlayer(player);
        SetPlayer(player);
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntityDwelling entity = (EntityDwelling)MapObjectClass;
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)MapObjectClass.ScriptableData;

        var dataPlural = new Dictionary<string, int> { { "value", 0 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            configData.Creature[entity.Data.level].title,
            arguments,
            dataPlural
            );

        LocalizedString description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "dwelling_d")
        {
            { "name", new StringVariable { Value = " <color=#FFFFAB>" + titlePlural + "</color>" } },
        };

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.title.GetLocalizedString(),
            Description = description.GetLocalizedString(),
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.OnlyOk,
            TypeWorkEffect = configData.TypeWorkEffect,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async UniTask<DataResultDialogDwelling> OnShowDialogDwelling()
    {
        EntityDwelling entity = (EntityDwelling)MapObjectClass;
        var dialogWindow = new DialogDwellingProvider(entity);
        return await dialogWindow.ShowAndHide();
    }
}
