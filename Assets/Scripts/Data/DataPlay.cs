using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPlay
{
    public DataLevel Level;
    public DataUnit Units;

    public DataPlay()
    {
        Units = new DataUnit();
    }
}
