using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableShortPosition: Dictionary<Vector3Int, bool>, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> values = new List<string>();

    public void OnBeforeSerialize()
    {
        values.Clear();
        foreach (KeyValuePair<Vector3Int, bool> pair in this)
        {
            values.Add($"{pair.Key.x}:{pair.Key.y}");
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for(int i = 0; i < values.Count; i++)
        {
            string[] arStr = values[i].Split(":");
            this.Add(new Vector3Int(int.Parse(arStr[0]), int.Parse(arStr[1])), true);
        }
    }

}
