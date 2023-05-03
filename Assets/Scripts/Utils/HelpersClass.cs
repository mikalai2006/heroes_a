using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding).
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
    }

    /// <summary>
    ///  Get the probability of getting an item
    /// </summary>
    /// <param name="item">Items for search</param>
    /// <returns>result item or first item<T></returns>
	public static ResultProbabiliti<T> GetProbabilityItem<T>(List<ItemProbabiliti<T>> items)
    {
        double p = new System.Random().NextDouble();
        double accumulator = 0.0;
        var result = new ResultProbabiliti<T>()
        {
            Item = items[0].Item,
            index = 0
        };
        for (int i = 0; i < items.Count; i++)
        {
            ItemProbabiliti<T> item = items[i];
            accumulator += item.probability;
            if (p <= accumulator)
            {
                result.Item = item.Item;
                result.index = i;
                break;
            }
        }
        return result;
    }

    public static int GenerateValueByRangeAndStep(int min, int max, int step)
    {
        System.Random rand = new System.Random();
        int fr = rand.Next((max - min) / step);
        int f = fr == 0 ? step : fr * step + min;
        return f;
    }

    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    /// <summary>
    /// Vector 3 to Matrix4x4
    /// </summary>
    /// <param name="input">Vector3</param>
    /// <returns>Vector3</returns>
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    public static List<Transform> GetChildren(this GameObject gameobject, bool recursive)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in gameobject.transform)
        {
            children.Add(child);
            if (recursive)
            {
                children.AddRange(GetChildren(child.gameObject, true));
            }
        }

        return children;
    }

    public static List<Transform> GetDeepChildren<T>(this GameObject gameobject, bool recursive)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in gameobject.transform)
        {
            var isChild = child.GetComponent<T>();
            if (isChild != null)
            {
                children.Add(child);
            }
            if (recursive)
            {
                children.AddRange(GetChildren(child.gameObject, true));
            }
        }

        return children;
    }
    public static Dictionary<Transform, dynamic> GetChildrenHierarchy(this GameObject gameobject)
    {
        Dictionary<Transform, dynamic> children = new Dictionary<Transform, dynamic>();

        foreach (Transform child in gameobject.transform)
        {
            children.Add(child, GetChildrenHierarchy(child.gameObject));
        }

        return children;
    }


    public static string GetStringNameCountWarrior(int n)
    {
        string name = "few";
        switch (n)
        {
            case int i when n > 5 && n <= 9:
                name = "several";
                break;
            case int i when n > 9 && n <= 19:
                name = "pack";
                break;
            case int i when n > 19 && n <= 49:
                name = "lots";
                break;
            case int i when n > 49 && n <= 99:
                name = "horde";
                break;
            case int i when n > 99 && n <= 249:
                name = "throng";
                break;
            case int i when n > 249 && n <= 499:
                name = "swarm";
                break;
            case int i when n > 499 && n <= 999:
                name = "cloud";
                break;
            case int i when n > 999:
                name = "legion";
                break;
        }

        return string.Format("army_{0}", name);
    }


    /// <summary>
    /// Move unit between lists.
    /// </summary>
    /// <param name="startCheckedCreatures"></param>
    /// <param name="startPositionChecked"></param>
    /// <param name="endCheckedCreatures"></param>
    /// <param name="endPositionChecked"></param>
    public static void MoveUnitBetweenList(
        ref SerializableDictionary<int, EntityCreature> startCheckedCreatures,
        int startPositionChecked,
        ref SerializableDictionary<int, EntityCreature> endCheckedCreatures,
        int endPositionChecked,
        int countStart = -1,
        int countEnd = -1
        )
    {
        var startUnit = startCheckedCreatures[startPositionChecked];
        var endUnit = endCheckedCreatures[endPositionChecked];

        if (endUnit != null && startUnit.ConfigAttribute.idObject == endUnit.ConfigAttribute.idObject)
        {
            if (startCheckedCreatures.Where(t => t.Value != null).Count() == 1)
            {
                if (countEnd != -1 && countStart != -1)
                {
                    endUnit.Data.value = countStart == 0 ? countEnd - 1 : countEnd;
                    startUnit.Data.value = countStart == 0 ? 1 : countStart;
                }
                else
                {
                    endUnit.Data.value += startUnit.Data.value - 1;
                    startUnit.Data.value = 1;
                }
            }
            else
            {
                if (countEnd != -1 && countStart != -1)
                {
                    endUnit.Data.value = countEnd;
                    startUnit.Data.value = countStart;
                }
                else
                {
                    endUnit.Data.value += startUnit.Data.value;
                    startUnit = startCheckedCreatures[startPositionChecked] = null;
                }
            }
        }
        else
        {
            if (countEnd != -1 && countStart != -1)
            {
                endCheckedCreatures[endPositionChecked]
                    = endUnit
                    = new EntityCreature(startUnit.ConfigAttribute);

                if (startCheckedCreatures.Where(t => t.Value != null).Count() == 1)
                {
                    endUnit.Data.value = countStart == 0 ? countEnd - 1 : countEnd;
                    startUnit.Data.value = countStart == 0 ? 1 : countStart;
                }
                else
                {
                    endUnit.Data.value = countEnd;
                    startUnit.Data.value = countStart;
                }
            }
            else
            {
                startCheckedCreatures[startPositionChecked] = endUnit;
                endCheckedCreatures[endPositionChecked] = startUnit;
            }
        }

        if (endUnit != null && endUnit.Data.value == 0)
        {
            endUnit = endCheckedCreatures[endPositionChecked] = null;
        }
        if (startUnit != null && startUnit.Data.value == 0)
        {
            startUnit = startCheckedCreatures[startPositionChecked] = null;
        }
    }

    /// <summary>
    /// Summ unit two lists.
    /// </summary>
    /// <param name="listTo"></param>
    /// <param name="listFrom"></param>
    public static SerializableDictionary<int, EntityCreature> SummUnitBetweenList(
        SerializableDictionary<int, EntityCreature> listTo,
        SerializableDictionary<int, EntityCreature> listFrom
        )
    {
        var rezList = new SerializableDictionary<int, EntityCreature>();
        foreach (var item in listTo)
        {
            rezList.Add(item.Key, item.Value);
        }

        foreach (var item in listFrom)
        {
            if (item.Value == null) continue;

            var existItem = rezList
                .Where(t => t.Value != null && t.Value.Data.idObject == item.Value.Data.idObject);

            if (existItem.Count() != 0)
            {
                existItem.First().Value.Data.value += item.Value.Data.value;
            }
            else
            {
                var emptyItems = rezList
                    .Where(t => t.Value == null);
                if (emptyItems.Count() == 0)
                {
                    rezList.Add(rezList.Count, item.Value);
                }
                else
                {
                    rezList[emptyItems.First().Key] = item.Value;
                }
            }
        }
        // var unitFrom = listFrom
        //     .Where(t => t.Value != null)
        //     .Select(t => t.Value)
        //     .ToDictionary(t => t.Data.idObject, t => t);
        // var unitTo = listTo
        //     .Where(t => t.Value != null)
        //     .Select(t => t.Value)
        //     .ToDictionary(t => t.Data.idObject, t => t);

        // foreach (var itemFrom in unitFrom)
        // {
        //     EntityCreature creatureTo;
        //     bool isExistTo = unitTo.TryGetValue(itemFrom.Key, out creatureTo);
        //     if (isExistTo)
        //     {
        //         creatureTo.Data.value += itemFrom.Value.Data.value;
        //     }
        //     else
        //     {
        //         unitTo.Add(itemFrom.Value.Data.idObject, itemFrom.Value);
        //     }
        // }

        // var result = listTo;

        // foreach (var item in unitTo)
        // {
        //     var emptyItemForInsert = result.Where(t => t.Value == null).First();

        //     var itemForInsert = result
        //         .Where(t => t.Value != null && t.Value.Data.idObject == item.Key)
        //         .First();
        //     if (itemForInsert.Value != null)
        //     {
        //         result[itemForInsert.Key] = item.Value;
        //     }
        //     else
        //     {
        //         result[emptyItemForInsert.Key] = item.Value;
        //     }
        //     // result[p] = item.Value;
        // }

        return rezList;
    }

    /// <summary>
    /// Clone Dictionary
    /// </summary>
    /// <param name="original"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
   (Dictionary<TKey, TValue> original) where TValue : ICloneable
    {
        Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                original.Comparer);
        foreach (KeyValuePair<TKey, TValue> entry in original)
        {
            ret.Add(entry.Key, (TValue)entry.Value.Clone());
        }
        return ret;
    }

    public static string GetLocalizedPluralString<T>(
        LocalizedString localizedString,
        Dictionary<string, T>[] args,
        Dictionary<string, T> dictionary
        )
    {
        if (localizedString.IsEmpty) return "NO_LANG";

        localizedString.Arguments = args;
        return localizedString.GetLocalizedString(dictionary);
    }

    public static string GetColorString(string str)
    {
        return " <color=#FFFFAB>" + str + "</color>";
    }
    // public static AsyncOperationHandle<string> LoadLocalizedSmartString(string tableRef, string tableEntryRef, Dictionary<string, string>[] args)
    // {
    //     LocalizedString localizedString = new LocalizedString();
    //     localizedString.TableReference = tableRef;
    //     localizedString.TableEntryReference = tableEntryRef;
    //     localizedString.Arguments = args;

    //     return localizedString.GetLocalizedString();
    // }
}

