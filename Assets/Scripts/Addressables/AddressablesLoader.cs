using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressablesLoader
{
    public static Dictionary<string, List<Object>> ResourceAssets = new Dictionary<string, List<Object>>();

    public static List<T> GetAllAssetsByLabel<T>(string label)
    {
        Debug.Log($"GetAllAssetsByLabel::: {label}");
        var list = new List<Object>();
        ResourceAssets.TryGetValue(label, out list);
        return list as List<T>;
    }


    public static async Task InitByNameOrLabel<T>(string assetNameOrLabel, List<T> createdObjs)
       where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetNameOrLabel).Task;

        await CreateObjectThenUpdateCollection(locations, createdObjs);

        List<Object> list = new List<T>() as List<Object>;

        ResourceAssets.Add(assetNameOrLabel, list);

    }
    public static async Task<List<T>> LoadCollectionsAsset<T>(string assetNameOrLabel)
       where T : Object
    {
        List<T> createdObjs = new List<T>();
        var locations = await Addressables.LoadResourceLocationsAsync(assetNameOrLabel).Task;

        await CreateAssetsThenUpdateCollection<T>(locations, createdObjs);

        List<Object> list = createdObjs as List<Object>;

        if (!ResourceAssets.ContainsKey(assetNameOrLabel))
        {
            ResourceAssets.Add(assetNameOrLabel, list);
            Debug.Log($"Load {assetNameOrLabel}::: {createdObjs.Count}");

        } else
        {
            Debug.LogWarning($" {assetNameOrLabel} is exists");

        }

        //foreach (var asset in createdObjs)
        //{
        //    Debug.Log($"asset name = {asset.GetType()}");

        //}
        return createdObjs;
    }
    //public static async Task IniByLoadedAddress<T>(IList<IResourceLocation> loadedLocations, List<T> createdObjs)
    //where T : Object
    //{
    //    await CreateAssetsThenUpdateCollection(loadedLocations, createdObjs);
    //}

    private static async Task CreateObjectThenUpdateCollection<T>(IList<IResourceLocation> locations, List<T> createdObjs)
        where T : Object
    {
        foreach (var location in locations)
            createdObjs.Add(await Addressables.InstantiateAsync(location).Task as T);
    }

    private static async Task CreateAssetsThenUpdateCollection<T>(IList<IResourceLocation> locations, List<T> createdObjs)
        where T : Object
    {
        foreach (var location in locations)
        {
            createdObjs.Add(await Addressables.LoadAssetAsync<T>(location).Task as T);
        }
    }
}