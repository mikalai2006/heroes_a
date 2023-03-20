//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;

//public static class SaveSystem
//{
//    public static void SaveGridMap(List<GridTileNode> listNodes, List<GridTileNatureNode> listNodeNature)
//    {
//        BinaryFormatter formatter = new BinaryFormatter();
//        string path = Application.persistentDataPath + "game.fun";
//        FileStream stream = new FileStream(path, FileMode.Create);

//        DataMap data = new DataMap();
//        //List<GridTileNode> _tileNodes = new List<GridTileNode>();
//        //foreach (GridTileNode node in listNodes)
//        //{
//        //    _tileNodes.Add(node);
//        //}
//        data.mapNode = listNodes;

//        //List<GridTileNatureNode> _tileNatureNodes = new List<GridTileNatureNode>();
//        //foreach (GridTileNatureNode node in listNodeNature)
//        //{
//        //    _tileNatureNodes.Add(node);
//        //}
//        data.natureNode = listNodeNature;
//        formatter.Serialize(stream, data);
//        stream.Close();

//        var json = JsonUtility.ToJson(data);
//        File.WriteAllText(Application.persistentDataPath + "/map.json", json);
//        //Debug.Log("jass0n" + json);

//    }

//    public static void SaveLevel(DataLevel level)
//    {
//        //BinaryFormatter formatter = new BinaryFormatter();
//        //string path = Application.persistentDataPath + "game.fun";
//        //FileStream stream = new FileStream(path, FileMode.Create);

//        //GameData data = new GameData();
//        //List<GridTileNode> _tileNodes = new List<GridTileNode>();
//        //foreach (GridTileNode node in listNodes)
//        //{
//        //    _tileNodes.Add(node);
//        //}
//        //data.listNodes = _tileNodes;

//        //formatter.Serialize(stream, data);
//        //stream.Close();
//        var json = JsonUtility.ToJson(level);

//        File.WriteAllText(Application.persistentDataPath + "/level.json", json);
//        //Debug.Log("level" + json);
//    }

//    public static DataMap LoadGame()
//    {
//        string path = Application.persistentDataPath + "game.fun";

//        if (!File.Exists(path))
//        {
//            BinaryFormatter formatter = new BinaryFormatter();

//            FileStream stream = new FileStream(path, FileMode.Open);

//            DataMap data = formatter.Deserialize(stream) as DataMap;

//            stream.Close();

//            return data;

//        } else
//        {
//            Debug.Log("Save file not found in " + path);
//            return null;
//        }
//    }

//    //public static void SaveLevel(DataLevel data)
//    //{
//    //    //var units = _tileMapUnits.GetComponentsInChildren<UnitBase>();
//    //    //DataUnitsForSave.heroes = new List<SaveDataUnit<DataHero>>();
//    //    //DataUnitsForSave.resources = new List<SaveDataUnit<DataResource>>();
//    //    //DataUnitsForSave.towns = new List<SaveDataUnit<DataTown>>();

//    //    //foreach (UnitBase unit in units)
//    //    //{
//    //    //    System.Type type = unit.GetType();
//    //    //    unit.OnSaveUnit();
//    //    //    Debug.Log($"Save::: {type}");
//    //    //}
//    //    var json = JsonUtility.ToJson(data);
//    //    System.IO.File.WriteAllText(Application.persistentDataPath + "/level.json", json);
//    //}
//}

