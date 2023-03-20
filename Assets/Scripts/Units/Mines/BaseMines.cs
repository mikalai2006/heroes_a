using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DataMine
{
    public int idPlayer;
    public bool isMeet;

}

public abstract class BaseMines : UnitBase, IDataPlay
{
    public DataMine Data = new DataMine();
    private SpriteRenderer _flag;
    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data.idPlayer = -1;

    }
    protected override void Start()
    {
        base.Start();
        _flag = transform.Find("Flag")?.GetComponent<SpriteRenderer>();
    }

    public void SetPlayer(PlayerData data)
    {
        Data.idPlayer = data.id;

        Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        _flag.color = player.DataPlayer.color;
    }

    public override void OnGoHero(Player player)
    {
        base.OnGoHero(player);
        player.AddMines(this);
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
}
