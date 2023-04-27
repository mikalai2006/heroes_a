using System;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScriptableAttribute : ScriptableObject
{
    public string idObject;
    public TypeAttribute TypeAttribute;
    public Sprite MenuSprite;
    [SerializeField] public AssetReferenceGameObject MapPrefab;
    [SerializeField] public LangEntity Text;
}

[Serializable]
public enum TypeAttribute
{
    Resource = 0,
    Artifact = 10,
    PrimarySkill = 30,
    SecondarySkill = 40,
    Spell = 60,
    Hero = 70,
    Creature = 80,
    Town = 100,
}