using System.Collections.Generic;

using System;

[System.Serializable]
public class JSONDataEffect
{
    public List<DataEffectItem> effects;
}

[System.Serializable]
public struct DataEffectItem
{
    public string id;
    public List<DataEffectProbability> variants;
}

[System.Serializable]
public struct DataEffectProbability
{
    public string probability;
    public List<DataEffectItemValue> items;
}

[System.Serializable]
public struct DataEffectItemValue
{
    public string idso;
    public Object data;
}

[System.Serializable]
public struct DataEffectResource
{
    public int value;
    public int max;
    public int min;
    public int step;
}