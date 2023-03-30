using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResourceSystem : StaticInstance<ResourceSystem>
{

    public Dictionary<string, List<Object>> ResourceAssets = new Dictionary<string, List<Object>>();

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
            var output = await Addressables.LoadAssetAsync<T>(location).Task as T;
            createdObjs.Add(output);
        }
    }

    public List<ScriptableGameMode> GetGameMode()
    {
        // ResourceAssets.Values.OfType<ScriptableGameMode>().ToList();
        return GetAllAssetsByLabel<ScriptableGameMode>("gamemode");
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

    public List<TileNature> GetNature() => GetAllAssetsByLabel<TileNature>("nature");
    public TileNature GetNature(string id) => GetNature().Where(t => t.idObject == id)?.First();
    public List<TileLandscape> GetLandscape() => GetAllAssetsByLabel<TileLandscape>("landscape");
    public TileLandscape GetLandscape(TypeGround typeGround) => GetLandscape().Where(t => t.typeGround == typeGround).First();
    public List<ScriptableUnitBase> GetUnits() => GetAllAssetsByLabel<ScriptableUnitBase>("units");

    public List<ScriptableBuildBase> GetCastleTown() => GetAllAssetsByLabel<ScriptableBuildBase>(Constants.Towns.TOWN_CASTLE);
    public List<T> GetUnitsByType<T>(TypeUnit typeUnit) where T : ScriptableUnitBase
    {
        var listUnits = GetUnits();

        List<T> units = new List<T>();

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (typeUnit == listUnits[i].TypeUnit) units.Add((T)listUnits[i]);
        }

        return units;
    }

    public T GetUnit<T>(TypeUnit typeUnit) where T : ScriptableUnitBase
    {
        var listUnits = GetUnits();

        // Filter units by faction.
        List<ScriptableUnitBase> units = new List<ScriptableUnitBase>();

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (typeUnit == listUnits[i].TypeUnit) units.Add(listUnits[i]);
        }
        var index = Random.Range(0, units.Count);

        return (T)units[index];
    }
    public T GetUnit<T>(TypeUnit typeUnit, TypeGround typeGround) where T : ScriptableUnitBase
    {
        var listUnits = GetUnits();

        List<ScriptableUnitBase> units = new List<ScriptableUnitBase>();

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (typeUnit == listUnits[i].TypeUnit && typeGround == listUnits[i].typeGround)
                units.Add(listUnits[i]);
        }

        var index = Random.Range(0, units.Count);

        return (T)units[index];
    }
    public T GetUnit<T>(string idObject) where T : ScriptableUnitBase
    {
        var listUnits = GetUnits();

        ScriptableUnitBase unitById = null;

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (idObject == listUnits[i].idObject)
            {
                unitById = listUnits[i];
                break;
            }
        }

        return (T)unitById;
    }
}