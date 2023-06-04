using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapEntityDwelling : BaseMapEntity, IDialogMapObjectOperation
{
    private SpriteRenderer _flag;
    private EntityDwelling entity => (EntityDwelling)_mapObject.Entity;
    private ScriptableEntityDwelling configData => (ScriptableEntityDwelling)_mapObject.ConfigData;
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
        // EntityDwelling entity = (EntityDwelling)_mapObject.Entity;
        // ScriptableEntityDwelling configData = (ScriptableEntityDwelling)_mapObject.ConfigData;

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
            if (entity.Data.quantityDefenders > 0)
            {
                // Show message of battle.
                var resultDefender = await OnShowDialogDefenders();
                if (!resultDefender.isOk)
                {
                    return;
                }

                // Get setting for arena.
                var arenaSetting = LevelManager.Instance.ConfigGameSettings.ArenaSettings
                    .Where(t => t.NativeGround.typeGround == MapObject.OccupiedNode.TypeGround)
                    .ToList();
                // TODO ARENA
                var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
                {
                    heroAttacking = player.ActiveHero,
                    creature = entity.Defender,
                    town = null,
                    ArenaSetting = arenaSetting[Random.Range(0, arenaSetting.Count())]
                });
                var resultArena = await loadingOperations.ShowHide();
                if (resultArena.isWinRightHero)
                {
                    // TODO проигрыш
                    return;
                }
            }

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
    public async UniTask<DataResultDialog> OnShowDialogDefenders()
    {
        string nameText = Helpers.GetStringNameCountWarrior(entity.Data.quantityDefenders);
        LocalizedString stringCountWarriors = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, nameText);

        var dataPlural = new Dictionary<string, int> { { "value", 0 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            configData.Creature[0].Text.title,
            arguments,
            dataPlural
            );
        var title = string.Format("{0}({1}) {2}", stringCountWarriors.GetLocalizedString(), entity.Data.quantityDefenders, titlePlural);

        var dataDefender = new Dictionary<string, object> {
            { "value", title },
            { "dwellingname", configData.Text.title.GetLocalizedString() }
        };
        var argumentsDefender = new[] { dataDefender };
        var description = Helpers.GetLocalizedPluralString(
            new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "dwellingdef_d"),
            argumentsDefender,
            dataDefender
            );

        var dialogData = new DataDialogHelp()
        {
            Header = configData.Text.title.GetLocalizedString(),
            Description = description,
            showCancelButton = true
        };

        var dialogWindow = new DialogHelpProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
