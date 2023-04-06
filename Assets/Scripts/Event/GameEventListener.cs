using UnityEngine;
using UnityEngine.Events;

// [CreateAssetMenu(fileName = "GameEventListener", menuName = "HeroesA/GameEventListener", order = 0)]
public class GameEventListener : MonoBehaviour
{
    public EntityEvent entityEvent;
    public UnityEvent onEventTriggered;

    private void OnEnable()
    {
        entityEvent.AddListener(this);
    }
    private void OnDisable()
    {
        entityEvent.RemoveListener(this);
    }

    public void OnEventTriggered()
    {
        onEventTriggered.Invoke();
    }
}