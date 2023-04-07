using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/New Resource")]
public class ScriptableResource : ScriptableObject
{
    public string idObject;
    public Sprite MenuSprite;
    [SerializeField] public LangEntity Text;
    public TypeResource TypeResource;
    // public int maxValue;
    // public int step;
    // public AnimationCurve Curve;
}

[System.Serializable]
public enum TypeResource
{
    Gold = 10,
    Ore = 20,
    Wood = 30,
    Mercury = 40,
    Crystal = 50,
    Gems = 60,
    Sulfur = 70
}
