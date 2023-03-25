using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/Units/New Resource")]
public class ScriptableResource : ScriptableUnitBase
{
    public TypeWork TypeWork;
    public TypeResource TypeResource;
    public int maxValue;
    public int step;
    public AnimationCurve Curve;

}

[System.Serializable]
public enum TypeResource
{
    Gold = 10,
    Iron = 20,
    Wood = 30,
    Mercury = 40,
    Diamond = 50,
    Gem = 60,
    Sulfur = 70
}
