using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public abstract class BaseBuild
{
    [NonSerialized] public UITown UITown;
    // public string idObject;
    public int level;
    [NonSerialized] public ScriptableBuildBase ConfigData;
    [NonSerialized] protected Player _player;
    public Player Player => _player;
    [NonSerialized] public BuildMonoBehavior BuildGameObject;

    // public List<GameObject> Childrens;

    // public ScriptableBuildBase ScriptableData;
    // public TypeBuild TypeBuild;
    // public int LevelBuild;

    // #region Events GameState
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

    // public virtual void OnAfterStateChanged(GameState newState)
    // {
    //     switch (newState)
    //     {
    //         case GameState.StepNextPlayer:
    //             OnNextDay();
    //             break;
    //     }
    // }

    // public virtual void OnBeforeStateChanged(GameState newState)
    // {
    // }

    // private void OnNextDay()
    // {
    //     if (LevelManager.Instance.ActivePlayer == null) return;

    // }
    // #endregion

    public void Init(ScriptableBuildBase data, int level)
    {
        // idObject = data.idObject;
        this.level = level;
        // if (saveData == null)
        // {
        // }
        // else
        // {
        //     level = saveData.level;
        // }
        // AddEvents();
    }

    public void CreateGameObject()
    {
        if (ConfigData != null)
        {
            // Draw GameObject.
            LoadGameObject();
        }
    }
    // public void DestroyBuild()
    // {
    //     RemoveEvents();
    // }

    public void UpdateGameObject()
    {
        BuildGameObject.InitGameObject(this);
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
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            // BuildGameObject.InitGameObject(this);
            UpdateGameObject();
            // obj.Init(this);
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

    // public virtual void Init(UITown uiTown)
    // {
    //     UITown = uiTown;
    //     // Player player = LevelManager.Instance.ActivePlayer;
    //     // for (int i = 0; i < ScriptableData.BuildLevels.Count; i++)
    //     // {
    //     //     var bl = ScriptableData.BuildLevels[i];
    //     //     if ((player.ActiveTown.Data.ProgressBuilds & bl.TypeBuild) == bl.TypeBuild)
    //     //     {
    //     //         SetActiveLevel(i);
    //     //     }
    //     // }
    //     // Debug.Log($"Init {name}");
    // }

    // private void SetActiveLevel(int i)
    // {
    //     for (int j = 0; j < Childrens.Count; j++)
    //     {
    //         Childrens[j].SetActive(j == i ? true : false);
    //     }
    // }
}
