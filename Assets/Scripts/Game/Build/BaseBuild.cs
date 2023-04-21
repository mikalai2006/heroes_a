using System;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public abstract class BaseBuild
{
    [NonSerialized] public UITown UITown;
    public int level;
    [NonSerialized] public ScriptableBuilding ConfigData;
    [NonSerialized] protected Player _player;
    public Player Player => _player;
    [NonSerialized] public BuildMonoBehavior BuildGameObject;
    [NonSerialized] public EntityTown Town;
    [NonSerialized] public bool _isPulse;

    #region Events GameState
    // public void AddEvents()
    // {
    //     GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
    //     GameManager.OnAfterStateChanged += OnAfterStateChanged;
    // }
    // public void RemoveEvents()
    // {
    //     GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
    //     GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    // }

    // public virtual void OnBeforeStateChanged(GameState newState)
    // {
    //     // switch (newState)
    //     // {
    //     //     case GameState.SaveGame:
    //     //         // OnSaveUnit();
    //     //         break;
    //     // }
    // }

    // public virtual void OnAfterStateChanged(GameState newState)
    // {
    // }
    #endregion

    public void Init(int level, EntityTown town, Player player)
    {
        this.level = level;
        this.Town = town;
        SetPlayer(player);
        // AddEvents();
    }

    public void CreateGameObject(bool isPulse = false)
    {
        _isPulse = isPulse;

        if (ConfigData != null)
        {
            // Draw GameObject.
            LoadGameObject();
        }
        else
        {
            Debug.LogWarning($"No config::: {ConfigData.name}");
        }
    }

    public virtual void OnRunOneEffect()
    {
        ((ScriptableBuilding)ConfigData).BuildLevels[level].RunOne(ref _player, Town);
    }

    public void DestroyGameObject()
    {
        // Debug.Log($"Destroy GameObject [{ConfigData.name}]");
        ConfigData.Prefab.ReleaseInstance(BuildGameObject.gameObject);
        BuildGameObject = null;
    }

    // public void DestroyBuild()
    // {
    //     // RemoveEvents();
    // }

    public void UpdateGameObject(bool isPulse = false)
    {
        BuildGameObject.InitGameObject(this);

        if (isPulse)
        {
            BuildGameObject.GoPulse();
        }
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    private void LoadGameObject()
    {
        if (ConfigData.Prefab.RuntimeKeyIsValid())
        {
            var boxTownGameObject = GameObject.FindGameObjectWithTag("Town");
            Addressables.InstantiateAsync(
                ConfigData.Prefab,
                boxTownGameObject.gameObject.transform.position,
                Quaternion.identity,
                boxTownGameObject.transform
                ).Completed += LoadedAsset;
        }
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            BuildGameObject = r_asset.GetComponent<BuildMonoBehavior>();
            UpdateGameObject();
            if (_isPulse)
            {
                BuildGameObject.GoPulse();
                _isPulse = false;
            }
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

}