// public static class HelperLanguage
// {
//     public static LangItem GetLocaleText(this List<LangItem> locale)
//     {
//         var t = locale.Find(t => t.Language == LocalizationSettings.SelectedLocale);
//         return t;
//     }

// }

// public interface IProperty<T> : IProperty
// {
//     new event Action<T> ValueChanged;
//     new T Value { get; }
// }

// public interface IProperty
// {
//     event Action<object> ValueChanged;
//     object Value { get; }
// }

// [Serializable]
// public class Property<T> : IProperty<T>
// {
//     public event Action<T> ValueChanged;

//     event Action<object> IProperty.ValueChanged
//     {
//         add => valueChanged += value;
//         remove => valueChanged -= value;
//     }

//     [SerializeField]
//     private T value;

//     public T Value
//     {
//         get => value;

//         set
//         {
//             if (EqualityComparer<T>.Default.Equals(this.value, value))
//             {
//                 return;
//             }

//             this.value = value;

//             ValueChanged?.Invoke(value);
//             valueChanged?.Invoke(value);
//         }
//     }

//     object IProperty.Value => value;

//     private Action<object> valueChanged;

//     public Property(T value) => this.value = value;

//     public static explicit operator Property<T>(T value) => new Property<T>(value);
//     public static implicit operator T(Property<T> binding) => binding.value;
// }

[System.Serializable]
public struct ItemProbabiliti<T>
{
    public T Item;
    [Range(0, 1)] public double probability;
}

[System.Serializable]
public struct ResultProbabiliti<T>
{
    public T Item;
    public int index;
}