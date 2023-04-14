
using System.Collections.Generic;

[System.Serializable]
public class DataBuild
{
    public List<SaveDataBuild<DataBuildGeneral>> General;
    public List<SaveDataBuild<DataBuildArmy>> Armys;

    public DataBuild()
    {
        General = new List<SaveDataBuild<DataBuildGeneral>>();
        Armys = new List<SaveDataBuild<DataBuildArmy>>();
    }
}

[System.Serializable]
public class SaveDataBuild<T>
{
    public string idUnit;
    public string idObject;
    public int level;
    public T data;
}

[System.Serializable]
public struct DataBuildArmy
{
    public int quantity;
}

[System.Serializable]
public struct DataBuildGeneral
{
    // public int level;
}
