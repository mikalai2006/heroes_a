using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataGame
{
    public DataMap dataMap;
    //public DataPlay dataPlay;

    public DataGame()
    {
        dataMap = new DataMap();
        //dataPlay = new DataPlay();
    }
}
