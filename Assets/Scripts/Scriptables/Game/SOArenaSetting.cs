using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ArenaSetting", menuName = "HeroesA/ArenaSetting")]
public class SOArenaSetting : ScriptableObject
{
    public string idObject;
    public List<Sprite> BgSprites;
    public TileLandscape NativeGround;
    public List<Sprite> Obstacles;
    public Sprite BgTownArena;
}
