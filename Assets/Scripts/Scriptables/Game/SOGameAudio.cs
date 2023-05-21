using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;


[CreateAssetMenu(fileName = "GameAudio", menuName = "HeroesA/GameAudio")]
public class SOGameAudio : ScriptableObject
{
    public string idObject;
    public AssetReferenceT<AudioClip> buttonClick;

}
