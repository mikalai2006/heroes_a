using UnityEngine;

[System.Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    public TypeBuild ProgressBuilds;
}

public abstract class BaseTown : UnitBase, IDataPlay
{

    [SerializeField] public DataTown Data;

    //private void Awake() => GameManager.OnBeforeStateChanged += OnStateChanged;

    //private void OnDestroy() => GameManager.OnBeforeStateChanged -= OnStateChanged;

    public override void OnAfterStateChanged(GameState newState)
    {
        //if (newState == GameState.HeroTurn) _canMove = true;
        base.OnAfterStateChanged(newState);
    }
    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);
        Data.name = data.name;
        Data.ProgressBuilds = TypeBuild.None; // TypeBuild.Tavern_1;
    }

    public void SetPlayer(PlayerData data)
    {
        //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");
        Data.idPlayer = data.id;

        Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    }

    public virtual void ExecuteMove()
    {
        // Override this to do some hero-specific logic, then call this base method to clean up the turn

        //_canMove = false;
    }

    //public override void OnSaveUnit()
    //{
    //    // SaveUnit(Data);
    //}

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }
    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.towns.Add(sdata);
    }
}