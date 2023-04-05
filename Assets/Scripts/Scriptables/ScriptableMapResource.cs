using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/Map/New Resource")]
public class ScriptableMapResource : ScriptableMapObjectBase
{
    // public TypeWork TypeWork;
    public TypeResource TypeResource;
    public int maxValue;
    public int step;
    public AnimationCurve Curve;

}
