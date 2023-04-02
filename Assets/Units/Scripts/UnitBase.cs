using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
[System.Serializable]
public abstract class UnitBase : MonoBehaviour, IPointerClickHandler
{

    [NonSerialized] public GridTileNode OccupiedNode = null;
    [NonSerialized] public GridTileNode ProtectedNode = null;
    //public bool IsProtected => OccupiedNode.ProtectedUnit != null;
    [NonSerialized] public TypeInput typeInput;
    [NonSerialized] public TypeUnit typeUnit;
    [NonSerialized] public Vector3Int Position;
    [NonSerialized] public ScriptableUnitBase ScriptableData;
    protected string idUnit;
    protected string idObject;
    protected string idWar;

    //public int keyArea;
    //public Stats Stats { get; private set; }
    //public virtual void SetStats(Stats stats) => Stats = stats;

    public virtual void TakeDamage(int dmg)
    {

    }
    public virtual void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        ScriptableData = data;

        typeUnit = data.TypeUnit;
        typeInput = data.typeInput;
        Position = pos;
        idUnit = System.Guid.NewGuid().ToString("N");
        idObject = data.idObject;
        //transform.position = pos + new Vector3(.5f,.5f);
    }

    public virtual void UpdateAnimate(Vector3Int fromPosition, Vector3Int toPosition) { }

    //public virtual void SetPathHero(List<GridTileNode> path)
    //{

    //}

    //private void OnMouseDown()
    //{
    //    bool isTrigger = true;

    //    //Debug.Log($"UnitBase Click {this.name} {this.typeUnit} [{transform.position}]");
    //    Vector3 posObject = transform.position;

    //    if (OccupiedNode.Protected)
    //    {
    //        Debug.Log($"Exist warrior [{OccupiedNode.ProtectedUnit.name}]");
    //        //posObject = OccupiedNode.ProtectedUnit.Position;
    //    }
    //    if (typeInput == TypeInput.None)
    //    {
    //        //Debug.Log($"Click nopath [{OccupiedNode.Empty}]!");
    //        isTrigger = false;
    //    }
    //    //TileData dataTile = Grid2DManager.Instance.GetTileData(new Vector3Int((int)posObject.x, (int)posObject.y));
    //    //GridTileNode baseTile = Grid2DManager.Instance.GetMapObjectByPosition((int)posObject.x, (int)posObject.y);

    //    if (posObject != null)
    //    {

    //        //float sp = dataTile.speed;
    //        //bool isWalkable = baseTile.Walkable;
    //        //Vector3 posHero = UnitManager.Instance.activeHero.transform.position;
    //        //Vector3Int start = new Vector3Int((int)posHero.x, (int)posHero.y);
    //        Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
    //        //List<GridTileNode> path = 
    //        LevelManager.Instance.ActivePlayer.FindPathForHero(end, isTrigger, true);

    //        //if (path != null)
    //        //{


    //        //    UnitManager.Instance.ChangePathForHero(path);
    //        //    //for (int i = 0; i < path.Count - 1; i++)
    //        //    //    {
    //        //    //        GridTileNode pathNode = path[i];
    //        //    //        GridTileNode pathNodeNext = path[i + 1];

    //        //    //        // Grid2DManager.Instance.SetColorForTile(new Vector3Int(pathNodeNext.x, pathNodeNext.y, 0), Color.blue);

    //        //    //        Debug.DrawLine(
    //        //    //            new Vector3(pathNode.x + (.5f * 1), pathNode.y + (.5f * 1), 0.05f),
    //        //    //            new Vector3(pathNodeNext.x + (.5f * 1), pathNodeNext.y + (.5f * 1), 0.05f),
    //        //    //            Color.white, 2f
    //        //    //        );
    //        //    //        // UnitManager.Instance.ChangePositionActiveUnit(new Vector3Int(end.x, end.y, 0));

    //        //    //    // _player.GetComponent<Rigidbody>().MovePosition(new Vector3(pathNodeNext.x, _player.transform.position.y, pathNodeNext.z));
    //        //    //}
    //        //}

    //        //print("Click at " + this.name + " there is a " + baseTile + " speed " + sp + " isWalkable=" + isWalkable);

    //    }
    //}

    public async virtual void OnGoHero(Player player)
    {
        //Debug.Log($"OnGoHero::: player[{player.DataPlayer.id}]");

    }

    public virtual void OnNextDay()
    {
        // Debug.Log($"OnGoHero::: player[{player.DataPlayer.id}]");
    }

    //public virtual void OnSaveUnit()
    //{
    //    // SaveUnit(new object());
    //}
    protected SaveDataUnit<T> SaveUnit<T>(T Data)
    {
        var SaveData = new SaveDataUnit<T>();

        SaveData.idUnit = idUnit;
        SaveData.position = Position;
        SaveData.typeUnit = typeUnit;
        SaveData.idObject = idObject;
        SaveData.data = Data;

        return SaveData;
    }

    public virtual void OnLoadUnit<T>(SaveDataUnit<T> Data)
    {
        // LoadUnit();
    }
    protected void LoadUnit<T>(SaveDataUnit<T> Data)
    {
        idUnit = Data.idUnit;
        Position = Data.position;
        typeUnit = Data.typeUnit;
        idObject = Data.idObject;
    }

    protected virtual void Awake()
    {
        GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }

    protected virtual void OnDestroy()
    {
        GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    protected virtual void Start()
    {
        //Debug.Log($"Start {name}");
    }

    public virtual void OnBeforeStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.SaveGame:
                // OnSaveUnit();
                break;
        }
    }

    public virtual void OnAfterStateChanged(GameState newState)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isTrigger = true;

        Debug.Log($"" +
            $"UnitBase Click \n" +
            $"name-{this.name} \n" +
            $"type-{this.typeUnit}\n" +
            $"pos-[{transform.position}]\n" +
            $"ocup-[{OccupiedNode.ToString()}]\n"
            );
        Vector3 posObject = transform.position;

        if (OccupiedNode.Protected)
        {
            Debug.Log($"Exist warrior [{OccupiedNode.ProtectedUnit.name}]");
            //posObject = OccupiedNode.ProtectedUnit.Position;
        }
        if (typeInput == TypeInput.None)
        {
            //Debug.Log($"Click nopath [{OccupiedNode.Empty}]!");
            isTrigger = false;
        }
        //TileData dataTile = Grid2DManager.Instance.GetTileData(new Vector3Int((int)posObject.x, (int)posObject.y));
        //GridTileNode baseTile = Grid2DManager.Instance.GetMapObjectByPosition((int)posObject.x, (int)posObject.y);

        if (posObject != null)
        {

            //float sp = dataTile.speed;
            //bool isWalkable = baseTile.Walkable;
            //Vector3 posHero = UnitManager.Instance.activeHero.transform.position;
            //Vector3Int start = new Vector3Int((int)posHero.x, (int)posHero.y);
            Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
            //List<GridTileNode> path = 
            LevelManager.Instance.ActivePlayer.FindPathForHero(end, isTrigger, true);

            //if (path != null)
            //{


            //    UnitManager.Instance.ChangePathForHero(path);
            //    //for (int i = 0; i < path.Count - 1; i++)
            //    //    {
            //    //        GridTileNode pathNode = path[i];
            //    //        GridTileNode pathNodeNext = path[i + 1];

            //    //        // Grid2DManager.Instance.SetColorForTile(new Vector3Int(pathNodeNext.x, pathNodeNext.y, 0), Color.blue);

            //    //        Debug.DrawLine(
            //    //            new Vector3(pathNode.x + (.5f * 1), pathNode.y + (.5f * 1), 0.05f),
            //    //            new Vector3(pathNodeNext.x + (.5f * 1), pathNodeNext.y + (.5f * 1), 0.05f),
            //    //            Color.white, 2f
            //    //        );
            //    //        // UnitManager.Instance.ChangePositionActiveUnit(new Vector3Int(end.x, end.y, 0));

            //    //    // _player.GetComponent<Rigidbody>().MovePosition(new Vector3(pathNodeNext.x, _player.transform.position.y, pathNodeNext.z));
            //    //}
            //}

            //print("Click at " + this.name + " there is a " + baseTile + " speed " + sp + " isWalkable=" + isWalkable);

        }
    }
}
