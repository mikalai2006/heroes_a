using System;

using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public abstract class BaseMapEntity : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected BaseEntity MapObjectClass;
    public BaseEntity GetMapObjectClass => MapObjectClass;
    public virtual void InitUnit(BaseEntity mapObject)
    {
        MapObjectClass = mapObject;
    }

    // public virtual void UpdateAnimate(Vector3Int fromPosition, Vector3Int toPosition) { }

    public virtual void OnGoHero(Player player)
    {
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
        MapObjectClass.DestroyEntity();
        MapObjectClass = null;

        GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    protected virtual void Start()
    {
        //Debug.Log($"Start {name}");
    }
    public void DestroyGameObject()
    {
        Destroy(gameObject);
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

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
        {
        }
        else
        {

            Debug.Log($"" +
                $"UnitBase Click \n" +
                $"name-{this.name} \n" +
                $"pos-[{transform.position}]\n" +
                $"ocup-[{MapObjectClass.OccupiedNode.ToString()}]\n"
                );
            Vector3 posObject = transform.position;

            if (posObject != null)
            {
                Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
                LevelManager.Instance.ActivePlayer.FindPathForHero(end, true);
            }
        }
    }
}
