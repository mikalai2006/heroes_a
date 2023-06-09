using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class SerializableNoSky : Dictionary<Vector3Int, NoskyMask>, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> values = new List<string>();
    [SerializeField] private List<NoskyMask> keys = new List<NoskyMask>();

    public void OnBeforeSerialize()
    {
        values.Clear();
        keys.Clear();
        foreach (KeyValuePair<Vector3Int, NoskyMask> pair in this)
        {
            values.Add($"{pair.Key.x}:{pair.Key.y}");
            keys.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < values.Count; i++)
        {
            string[] arStr = values[i].Split(":");
            this.Add(new Vector3Int(int.Parse(arStr[0]), int.Parse(arStr[1])), keys[i]);
        }
    }

}

[System.Flags]
public enum NoskyMask
{
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2,
    Four = 1 << 3,
    Five = 1 << 4,
    Six = 1 << 5,
    Seven = 1 << 6,
    Eight = 1 << 7,
    Nine = 1 << 8,
    Ten = 1 << 9
}
