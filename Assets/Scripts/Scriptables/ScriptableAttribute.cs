using System;

using UnityEngine;

public class ScriptableAttribute : ScriptableObject
{
    public string idObject;
    public TypeAttribute TypeAttribute;
    public Sprite MenuSprite;
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
}