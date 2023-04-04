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

public abstract class BaseMines : UnitBase, IDataPlay, IDialogMapObjectOperation
{
    public DataMine Data = new DataMine();
    private SpriteRenderer _flag;
    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data.idPlayer = -1;

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

        var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = t.Text.title,
            Description = t.Text.visit_ok,
            Sprite = this.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
