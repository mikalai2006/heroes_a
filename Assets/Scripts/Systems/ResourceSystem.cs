using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem> {
    private List<ScriptableUnitBase> _units;
    //public List<ScriptableHero> Heroes { get; private set; }
    public Dictionary<TypeGround, TileLandscape> Landscape { get; private set; } = new Dictionary<TypeGround, TileLandscape>();
    public List<TileNature> Nature { get; private set; } = new List<TileNature>();
    //public List<ScriptableResource> Resource { get; private set; } = new List<ScriptableResource>();
    //public Dictionary<TypeUnit, List<ScriptableUnitBase>> Units { get; private set; } = new Dictionary<TypeUnit, List<ScriptableUnitBase>>();
    //public Dictionary<TypeGround, ScriptableTown> Towns { get; private set; }
    //public Dictionary<TypeGround, List<ScriptableHero>> Heroes { get; private set; } = new Dictionary<TypeGround, List<ScriptableHero>>();  
    //public List<ScriptableWarriors> Warriors { get; private set; } = new List<ScriptableWarriors>();    
    //public Dictionary<MapObjectType, List<ScriptableMapObject>> MapObjects { get; private set; } = new Dictionary<MapObjectType, List<ScriptableMapObject>>();

    protected override void Awake() {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        ////Directory.GetFiles("Assets/Resources/Landscape", "*.asset",
        ////    SearchOption.AllDirectories)
        Landscape = Resources.LoadAll<TileLandscape>("Landscape").ToDictionary(t => t.typeGround, t => t);
        Nature = Resources.LoadAll<TileNature>("Nature").ToList();
        //Resource = Resources.LoadAll<ScriptableResource>("Units/Resources").ToList();

        _units = Resources.LoadAll<ScriptableUnitBase>("Units").ToList();
        //foreach (var unit in _units)
        //{
        //    if (!Units.ContainsKey(unit.TypeUnit)) Units.Add(unit.TypeUnit, new List<ScriptableUnitBase>());
        //    Units[unit.TypeUnit].Add(unit);
        //}
        ////List<ScriptableUnitBase> Towns = Resources.LoadAll<ScriptableTown>("Towns").ToList();
        ////Heroes = Resources.LoadAll<ScriptableHero>("Heroes").ToList();
        //Towns = Resources.LoadAll<ScriptableTown>("Units/Towns")
        //    .ToList()
        //    .ToDictionary(r => r.typeGround, r => r);
        ////Towns = _towns.ToDictionary(r => r.typeGround, r => r);

        //List<ScriptableHero> heroes = Resources.LoadAll<ScriptableHero>("Units/Heroes").ToList();
        //foreach (var hero in heroes)
        //{
        //    if (!Heroes.ContainsKey(hero.typeGround)) Heroes.Add(hero.typeGround, new List<ScriptableHero>());
        //    Heroes[hero.typeGround].Add(hero);
        //}

        //Warriors = Resources.LoadAll<ScriptableWarriors>("Units/Warriors").ToList();
        ////foreach (var warrior in warriors)
        ////{
        ////    if (!Warriors.ContainsKey(warrior.typeGround)) Warriors.Add(warrior.typeGround, new List<ScriptableWarriors>());
        ////    Warriors[warrior.typeGround].Add(warrior);
        ////}

        //List<ScriptableMapObject> _MapObjects = Resources.LoadAll<ScriptableMapObject>("Units/MapObject").ToList();
        //foreach (var mapObject in _MapObjects) {
        //    if (!MapObjects.ContainsKey(mapObject.mapObjectType)) MapObjects.Add(mapObject.mapObjectType, new List<ScriptableMapObject>());
        //    MapObjects[mapObject.mapObjectType].Add(mapObject); //  = _MapObjects.ToDictionary(r => r.mapObjectType, r => r);
        //}
    }

    //public T GetRandomHero<T>(TypeUnit faction) where T : ScriptableHero
    //{
    //    return (T)_units.Where(h => h.TypeUnit == faction).OrderBy(o => Random.value).First();
    //}

    public List<TileNature> GetNature() => Nature;
    public TileNature GetNature(string id) => Nature.Where(t => t.idObject == id)?.First();
    public TileLandscape GetLandscape(TypeGround typeGround) => Landscape[typeGround];
    public Dictionary<TypeGround, TileLandscape> AllLandscape() => Landscape;


    //public ScriptableTown GetTown(TypeGround typeGround) => (ScriptableTown)_units.Where(t => t.TypeUnit == TypeUnit.Town && t.typeGround == typeGround).First();
    //public List<ScriptableMapObject> GetMapObject(MapObjectType t) => MapObjects[t];
    //public List<ScriptableHero> GetHeroes(TypeGround t) => Heroes[t];
    //public List<ScriptableWarriors> GetWarriors() => Warriors;

    //public List<ScriptableResource> GetResource() => Resource;
    //public ScriptableResource GetResourceById(string id) => Resource.Where(t => t.idObject == id).First();
    //public List<ScriptableUnitBase> GetUnitsByTypeUnit(TypeUnit typeUnit) => Units[typeUnit];

    public List<T> GetUnitsByType<T>(TypeUnit typeUnit) where T: ScriptableUnitBase
    {

        // Filter units by faction.
        List<T> units = new List<T>();

        for (int i = 0; i < _units.Count; i++)
        {
            if (typeUnit == _units[i].TypeUnit) units.Add((T)_units[i]);
        }
        //Debug.Log($"units {typeUnit} Count = {units.Count}");

        return units;
    }

    public T GetUnit<T>(TypeUnit typeUnit) where T : ScriptableUnitBase
    {

        // Filter units by faction.
        List<ScriptableUnitBase> units = new List<ScriptableUnitBase>();

        for (int i = 0; i < _units.Count; i++)
        {
            if (typeUnit == _units[i].TypeUnit) units.Add(_units[i]);
        }
        //Debug.Log($"units {typeUnit} Count = {units.Count}");
        var index = Random.Range(0, units.Count);

        return (T)units[index];
    }
    public T GetUnit<T>(TypeUnit typeUnit, TypeGround typeGround) where T : ScriptableUnitBase
    {

        List<ScriptableUnitBase> units = new List<ScriptableUnitBase>();

        for (int i = 0; i < _units.Count; i++)
        {
            if (typeUnit == _units[i].TypeUnit && typeGround == _units[i].typeGround)
                units.Add(_units[i]);
        }
        //Debug.Log($"units {typeUnit} Count = {units.Count}");
        var index = Random.Range(0, units.Count);

        return (T)units[index];
    }
    public T GetUnit<T>(string idObject) where T : ScriptableUnitBase
    {

        ScriptableUnitBase unitById = null;

        for (int i = 0; i < _units.Count; i++)
        {
            if (idObject == _units[i].idObject)
            {
                unitById = _units[i];
                break;
            }
        }

        return (T)unitById;
    }
    //public T GetUnitByGroundType<T>(TypeUnit typeUnit, TypeGround typeGround) where T : ScriptableUnitBase
    //{

    //    // Filter units by typeUnit and typeGround.
    //    List<ScriptableUnitBase> units = new List<ScriptableUnitBase>();

    //    for (int i = 0; i < _units.Count; i++)
    //    {
    //        if (typeUnit == _units[i].TypeUnit && _units[i].TypeGround == typeGround) units.Add(_units[i]);
    //    }
    //    //Debug.Log($"units {faction} Count = {units.Count}");
    //    var index = Random.Range(0, units.Count);

    //    return (T)units[index];
    //}
}