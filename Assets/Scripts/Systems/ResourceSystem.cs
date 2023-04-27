using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResourceSystem : StaticInstance<ResourceSystem>
{

    public Dictionary<string, List<Object>> ResourceAssets = new Dictionary<string, List<Object>>();
    // private Dictionary<string, ScriptableAttribute> Attributes = new Dictionary<string, ScriptableAttribute>();

    #region Asset load and destroy
    public async Task<List<T>> LoadCollectionsAsset<T>(string assetNameOrLabel)
       where T : Object
    {
        List<T> createdObjs = new List<T>();
        var locations = await Addressables.LoadResourceLocationsAsync(assetNameOrLabel).Task;

        await CreateAssetsThenUpdateCollection<T>(locations, createdObjs);

        List<Object> list = new List<Object>();
        foreach (var asset in createdObjs)
        {
            //Debug.Log($"list name = {asset.GetType()}");
            list.Add(asset);
        }
        if (!ResourceAssets.ContainsKey(assetNameOrLabel))
        {
            ResourceAssets.Add(assetNameOrLabel, list);
            //Debug.Log($"Load {assetNameOrLabel}::: {list.Count}");

        }
        else
        {
            Debug.LogWarning($" {assetNameOrLabel} is exists");

        }
        return createdObjs;
    }


    private async Task CreateAssetsThenUpdateCollection<T>(IList<IResourceLocation> locations, List<T> createdObjs)
        where T : Object
    {
        foreach (var location in locations)
        {
            if (location.ResourceType.IsSubclassOf(typeof(T)) || location.ResourceType == typeof(T))
            {
                var output = await Addressables.LoadAssetAsync<T>(location).Task as T;
                // Debug.Log($"ResourceType={location.ResourceType}, typeof(T)({typeof(T)})[{location.ResourceType is T}]");
                // Debug.Log($"IsSubclassOf = {location.ResourceType.IsSubclassOf(typeof(T))}");
                // Debug.Log($"output is T[{output is T}]");
                createdObjs.Add(output);
            }
        }
    }

    public void DestroyAssets()
    {
        foreach (var asset in ResourceAssets)
        {
            if (asset.Value.Count > 0)
            {
                foreach (var item in asset.Value)
                {
                    if (item != null) Addressables.Release(item);
                }
            }
        }
    }

    public void DestroyAssetsByLabel(string label)
    {
        ResourceAssets.TryGetValue(label, out List<Object> list);
        foreach (var asset in list)
        {
            if (asset != null) Addressables.Release(asset);
        }
    }
    public void DestroyAsset(Object asset)
    {
        if (asset != null) Addressables.Release(asset);
    }
    #endregion

    public List<T> GetAllAssetsByLabel<T>(string label)
       where T : Object
    {
        var output = new List<T>();
        ResourceAssets.TryGetValue(label, out List<Object> list);
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i] as T;
            output.Add(item);
        }

        //Debug.Log($"GetAllAssetsByLabel::: label={label}, values={ttt.Count}");
        return output;
    }

    #region managers entity
    public List<ScriptableEntity> AllEntity() => GetAllAssetsByLabel<ScriptableEntity>(Constants.Labels.LABEL_ENTITY);
    public List<T> GetEntityByType<T>(TypeEntity typeEntity) where T : ScriptableEntity
    {
        var listUnits = AllEntity();

        List<T> items = new List<T>();

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (typeEntity == listUnits[i].TypeEntity) items.Add((T)listUnits[i]);
        }

        return items;
    }
    #endregion

    #region managers attribute
    public List<ScriptableAttribute> AllAttributes() => GetAllAssetsByLabel<ScriptableAttribute>(Constants.Labels.LABEL_ATTRIBUTE);
    public List<T> GetAttributesByType<T>(TypeAttribute typeAttribute) where T : ScriptableAttribute
    {
        var listUnits = AllAttributes();

        List<T> items = new List<T>();

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (typeAttribute == listUnits[i].TypeAttribute) items.Add((T)listUnits[i]);
        }

        return items;
    }
    #endregion

    public List<SOGameMode> GetGameMode()
    {
        // ResourceAssets.Values.OfType<ScriptableGameMode>().ToList();
        return GetAllAssetsByLabel<SOGameMode>("gamemode");
    }

    public List<TileNature> GetNature() => GetAllAssetsByLabel<TileNature>("nature");
    public TileNature GetNature(string id) => GetNature().Where(t => t.idObject == id)?.First();
    public List<TileLandscape> GetLandscape() => GetAllAssetsByLabel<TileLandscape>("landscape");
    public TileLandscape GetLandscape(TypeGround typeGround) => GetLandscape().Where(t => t.typeGround == typeGround).First();

    // public List<ScriptableAttributeTown> GetBuildTowns() => GetAllAssetsByLabel<ScriptableAttributeTown>(Constants.Labels.LABEL_BUILD_TOWN);
    public List<ScriptableBuilding> GetAllBuildsForTown() => GetAllAssetsByLabel<ScriptableBuilding>(Constants.Labels.LABEL_BUILD_BASE);

    // public List<ScriptableMapObjectBase> GetUnits() => GetAllAssetsByLabel<ScriptableMapObjectBase>("units");
    // public List<T> GetUnitsByType<T>(TypeMapObject typeUnit) where T : ScriptableMapObjectBase
    // {
    //     var listUnits = GetUnits();

    //     List<T> units = new List<T>();

    //     for (int i = 0; i < listUnits.Count; i++)
    //     {
    //         if (typeUnit == listUnits[i].TypeMapObject) units.Add((T)listUnits[i]);
    //     }

    //     return units;
    // }

    // public T GetUnit<T>(TypeMapObject typeUnit) where T : ScriptableMapObjectBase
    // {
    //     var listUnits = GetUnits();

    //     // Filter units by faction.
    //     List<ScriptableMapObjectBase> units = new List<ScriptableMapObjectBase>();

    //     for (int i = 0; i < listUnits.Count; i++)
    //     {
    //         if (typeUnit == listUnits[i].TypeMapObject) units.Add(listUnits[i]);
    //     }
    //     var index = Random.Range(0, units.Count);

    //     return (T)units[index];
    // }
    // public T GetUnit<T>(TypeMapObject typeUnit, TypeGround typeGround) where T : ScriptableMapObjectBase
    // {
    //     var listUnits = GetUnits();

    //     List<ScriptableMapObjectBase> units = new List<ScriptableMapObjectBase>();

    //     for (int i = 0; i < listUnits.Count; i++)
    //     {
    //         if (typeUnit == listUnits[i].TypeMapObject && typeGround == listUnits[i].typeGround)
    //             units.Add(listUnits[i]);
    //     }

    //     var index = Random.Range(0, units.Count);

    //     return (T)units[index];
    // }
    // public T GetUnit<T>(string idObject) where T : ScriptableMapObjectBase
    // {
    //     var listUnits = GetUnits();

    //     ScriptableMapObjectBase unitById = null;

    //     for (int i = 0; i < listUnits.Count; i++)
    //     {
    //         if (idObject == listUnits[i].idObject)
    //         {
    //             unitById = listUnits[i];
    //             break;
    //         }
    //     }

    //     return (T)unitById;
    // }
}