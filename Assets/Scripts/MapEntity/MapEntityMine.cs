using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[System.Serializable]
public struct DataMine
{
    public int idPlayer;
    public bool isMeet;

}

public class MapEntityMine : BaseMapEntity, IDataPlay, IDialogMapObjectOperation
{
    public DataMine Data = new DataMine();
    private SpriteRenderer _flag;
    public override void InitUnit(ScriptableEntity data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data.idPlayer = -1;

    }
    public override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
        if (newState == GameState.StepNextPlayer)
        {
            Player player = LevelManager.Instance.ActivePlayer;
            if (Data.idPlayer == player.DataPlayer.id)
            {
                var data = (ScriptableEntityMapObject)ScriptableData;
                if (data.Resources.Count > 0)
                {
                    var res = data.Resources[0].ListVariant[0].Resource;
                    player.ChangeResource(res.TypeResource, res.maxValue);
                }
            }
        }
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
        Data.idPlayer = player.DataPlayer.id;

        // Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        Debug.Log($"Flagg::: {_flag.name}");
        _flag.color = player.DataPlayer.color;
    }

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }
    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.mines.Add(sdata);
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            player.AddMines(this);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {

        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = this.ScriptableData.Text.title.GetLocalizedString(), //t.Text.title,
            // Description = t.Text.visit_ok,
            Sprite = this.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
