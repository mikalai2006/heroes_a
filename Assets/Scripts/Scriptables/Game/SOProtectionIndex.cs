using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ProtectionIndex", menuName = "HeroesA/ProtectionIndex", order = 0)]
public class SOProtectionIndex : ScriptableObject
{
    public LocalizedString title;
    public int protectionIndex;
    public int minimalValue1;
    [Range(0f, 1.5f)] public float koof1;
    public int minimalValue2;
    [Range(0f, 1.5f)] public float koof2;
}
