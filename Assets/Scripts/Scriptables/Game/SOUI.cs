using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SOUI", menuName = "HeroesA/SOUI")]
public class SOUI : ScriptableObject
{
    public string idObject;
    public List<Sprite> SpritesWaitStep;
    public Sprite flag;

}