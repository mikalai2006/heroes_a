using System;

using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public abstract class BaseMapEntity : MonoBehaviour, IPointerClickHandler
{
    [NonSerialized] protected BaseEntity MapObjectClass;
    public BaseEntity GetMapObjectClass => MapObjectClass;

    // public virtual void TakeDamage(int dmg)
    // {

    // }
    public virtual void InitUnit(BaseEntity mapObject)
    {
        MapObjectClass = mapObject;
    }

    // public virtual void UpdateAnimate(Vector3Int fromPosition, Vector3Int toPosition) { }

    public virtual void OnGoHero(Player player)
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
    // protected SaveDataUnit<T> SaveUnit<T>(T Data)
    // {
    //     var SaveData = new SaveDataUnit<T>();

    //     // SaveData.idUnit = idUnit;
    //     // SaveData.position = Position;
    //     // // SaveData.typeEntity = typeEntity;
    //     // // SaveData.typeMapObject = typeMapObject;
    //     // SaveData.idObject = idObject;
    //     // SaveData.data = Data;

    //     return SaveData;
    // }

    // public virtual void OnLoadUnit<T>(SaveDataUnit<T> Data)
    // {
    //     // LoadUnit();
    // }
    // protected void LoadUnit<T>(SaveDataUnit<T> Data)
    // {
    //     // idUnit = Data.idUnit;
    //     // Position = Data.position;
    //     // // typeMapObject = Data.typeMapObject;
    //     // // typeEntity = Data.typeEntity;
    //     // idObject = Data.idObject;
    // }

    protected virtual void Awake()
    {
        GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }

    protected virtual void OnDestroy()
    {
        // Debug.LogWarning($"object [{MapObjectClass.ScriptableData.name}] - gameObject[{gameObject.name}]");
        MapObjectClass.ScriptableData.MapPrefab.ReleaseInstance(gameObject);
        MapObjectClass = null;

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
            // $"type-{this.typeEntity}\n" +
            $"pos-[{transform.position}]\n" +
            $"ocup-[{MapObjectClass.OccupiedNode.ToString()}]\n"
            );
        Vector3 posObject = transform.position;

        // if (OccupiedNode.Protected)
        // {
        //     Debug.Log($"Exist warrior [{OccupiedNode.ProtectedUnit.name}]");
        //     //posObject = OccupiedNode.ProtectedUnit.Position;
        // }
        if (MapObjectClass.ScriptableData.typeInput == TypeInput.None)
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
