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
    PrimarySkill = 30,
    SecondarySkill = 40,
    Spell = 60,
}