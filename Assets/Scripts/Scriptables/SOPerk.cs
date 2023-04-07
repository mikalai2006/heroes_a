using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu()]
public class SOPerk : ScriptableObject, IPerked
{
    public string title;
    [SerializeField] public List<ArenaItemSkill> ListPerks;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var perk in ListPerks)
        {

            Debug.Log($"Hello {perk.Perk.name}[{perk.value}] for {player.DataPlayer.id}");
        }
    }
}


[System.Serializable]
public struct ArenaItemSkill
{
    public string id;
    public ScriptableAttribute Perk;
    public int value;
}