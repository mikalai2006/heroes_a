using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataGame
{
    void LoadDataGame(DataGame data);
    void SaveDataGame(ref DataGame data);
}
