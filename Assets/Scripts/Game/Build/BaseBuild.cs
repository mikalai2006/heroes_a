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


    public void Init(int level, EntityTown town)
    {
        this.level = level;
        this.Town = town;
    }

    public void CreateGameObject()
    {
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
            UpdateGameObject();
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }
}
