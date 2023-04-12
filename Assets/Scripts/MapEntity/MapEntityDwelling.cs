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
        // var mapObjectClass = (EntityDwelling)MapObjectClass;
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
            MapObjectClass.SetPlayer(player);
            SetPlayer(player);
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

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntityDwelling entity = (EntityDwelling)MapObjectClass;
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)MapObjectClass.ScriptableData;

        var nameCreature = configData.Creature[entity.Data.level].title.IsEmpty ?
            "" : configData.Creature[entity.Data.level].title.GetLocalizedString();

        LocalizedString description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "dwelling_d")
        {
            { "name", new StringVariable { Value = "<color=#FFFFAB>" + nameCreature + "</color>" } },
        };

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.title.GetLocalizedString(),
            Description = description.GetLocalizedString(),
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.OnlyOk,
            TypeWorkAttribute = configData.TypeWorkAttribute,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async UniTask<DataResultDialogDwelling> OnShowDialogDwelling()
    {
        EntityDwelling entity = (EntityDwelling)MapObjectClass;
        // ScriptableEntityDwelling configData = (ScriptableEntityDwelling)MapObjectClass.ScriptableData;

        var dialogWindow = new DialogDwellingProvider(entity);
        return await dialogWindow.ShowAndHide();
    }
}
